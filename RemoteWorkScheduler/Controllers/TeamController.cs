using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RemoteWorkScheduler.AppService;
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
        private readonly ITeamAppService _teamAppService;
        private readonly IMapper _mapper;
        private IValidator<TeamForCreationDto> _postValidator;
        private IValidator<TeamForUpdateDto> _updateValidator;


        public TeamController(ITeamAppService teamAppService, IReWoSeRepository reWoSeRepository, IMapper mapper, IValidator<TeamForCreationDto> validator, IValidator<TeamForUpdateDto> updateValidator)
        {
            _teamAppService = teamAppService ?? throw new ArgumentNullException(nameof(teamAppService));
            _reWoSeRepository = reWoSeRepository ?? throw new ArgumentNullException(nameof(reWoSeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamWithoutEmployeesDto>>> GetTeams()
        {
            var teamsList = await _teamAppService.GetTeamsAS();

            return Ok(teamsList);
        }
        [HttpGet("{teamId}")]
        public async Task<IActionResult> GetTeam(Guid teamId)
        {
            var teamDto = await _teamAppService.GetTeamAS(teamId);

            if (teamDto == null)
            {
                return NotFound();
            }

            return Ok(teamDto);
        }
        [HttpGet("{teamId}/employees")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeamEmployees(Guid teamId)
        {
            var employeesList = await _teamAppService.GetTeamEmployeesAS(teamId);

            return Ok(employeesList);
        }
        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam(TeamForCreationDto teamForCreation)
        {
            ValidationResult validationResult = _postValidator.Validate(teamForCreation);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var teamToReturn = await _teamAppService.AddTeamAS(teamForCreation);

            return CreatedAtAction(nameof(GetTeam), new { teamId = teamToReturn.Id }, teamToReturn);
        }
        [HttpPut("{teamId}")]
        public async Task<IActionResult> UpdateTeam(Guid teamId, TeamForUpdateDto teamForUpdate)
        {
            ValidationResult validationResult = _updateValidator.Validate(teamForUpdate);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (teamId != teamForUpdate.Id)
            {
                return BadRequest("Team Id does not match.");
            }

            await _teamAppService.UpdateTeamAS(teamId, teamForUpdate);

            return NoContent();
        }
        [HttpPatch("{teamId}")]
        public async Task<IActionResult> PartiallyUpdateTeam(Guid teamId, [FromBody] JsonPatchDocument<TeamForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var teamFromRepo = await _reWoSeRepository.GetTeamAsync(teamId);
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

            if (teamId != teamToPatch.Id)
            {
                return BadRequest("Team Id does not match.");
            }

            await _teamAppService.UpdateTeamAS(teamId, teamToPatch);

            return NoContent();
        }
        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(Guid teamId)
        {
            await _teamAppService.DeleteTeamAS(teamId);

            return NoContent();
        }
    }

}
