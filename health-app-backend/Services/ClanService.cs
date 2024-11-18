using AutoMapper;
using health_app_backend.DTOs;
using health_app_backend.Models;
using health_app_backend.Repositories;

namespace health_app_backend.Services;

public class ClanService : IClanService
{
    private readonly IClanRepository _clanRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClanJoinRequestRepository _clanJoinRequestRepository;
    private readonly IMapper _mapper;
    private readonly IClanChallengeRepository _clanChallengeRepository;

    public ClanService(
        IClanRepository clanRepository,
        IUserRepository userRepository,
        IClanJoinRequestRepository clanJoinRequestRepository,
        IClanChallengeRepository clanChallengeRepository,
        IMapper mapper)
    {
        _clanRepository = clanRepository;
        _userRepository = userRepository;
        _clanJoinRequestRepository = clanJoinRequestRepository;
        _clanChallengeRepository = clanChallengeRepository;
        _mapper = mapper;
    }
    
    // Get Clans (when searching) ---------------------------------------------------------------------------------------------------
    public async Task<IEnumerable<ClanSearchDto>> GetClansAsync()
    {
        var clans = await _clanRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ClanSearchDto>>(clans);
    }
    
    // Get Clan Details ---------------------------------------------------------------------------------------------------

    public async Task<ClanDetailsDto> GetClanDetailsAsync(string clanId)
    {
        // Parse the clanId from string to Guid
        if (!Guid.TryParse(clanId, out var clanGuid))
        {
            throw new ArgumentException("Invalid clan ID format");
        }
        
        // Fetch the clan with members
        var clan = await _clanRepository.GetClanWithMembersAsync(clanGuid);
        if (clan == null)
        {
            throw new Exception("Clan not found");
        }
        
        var clanDetailsDto = _mapper.Map<ClanDetailsDto>(clan);
        return clanDetailsDto;
    }
    
    // Create Clan ---------------------------------------------------------------------------------------------------
    public async Task<string> CreateClan(ClanCreateDto details)
    {
        var leader = await _userRepository.GetByIdAsync(Guid.Parse(details.LeaderId));
        if (leader == null)
        {
            throw new Exception("Leader not found.");
        }
        // Create the new clan entity
        var clan = new Clan
        {
            Id = Guid.NewGuid(),
            Name = details.Name,
            Description = details.Description,
            Location = details.Location,
            LeaderId = Guid.Parse(details.LeaderId),
            Members = new List<ClanMember>
            {
                new ClanMember
                {
                    Id = Guid.NewGuid(),
                    UserId = leader.Id,
                    Role = "Leader",
                    JoinedAt = DateTime.UtcNow
                }
            }
        };

        // Save clan to database
        await _clanRepository.AddAsync(clan);
        await _clanRepository.SaveChangesAsync();
        return clan.Id.ToString();
    }
    
    // Send Clan Invite ---------------------------------------------------------------------------------------------------
    public async Task<bool> SendClanInvite(string clanId, string userId)
    {
        var clan = await _clanRepository.GetByIdAsync(Guid.Parse(clanId));
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

        // Validate clan and user existence
        if (clan == null || user == null)
        {
            return false;
        }

        // Create and save the clan join request
        var invite = new ClanJoinRequest
        {
            Id = Guid.NewGuid(),
            ClanId = clan.Id,
            UserId = user.Id,
            RequestedAt = DateTime.UtcNow,
            IsPending = true
        };

        await _clanJoinRequestRepository.AddAsync(invite);
        await _clanJoinRequestRepository.SaveChangesAsync();
        return true;
    }
    
    // Accept Clan Invite ---------------------------------------------------------------------------------------------------
    public async Task<bool> AcceptClanInvite(string requestId)
    {
        var request = await _clanJoinRequestRepository.GetByIdAsync(Guid.Parse(requestId));

        // Check if the request is pending
        if (request == null || !request.IsPending)
        {
            return false;
        }

        await _clanJoinRequestRepository.ApproveJoinRequestAsync(request.Id);
        return true;
    }
    
    // Decline Clan Invite ---------------------------------------------------------------------------------------------------
    public async Task<bool> DeclineClanInvite(string requestId)
    {
        var request = await _clanJoinRequestRepository.GetByIdAsync(Guid.Parse(requestId));

        // Check if the request is pending
        if (request == null || !request.IsPending)
        {
            return false;
        }

        await _clanJoinRequestRepository.RejectJoinRequestAsync(request.Id);
        return true;
    }
    
    // Get Clan Invites ---------------------------------------------------------------------------------------------------
    public async Task<IEnumerable<ClanInviteDto>> GetClanInvites(string clanId)
    {
        var invites = await _clanJoinRequestRepository.GetPendingRequestsForClanAsync(Guid.Parse(clanId));
        return _mapper.Map<IEnumerable<ClanInviteDto>>(invites);
    }
    
    // Get Clan Challenges ---------------------------------------------------------------------------------------------------
    public async Task<IEnumerable<ClanChallengeDto>> GetClanChallenges(string clanId)
    {
        try
        {
            var challenges = await _clanChallengeRepository.GetChallengesByClanIdAsync(Guid.Parse(clanId));
            return _mapper.Map<IEnumerable<ClanChallengeDto>>(challenges);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    // Update Clan Challenge Progress ---------------------------------------------------------------------------------------------------
    public async Task UpdateClanChallengeProgress(Guid clanId, string dataType, float contributionAmount)
    {
        var activeChallenges = await _clanChallengeRepository.GetChallengesByClanIdAsync(clanId);
        
        // Iterate over each challenge and update the progress if criteria are met
        foreach (var challenge in activeChallenges)
        {
            if (challenge.EndDate > DateTime.UtcNow && challenge.DataType == dataType)
            {
                // Update the progress
                challenge.TotalProgress += contributionAmount;
                

                // Save the updated challenge
                _clanChallengeRepository.Update(challenge);
            }
        }
        
        await _clanChallengeRepository.SaveChangesAsync();
        
    }
}