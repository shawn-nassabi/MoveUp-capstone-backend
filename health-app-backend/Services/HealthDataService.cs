using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using health_app_backend.DTOs;
using health_app_backend.Models;
using health_app_backend.Repositories;

namespace health_app_backend.Services
{
    public class HealthDataService : IHealthDataService
    {
        private readonly IHealthDataRepository _healthDataRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public HealthDataService(IHealthDataRepository healthDataRepository, IUserRepository userRepository, IMapper mapper)
        {
            _healthDataRepository = healthDataRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // Get HealthData by Id
        public async Task<HealthDataResponseDto> GetHealthDataAsync(string healthDataId)
        {
            if (!Guid.TryParse(healthDataId, out var guidId))
            {
                throw new ArgumentException("Invalid HealthData ID format.");
            }

            var healthData = await _healthDataRepository.GetByIdAsync(guidId);
            return healthData != null ? _mapper.Map<HealthDataResponseDto>(healthData) : null;
        }

        // Get all HealthData by Username
        public async Task<List<HealthDataResponseDto>> GetHealthDataByUsernameAsync(string username)
        {
            var healthDataList = await _healthDataRepository.GetAllByUsernameAsync(username);
            return _mapper.Map<List<HealthDataResponseDto>>(healthDataList);
        }

        // Get HealthData by Username within a date range
        public async Task<List<HealthDataResponseDto>> GetHealthDataByUsernameAndFromDateAsync(string username, string fromDate, string toDate)
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
            return _mapper.Map<List<HealthDataResponseDto>>(healthDataList);
        }


        // Add new HealthData
        public async Task<string> AddHealthDataAsync(HealthDataCreateDto healthDataCreateDto)
        {
            var user = await _userRepository.GetByIdAsync(healthDataCreateDto.UserId);
            if (user == null)
            {
                throw new Exception("Invalid UserId provided.");
            }

            var healthData = _mapper.Map<HealthData>(healthDataCreateDto);
            healthData.Id = Guid.NewGuid();

            await _healthDataRepository.AddAsync(healthData);
            await _healthDataRepository.SaveChangesAsync();

            return healthData.Id.ToString();
        }
    }
}
