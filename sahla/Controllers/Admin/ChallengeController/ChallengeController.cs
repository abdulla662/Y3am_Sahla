using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sahla.DTOs.Response;

namespace sahla.Controllers.Admin.ChallengeController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChallengeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChallengeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
                _unitOfWork = unitOfWork;
            _userManager = userManager; 
        }
        [HttpPost("CreateChallenge")]
        public IActionResult CreateChallenge(CreateChallengeDto challengeDto) {
            if (challengeDto == null) return BadRequest("Challenge Is Empty");
            var challenge = challengeDto.Adapt<Challenge>();
            _unitOfWork.Challenges.Create(challenge);
            _unitOfWork.Challenges.comit();
            return Ok("Challenge Created Succefully");
        }
        [HttpGet("GetAllChallenges")]
        public IActionResult GetAllChallenges() { 
        var challenges = _unitOfWork.Challenges.Get().ToList();
            if (challenges == null) return BadRequest("No Challenges Is Found");
            var data = challenges.Adapt<List<GetAllChallengesDto>>();
            return Ok(data);
        }
        [HttpPut("EditChallenge")]
        public IActionResult EditChallenge(UpdateChallengeDto updateChallengeDto)
        {
            if (updateChallengeDto == null) return BadRequest("Challenhge Is Empty");
            var challenge = updateChallengeDto.Adapt<Challenge>();
            _unitOfWork.Challenges.Edit(challenge);
            _unitOfWork.Challenges.comit();
            return Ok();
        }
        [HttpGet("{id}")]
        public IActionResult GetChallengeById(int id)
        {
            var challenge = _unitOfWork.Challenges.GetOne(e => e.ChallengeId == id);
            if (challenge == null)
                return NotFound("Challenge not found");

            var result = challenge.Adapt<GetAllChallengesDto>();
            return Ok(result);
        }
        [HttpDelete("DeleteChallenge")]
        public IActionResult DeleteChallenge(int id) {
            var data = _unitOfWork.Challenges.GetOne(e => e.ChallengeId == id);
            if (data == null) return BadRequest("Null");
            _unitOfWork.Challenges.Delete(data);
            _unitOfWork.Challenges.comit();
            return Ok();
        }

    }
}
