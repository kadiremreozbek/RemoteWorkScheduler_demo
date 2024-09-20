using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;
using RemoteWorkScheduler.Services;
using RemoteWorkScheduler.Validators;


namespace RemoteWorkScheduler.Controllers
{
    [Route("api/teams")]
    [ApiController]

    public class TeamController : ControllerBase
    {
        private readonly IReWoSeRepository _reWoSeRepository;
        private readonly IMapper _mapper;
        private IValidator<TeamForCreationDto> _postValidator;
        private IValidator<TeamForUpdateDto> _updateValidator;


        public TeamController(IReWoSeRepository reWoSeRepository, IMapper mapper, IValidator<TeamForCreationDto> validator, IValidator<TeamForUpdateDto> updateValidator)
        {
            _reWoSeRepository = reWoSeRepository ?? throw new ArgumentNullException(nameof(reWoSeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamWithoutEmployeesDto>>> GetTeams()
        {
            var teamsFromRepo = await _reWoSeRepository.GetTeamsListAsync();
            return Ok(_mapper.Map<IEnumerable<TeamWithoutEmployeesDto>>(teamsFromRepo));
        }
        [HttpGet("{teamId}")]
        public async Task<IActionResult> GetTeam(Guid teamId)
        {
            var teamFromRepo = await _reWoSeRepository.GetTeamAsync(teamId);
            if (teamFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TeamWithoutEmployeesDto>(teamFromRepo));
        }
        [HttpGet("{teamId}/employees")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeamEmployees(Guid teamId)
        {
            var teamFromRepo = await _reWoSeRepository.GetTeamAsync(teamId);
            if (teamFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(teamFromRepo.Employees));
        }
        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam(TeamForCreationDto teamForCreation)
        {
            ValidationResult validationResult = _postValidator.Validate(teamForCreation);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (await _reWoSeRepository.TeamNameExistsAsync(teamForCreation.Name))
            {
                return BadRequest("Team name already exists.");
            }

            var teamEntity = _mapper.Map<Team>(teamForCreation);

            await _reWoSeRepository.AddTeamAsync(teamEntity);
            await _reWoSeRepository.SaveChangesAsync();

            var teamToReturn = _mapper.Map<TeamDto>(teamEntity);

            return CreatedAtAction(nameof(GetTeam), new { teamName = teamToReturn.Name }, teamToReturn);
        }
        [HttpPut("{teamId}")]
        public async Task<IActionResult> UpdateTeam(Guid teamID, TeamForUpdateDto teamForUpdate)
        {
            ValidationResult validationResult = _updateValidator.Validate(teamForUpdate);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (await _reWoSeRepository.TeamExistsAsync(teamForUpdate.Id))
            {
                return BadRequest("Team Id already exists.");
            }

            if (await _reWoSeRepository.TeamNameExistsAsync(teamForUpdate.Name))
            {
                return BadRequest("Team name already exists.");
            }

            var teamFromRepo = await _reWoSeRepository.GetTeamAsync(teamID);
            if (teamFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(teamForUpdate, teamFromRepo);

            await _reWoSeRepository.UpdateTeamAsync(teamFromRepo);
            await _reWoSeRepository.SaveChangesAsync();

            return NoContent();
        }
        [HttpPatch("{teamId}")]
        public async Task<IActionResult> PartiallyUpdateTeam(Guid teamID, [FromBody] JsonPatchDocument<TeamForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var teamFromRepo = await _reWoSeRepository.GetTeamAsync(teamID);
            if (teamFromRepo == null)
            {
                return NotFound();
            }

            var teamToPatch = _mapper.Map<TeamForUpdateDto>(teamFromRepo);
            patchDocument.ApplyTo(teamToPatch, ModelState);

            ValidationResult validationResult = _updateValidator.Validate(teamToPatch);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (!TryValidateModel(teamToPatch))
            {
                return ValidationProblem(ModelState);
            }

            if (await _reWoSeRepository.TeamExistsAsync(teamToPatch.Id))
            {
                return BadRequest("Team Id already exists.");
            }

            if (await _reWoSeRepository.TeamNameExistsAsync(teamToPatch.Name))
            {
                return BadRequest("Team name already exists.");
            }

            _mapper.Map(teamToPatch, teamFromRepo);

            await _reWoSeRepository.UpdateTeamAsync(teamFromRepo);
            var saveResult = await _reWoSeRepository.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(Guid teamId)
        {
            var teamFromRepo = await _reWoSeRepository.GetTeamAsync(teamId);
            if (teamFromRepo == null)
            {
                return NotFound();
            }

            _reWoSeRepository.DeleteTeam(teamFromRepo);
            await _reWoSeRepository.SaveChangesAsync();

            return NoContent();
        }
    }

}
