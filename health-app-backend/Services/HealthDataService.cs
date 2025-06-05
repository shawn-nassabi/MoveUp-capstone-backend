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
                existingHealthData.TimeZoneOffset = healthDataCreateDto.TimeZoneOffset;

                _healthDataRepository.Update(existingHealthData);
                await _healthDataRepository.SaveChangesAsync();

                idToReturn = existingHealthData.Id.ToString();
            }
            else
            {
                // No existing record, create a new one
                var healthData = _mapper.Map<HealthData>(healthDataCreateDto);
                healthData.Id = Guid.NewGuid();
                healthData.TimeZoneOffset = healthDataCreateDto.TimeZoneOffset;

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
        public async Task SyncYesterdayDataAsync(Guid userId, DateTime nowUtc)
        {
            // 1) Determine "today" in the user’s local time.
            //    We need the user’s offset. We could fetch the *last known* offset for this user, but
            //    actually each HealthData row has its own offset. Usually that offset won't change
            //    for a given user unless they travel across time zones. If you assume they stay in one
            //    offset per day, you can fetch any one of their latest offsets (e.g., look at their most
            //    recent HealthData row or store the offset in the User record).
            
            // For simplicity, let’s look up the user's most recent HealthData offset (if any):
            var mostRecent = await _healthDataRepository.GetMostRecentByUserIdAsync(userId);
            if (mostRecent == null)
            {
                Console.WriteLine($"[Sync] No health data at all for user {userId}. Skipping.");
                return;
            }

            var offsetMinutes = mostRecent.TimeZoneOffset ?? 0; // e.g. -300 for UTC-5
            // Convert "nowUtc" to that user’s local DateTime:
            var userLocalNow = nowUtc.AddMinutes(offsetMinutes);

            // 2) Figure out the user's "yesterday" local‐day boundaries in UTC:
            var localYestDate = userLocalNow.Date.AddDays(-1);
            
            // “Local midnight start” in UTC = midnight local minus offset:
            var utcWindowStart = localYestDate.AddMinutes(-offsetMinutes);           // 00:00 local → UTC
            var utcWindowEnd   = localYestDate.AddDays(1).AddMinutes(-offsetMinutes).AddTicks(-1);
            // (i.e. 23:59:59.999 local → UTC)

            // 3) Before querying, see if we already recorded a sync for that local‐day:
            //    We can store the "synced date" itself in the DailySyncRecords table.
            if (await _dailySyncRecordRepository.IsSynced(userId, localYestDate))
            {
                Console.WriteLine($"[Sync] Local‐day {localYestDate:yyyy-MM-dd} already synced for user {userId}.");
                return;
            }

            // 4) Now fetch all health rows in that UTC window:
            var healthDataRecords = await _healthDataRepository.GetAllByUserIdAndDateRangeAsync(
                userId,
                utcWindowStart, 
                utcWindowEnd
            );

            if (!healthDataRecords.Any())
            {
                Console.WriteLine($"[Sync] No data for user {userId} on local date {localYestDate:yyyy-MM-dd}.");
                return;
            }
            
            // 5) Build the hash/extract data types, etc.
            
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

            // 6) Push both pieces to blockchain:
            await _blockchainService.SubmitDataHashAsync(userId, dataHash);
            await _blockchainService.SubmitDailyDataAsync(userId.ToString(), distinctDataTypes, false);

            // 7) Mark “localYestDate” as synced:
            await _dailySyncRecordRepository.SetSynced(userId, localYestDate);

            Console.WriteLine($"✅ Synced user {userId} local‐day {localYestDate:yyyy-MM-dd} → on‐chain.");
        }
        
    }
}
