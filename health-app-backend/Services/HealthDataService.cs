using System.Text;
using AutoMapper;
using health_app_backend.DTOs;
using health_app_backend.Helpers;
using health_app_backend.Models;
using health_app_backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Services
{
    public class HealthDataService : IHealthDataService
    {
        private readonly IHealthDataRepository _healthDataRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IClanMemberRepository _clanMemberRepository;
        private readonly IClanService _clanService;
        private readonly IDataTypeRepository _dataTypeRepository;
        private readonly IDailySyncRecordRepository _dailySyncRecordRepository;
        private readonly IBlockchainService _blockchainService;

        public HealthDataService(IHealthDataRepository healthDataRepository, IUserRepository userRepository, IClanMemberRepository clanMemberRepository, IDataTypeRepository dataTypeRepository, IDailySyncRecordRepository dailySyncRecordRepository, IClanService clanService, IMapper mapper, IBlockchainService blockchainService)
        {
            _healthDataRepository = healthDataRepository;
            _userRepository = userRepository;
            _clanMemberRepository = clanMemberRepository;
            _dataTypeRepository = dataTypeRepository;
            _dailySyncRecordRepository =  dailySyncRecordRepository;
            _clanService = clanService;
            _mapper = mapper;
            _blockchainService = blockchainService;
        }

        // Get HealthData by Id
        public async Task<HealthDataResponseDto> GetHealthDataAsync(Guid healthDataId)
        {
            // if (!Guid.TryParse(healthDataId, out var guidId))
            // {
            //     throw new ArgumentException("Invalid HealthData ID format.");
            // }

            var healthData = await _healthDataRepository.GetByIdAsync(healthDataId);
            return healthData != null ? _mapper.Map<HealthDataResponseDto>(healthData) : null;
        }

        // Get all HealthData by Username
        public async Task<IEnumerable<HealthDataResponseDto>> GetHealthDataByUserIdAsync(Guid userId)
        {
            var healthDataList = await _healthDataRepository.GetAllByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<HealthDataResponseDto>>(healthDataList);
        }

        // Get HealthData by Username within a date range
        public async Task<IEnumerable<HealthDataResponseDto>> GetHealthDataByUsernameAndFromDateAsync(string username, string fromDate, string toDate)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (!DateTime.TryParse(fromDate, out var startDate) || !DateTime.TryParse(toDate, out var endDate))
            {
                throw new ArgumentException("Invalid date format. Use yyyy-MM-dd or yyyy-MM-ddTHH:mm:ss.");
            }

            // Convert Unspecified DateTimeKind to UTC
            if (startDate.Kind == DateTimeKind.Unspecified)
            {
                startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            }

            if (endDate.Kind == DateTimeKind.Unspecified)
            {
                endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            }

            var healthDataList = await _healthDataRepository.GetAllByUserIdAndDateRangeAsync(user.Id, startDate, endDate);
            return _mapper.Map<IEnumerable<HealthDataResponseDto>>(healthDataList);
        }
        
        // Get data of a specific type for a specific user ID
        public async Task<IEnumerable<HealthDataResponseDto>> GetHealthDataByUserIdAndTypeAsync(Guid userId,
            int datatypeId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var healthDataList = await _healthDataRepository.GetAllByUserIdAsync(userId);
            
            // Filter by datatypeId
            var filteredHealthDataList = healthDataList.Where(hd => hd.DatatypeId == datatypeId);
            
            // Map to DTOs and return
            return _mapper.Map<IEnumerable<HealthDataResponseDto>>(filteredHealthDataList);
        }


        // Add new HealthData
        public async Task<string> AddHealthDataAsync(HealthDataCreateDto healthDataCreateDto)
        {
            var user = await _userRepository.GetByIdAsync(healthDataCreateDto.UserId);
            if (user == null)
            {
                throw new Exception("Invalid UserId provided.");
            }
            
            // Calculate the user's local day based on their time zone offset
            var userLocalDate = healthDataCreateDto.RecordedAt.AddMinutes(healthDataCreateDto.TimeZoneOffset).Date;
            Console.WriteLine($"User's local date: {userLocalDate}");
            
            // This will be used to automatically contribute towards clan challenges when data is updated
            float contributionAmount;
            Console.WriteLine($"Adding health data for date: {healthDataCreateDto.RecordedAt}");
            
            string idToReturn = null;
            
            // Check if there's already an entry for the same user, datatype, and day
            var existingHealthData = await _healthDataRepository
                .GetAll() // Assuming GetAll() returns IQueryable<HealthData>
                .Where(hd => hd.UserId == healthDataCreateDto.UserId &&
                             hd.DatatypeId == healthDataCreateDto.DatatypeId &&
                             hd.RecordedAt.AddMinutes(healthDataCreateDto.TimeZoneOffset).Date == userLocalDate)
                .FirstOrDefaultAsync();
            
            if (existingHealthData != null)
            {
                // Calculate the difference in data value for progress update
                contributionAmount = healthDataCreateDto.DataValue - existingHealthData.DataValue;
                
                // Update existing record's value and RecordedAt
                existingHealthData.DataValue = healthDataCreateDto.DataValue;
                existingHealthData.RecordedAt = healthDataCreateDto.RecordedAt;

                _healthDataRepository.Update(existingHealthData);
                await _healthDataRepository.SaveChangesAsync();

                idToReturn = existingHealthData.Id.ToString();
            }
            else
            {
                // No existing record, create a new one
                var healthData = _mapper.Map<HealthData>(healthDataCreateDto);
                healthData.Id = Guid.NewGuid();

                await _healthDataRepository.AddAsync(healthData);
                
                // Full contribution as there was no prior record
                contributionAmount = healthDataCreateDto.DataValue;
                
                idToReturn = healthData.Id.ToString();
            }
            
            await _healthDataRepository.SaveChangesAsync();
            
            var dataType = await _dataTypeRepository.GetByIdAsync(healthDataCreateDto.DatatypeId);
            
            // Fetch the user's clan and update the challenge progress
            var clanMember = await _clanMemberRepository.GetByUserIdAsync(user.Id);
            if (clanMember != null)
            {
                await _clanService.UpdateClanChallengeProgress(clanMember.ClanId, dataType.Name, contributionAmount);
            }

            return idToReturn;
        }
        
        // Sync data with the blockchain
        public async Task SyncYesterdayDataAsync(Guid userId, DateTime userLocalNow)
        {
            var yesterdayDate = userLocalNow.Date.AddDays(-1);

            // Check if already synced
            var alreadySynced = await _dailySyncRecordRepository.IsSynced(userId, yesterdayDate);
            if (alreadySynced)
            {
                Console.WriteLine($"Data for {yesterdayDate} already synced on-chain.");
                return;
            }

            // Get HealthData records for yesterday
            var healthDataRecords = await _healthDataRepository.GetAllByUserIdAndDateRangeAsync(
                userId,
                yesterdayDate,
                yesterdayDate.AddDays(1).AddTicks(-1)
            );

            if (!healthDataRecords.Any())
            {
                Console.WriteLine($"No data for {yesterdayDate}. Nothing to sync.");
                return;
            }

            // Generate combined string of health data
            var sb = new StringBuilder();
            // Always sort by DatatypeId to ensure consistent hashing
            var sortedRecords = healthDataRecords
                .OrderBy(r => r.DatatypeId)
                .ToList();
            foreach (var record in sortedRecords)
            {
                sb.Append($"{record.DatatypeId}:{record.DataValue}:{record.RecordedAt.Ticks}|");
            }

            var concatenatedData = sb.ToString();
            var dataHash = HashingUtils.ComputeSha256Hash(concatenatedData);

            var distinctDataTypes = healthDataRecords.Select(r => r.DatatypeId).Distinct().Count();

            // Submit to blockchain
            await _blockchainService.SubmitDataHashAsync(userId, dataHash);
            await _blockchainService.SubmitDailyDataAsync(userId.ToString(), distinctDataTypes, false);

            // Mark as synced
            await _dailySyncRecordRepository.SetSynced(userId, yesterdayDate);

            Console.WriteLine($"✅ Successfully synced {yesterdayDate} on-chain.");
        }
    }
}
