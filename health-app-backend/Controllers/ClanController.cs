using health_app_backend.DTOs;
using health_app_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers;

[ApiController]
[Route("api/clan")]
public class ClanController : ControllerBase
{
   private readonly IClanService _clanService;

   public ClanController(IClanService clanService)
   {
      _clanService = clanService;
   }
   
   // Get all clans
   [HttpGet]
   public async Task<ActionResult<IEnumerable<ClanSearchDto>>> GetClans()
   {
      try
      {
         var clans = await _clanService.GetClansAsync();
         return Ok(clans);
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { message = "An error occurred while fetching clans.", details = ex.Message });
      }
   }
   
   // Get clan details by clan ID
   [HttpGet("{clanId}")]
   public async Task<ActionResult<ClanDetailsDto>> GetClanDetails(string clanId)
   {
      try
      {
         var clanDetails = await _clanService.GetClanDetailsAsync(clanId);
         if (clanDetails == null)
         {
            return NotFound(new { message = "Clan not found." });
         }
         return Ok(clanDetails);
      }
      catch (ArgumentException ex)
      {
         return BadRequest(new { message = ex.Message });
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { message = "An error occurred while fetching clan details.", details = ex.Message });
      }
   }
   
   // Create a new clan
   [HttpPost]
   public async Task<ActionResult<string>> CreateClan([FromBody] ClanCreateDto details)
   {
      try
      {
         var clanId = await _clanService.CreateClan(details);
         return CreatedAtAction(nameof(GetClanDetails), new { clanId }, clanId);
      }
      catch (Exception ex)
      {
         return BadRequest(new { message = ex.Message });
      }
   }
   
   // Send a clan invite
   [HttpPost("{clanId}/invite/{userId}")]
   public async Task<ActionResult> SendClanInvite(string clanId, string userId)
   {
      try
      {
         var result = await _clanService.SendClanInvite(clanId, userId);
         if (result)
         {
            return Ok(new { message = "Invite sent successfully." });
         }
         return BadRequest(new { message = "Failed to send invite." });
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { message = "An error occurred while sending the invite.", details = ex.Message });
      }
   }
   
   // Accept a clan invite
   [HttpPost("invite/accept/{requestId}")]
   public async Task<ActionResult> AcceptClanInvite(string requestId)
   {
      try
      {
         var result = await _clanService.AcceptClanInvite(requestId);
         if (result)
         {
            return Ok(new { message = "Invite accepted successfully." });
         }
         return BadRequest(new { message = "Failed to accept invite." });
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { message = "An error occurred while accepting the invite.", details = ex.Message });
      }
   }
   
   // Decline a clan invite
   [HttpPost("invite/decline/{requestId}")]
   public async Task<ActionResult> DeclineClanInvite(string requestId)
   {
      try
      {
         var result = await _clanService.DeclineClanInvite(requestId);
         if (result)
         {
            return Ok(new { message = "Invite declined successfully." });
         }
         return BadRequest(new { message = "Failed to decline invite." });
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { message = "An error occurred while declining the invite.", details = ex.Message });
      }
   }
   
   // Get all invites for a clan
   [HttpGet("{clanId}/invites")]
   public async Task<ActionResult<IEnumerable<ClanInviteDto>>> GetClanInvites(string clanId)
   {
      try
      {
         var invites = await _clanService.GetClanInvites(clanId);
         return Ok(invites);
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { message = "An error occurred while fetching invites.", details = ex.Message });
      }
   }
   
   // Leave clan
   [HttpPost("{clanId}/leave/{userId}")]
   public async Task<ActionResult> LeaveClan(string clanId, string userId)
   {
      try
      {
         var result = await _clanService.LeaveClan(clanId, userId);
         if (result)
         {
            return Ok(new { message = "Clan left successfully." });
         }
         else
         {
            return BadRequest(new { message = "Failed to leave clan." });
         }
      }
      catch (Exception e)
      {
         Console.WriteLine(e);
         return StatusCode(500, new { message = e.Message });
         throw;
      }
   }
   
   // Get all challenges for a clan
   [HttpGet("{clanId}/challenges")]
   public async Task<ActionResult<IEnumerable<ClanChallengeDto>>> GetClanChallenges(string clanId)
   {
      try
      {
         var challenges = await _clanService.GetClanChallenges(clanId);
         return Ok(challenges);
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { message = "An error occurred while fetching challenges.", details = ex.Message });
      }
   }

   [HttpPost("{clanId}/challenge")]
   public async Task<ActionResult<ClanChallengeDto>> CreateClanChallenge([FromBody] ClanChallengeCreateDto details)
   {
      try
      {
         // Validate the input ClanId matches the one in the route
         if (details.ClanId != null && !details.ClanId.Equals(details.ClanId))
         {
            return BadRequest(new { message = "Clan ID in the route does not match the Clan ID in the body." });
         }

         var clanChallenge = await _clanService.CreateClanChallenge(details);
         return CreatedAtAction(nameof(GetClanChallenges), new { clanId = details.ClanId }, clanChallenge);
      }
      catch (ArgumentException ex)
      {
         return BadRequest(new { message = ex.Message });
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { message = "An error occurred while creating the challenge.", details = ex.Message });
      }
   }

}