using AutoMapper;
using health_app_backend.DTOs;
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

        public HealthDataService(IHealthDataRepository healthDataRepository, IUserRepository userRepository, IClanMemberRepository clanMemberRepository, IDataTypeRepository dataTypeRepository, IClanService clanService, IMapper mapper)
        {
            _healthDataRepository = healthDataRepository;
            _userRepository = userRepository;
            _clanMemberRepository = clanMemberRepository;
            _dataTypeRepository = dataTypeRepository;
            _clanService = clanService;
            _mapper = mapper;
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
    }
}
