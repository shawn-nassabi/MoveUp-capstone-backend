using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using health_app_backend.Helpers;
using health_app_backend.DTOs;
using health_app_backend.Models;
using health_app_backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Services
{
    public class DemographicBenchmarkService : IDemographicBenchmarkService
    {
        private readonly IDemographicBenchmarkRepository _benchmarkRepository;
        private readonly IHealthDataRepository _healthDataRepository;
        private readonly IMapper _mapper;
        private readonly IUserBenchmarkRecordRepository _userBenchmarkRecordRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDataTypeRepository _dataTypeRepository;

        public DemographicBenchmarkService(IDemographicBenchmarkRepository benchmarkRepository, IHealthDataRepository healthDataRepository, IUserBenchmarkRecordRepository userBenchmarkRecord, ILocationRepository locationRepository, IUserRepository userRepository, IDataTypeRepository dataTypeRepository, IMapper mapper)
        {
            _benchmarkRepository = benchmarkRepository;
            _healthDataRepository = healthDataRepository;
            _userBenchmarkRecordRepository = userBenchmarkRecord;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _dataTypeRepository = dataTypeRepository;
            _mapper = mapper;
        }
        

        public async Task<UserBenchmarkResponseDto> GetOrCreateBenchmarkAsync(UserBenchmarkCreateDto userBenchmarkCreateDto)
        {
            // Date threshold for recent benchmarks (1 day old)
            DateTime thresholdDate = DateTime.UtcNow.AddDays(-1);

            // Check if a recent benchmark exists
            var existingBenchmark = await _benchmarkRepository
                .GetAll()
                .Include(b => b.Location)
                .Where(b => b.AgeRange == $"{userBenchmarkCreateDto.MinAge}-{userBenchmarkCreateDto.MaxAge}"
                            && b.Gender == userBenchmarkCreateDto.Gender 
                            && b.LocationId == userBenchmarkCreateDto.LocationId 
                            && b.DataTypeId == userBenchmarkCreateDto.DataTypeId 
                            && b.Timeframe == userBenchmarkCreateDto.TimeFrame
                            && b.CreatedAt >= thresholdDate)
                .FirstOrDefaultAsync();

            if (existingBenchmark != null)
            {
                Console.WriteLine("Benchmark already exists! and the location is" + existingBenchmark.Location.CityName);
                // If the benchmark exists but no UserBenchmarkRecord exists, create one
                var newUserBenchmarkRecordA = new UserBenchmarkRecordModel
                {
                    Id = Guid.NewGuid(),
                    DataValue = userBenchmarkCreateDto.DataValue,
                    DemographicBenchmark = existingBenchmark,
                    DemographicBenchmarkId = existingBenchmark.Id,
                    UserId = userBenchmarkCreateDto.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                await _userBenchmarkRecordRepository.AddAsync(newUserBenchmarkRecordA);
                await _userBenchmarkRecordRepository.SaveChangesAsync();

                return _mapper.Map<UserBenchmarkResponseDto>(newUserBenchmarkRecordA);
            }

            // Determine start date based on timeframe
            DateTime startDate = userBenchmarkCreateDto.TimeFrame.ToLower() == "month" ? DateTime.UtcNow.AddMonths(-1) : DateTime.UtcNow.AddDays(-7);

            // Filter HealthData based on age range and other criteria
            var healthDataQuery = _healthDataRepository.GetAll()
                .Where(hd => hd.User.Age >= userBenchmarkCreateDto.MinAge 
                             && hd.User.Age <= userBenchmarkCreateDto.MaxAge 
                             && hd.User.Gender == userBenchmarkCreateDto.Gender 
                             && hd.User.LocationId == userBenchmarkCreateDto.LocationId 
                             && hd.DatatypeId == userBenchmarkCreateDto.DataTypeId 
                             && hd.RecordedAt >= startDate);

            // Execute query and calculate average
            var healthDataList = await healthDataQuery.ToListAsync();
            float averageValue = healthDataList.Count > 0 
                ? healthDataList.Average(hd => hd.DataValue) 
                : 0;
            
            var healthDataTypeName = _dataTypeRepository.GetByIdAsync(userBenchmarkCreateDto.DataTypeId).Result.Name;
            
            // Calculate recommended value
            float recommendedValue = RecommendationHelper.CalculateRecommendedValue(userBenchmarkCreateDto.MinAge, userBenchmarkCreateDto.MaxAge, userBenchmarkCreateDto.Gender, healthDataTypeName, averageValue);

            // Create and save new benchmark
            var newBenchmark = new DemographicBenchmark
            {
                Id = Guid.NewGuid(),
                AgeRange = $"{userBenchmarkCreateDto.MinAge}-{userBenchmarkCreateDto.MaxAge}",
                Gender = userBenchmarkCreateDto.Gender,
                LocationId = userBenchmarkCreateDto.LocationId,
                DataTypeId = userBenchmarkCreateDto.DataTypeId,
                Timeframe = userBenchmarkCreateDto.TimeFrame,
                AverageValue = averageValue,
                RecommendedValue = recommendedValue,
                CreatedAt = DateTime.UtcNow
            };

            await _benchmarkRepository.AddAsync(newBenchmark);
            await _benchmarkRepository.SaveChangesAsync();

            var newUserBenchmarkRecord = new UserBenchmarkRecordModel
            {
                Id = Guid.NewGuid(),
                DataValue = userBenchmarkCreateDto.DataValue,
                DemographicBenchmark = newBenchmark,
                DemographicBenchmarkId = newBenchmark.Id,
                UserId = userBenchmarkCreateDto.UserId,
                CreatedAt = DateTime.UtcNow
            };
            
            await _userBenchmarkRecordRepository.AddAsync(newUserBenchmarkRecord);
            await _userBenchmarkRecordRepository.SaveChangesAsync();
            
            string cityName = _locationRepository.GetByIdAsync(newBenchmark.LocationId).Result.CityName;
            
            var responseDto = _mapper.Map<UserBenchmarkResponseDto>(newUserBenchmarkRecord);
            responseDto.LocationName = cityName;

            return responseDto;
        }
        

    }
}
