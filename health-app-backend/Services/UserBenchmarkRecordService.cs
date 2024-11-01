using AutoMapper;
using health_app_backend.DTOs;
using health_app_backend.Models;
using health_app_backend.Repositories;

namespace health_app_backend.Services;

public class UserBenchmarkRecordService : IUserBenchmarkRecordService
{
    private readonly IUserBenchmarkRecordRepository _userBenchmarkRecordRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public UserBenchmarkRecordService(IUserBenchmarkRecordRepository userBenchmarkRecordRepository, IUserRepository userRepository, IMapper mapper)
    {
        _userBenchmarkRecordRepository = userBenchmarkRecordRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserBenchmarkResponseDto>> GetUserBenchmarkRecordsByUserIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null; // User not found
        }
        
        // Retrieve UserBenchmarkRecords for the specified userId
        var userBenchmarkRecords = await _userBenchmarkRecordRepository.GetAllByUserIdAsync(userId);

        // Map the results to UserBenchmarkResponseDto
        return _mapper.Map<IEnumerable<UserBenchmarkResponseDto>>(userBenchmarkRecords);
        
    }
}