using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RemoteWorkScheduler.AppService;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;
using RemoteWorkScheduler.Services;

namespace RemoteWorkScheduler.Controllers
{
    [Route("api/remotelogs")]
    [ApiController]
    public class RemoteLogController : ControllerBase
    {
        private readonly IReWoSeRepository _reWoSeRepository;
        private readonly IRemoteLogAppService _remoteLogAppService;
        private readonly IMapper _mapper;
        private IValidator<RemoteLogForCreationDto> _postValidator;

        public RemoteLogController(IRemoteLogAppService remoteLogAppService, IReWoSeRepository reWoSeRepository, IMapper mapper, IValidator<RemoteLogForCreationDto> validator)
        {
            _remoteLogAppService = remoteLogAppService ?? throw new ArgumentNullException(nameof(remoteLogAppService));
            _reWoSeRepository = reWoSeRepository ?? throw new ArgumentNullException(nameof(reWoSeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postValidator = validator ?? throw new ArgumentNullException(nameof(validator));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<RemoteLogDto>>> GetRemoteLogs()
        {
            var logs = await _remoteLogAppService.GetRemoteLogsAS();
            return Ok(logs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRemoteLog(Guid id)
        {
            var log = await _remoteLogAppService.GetRemoteLogAS(id);

            return Ok(log);
        }
        [HttpGet("bydate/{date}")]
        public async Task<IActionResult> GetRemoteLogsByDate(DateTime date)
        {
            var dateLogs = await _remoteLogAppService.GetRemoteLogsByDateAS(date);
            return Ok(dateLogs);
        }
        [HttpGet("byemployee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<RemoteLogDto>>> GetRemoteLogsByEmployeeId(Guid employeeId)
        {
            var remoteLogsFromRepo = await _remoteLogAppService.GetRemoteLogsByEmployeeIdAS(employeeId);
            return Ok(remoteLogsFromRepo);
        }
        [HttpPost]
        public async Task<ActionResult<RemoteLogDto>> CreateRemoteLog(RemoteLogForCreationDto remoteLogForCreation)
        {
            ValidationResult validationResult = _postValidator.Validate(remoteLogForCreation);

            if(!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (await _reWoSeRepository.LogExistsAsync(remoteLogForCreation.Date, remoteLogForCreation.EmployeeId))
            {
                return BadRequest("Remote log already exists.");
            }


            if (!await _remoteLogAppService.LogEligibleToPostAS(remoteLogForCreation))
            {
                return BadRequest("This log does not follow the rules.");
            }

            var remoteLogToReturn = await _remoteLogAppService.CreateRemoteLogAS(remoteLogForCreation);

            return CreatedAtAction(nameof(GetRemoteLog) ,new { id = remoteLogToReturn.Id }, remoteLogToReturn);
        }
        [HttpPut("{id}")]//test
        public async Task<IActionResult> UpdateRemoteLog(Guid id, RemoteLogForCreationDto remoteLogForUpdate)
        {
            ValidationResult validationResult = _postValidator.Validate(remoteLogForUpdate);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var remoteLogFromRepo = await _reWoSeRepository.GetRemoteLogAsync(id);
            if (remoteLogFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(remoteLogForUpdate, remoteLogFromRepo);

            if (!await _remoteLogAppService.LogEligibleUpdateAS(remoteLogFromRepo))
            {
                return BadRequest("This log does not follow the rules.");
            }

            await _remoteLogAppService.UpdateRemoteLogAS(remoteLogFromRepo);

            return NoContent();
        }
        [HttpPatch("{id}")]//test
        public async Task<IActionResult> PartiallyUpdateRemoteLog(Guid id, [FromBody] JsonPatchDocument<RemoteLogForCreationDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var remoteLogFromRepo = await _reWoSeRepository.GetRemoteLogAsync(id);
            if (remoteLogFromRepo == null)
            {
                return NotFound();
            }

            var remoteLogToPatch = _mapper.Map<RemoteLogForCreationDto>(remoteLogFromRepo);
            patchDocument.ApplyTo(remoteLogToPatch, ModelState);

            ValidationResult validationResult = _postValidator.Validate(remoteLogToPatch);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (!TryValidateModel(remoteLogToPatch))
            {
                return ValidationProblem(ModelState);
            }

            if (!await _remoteLogAppService.LogEligibleToPostAS(remoteLogToPatch))
            {
                return BadRequest("Remote log already exists.");
            }

            _mapper.Map(remoteLogToPatch, remoteLogFromRepo);

            if (!await _remoteLogAppService.LogEligibleUpdateAS(remoteLogFromRepo))
            {
                return BadRequest("This log does not follow the rules.");
            }

            await _remoteLogAppService.UpdateRemoteLogAS(remoteLogFromRepo);

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRemoteLog(Guid id)
        {
            await _remoteLogAppService.DeleteRemoteLogAS(id);

            return NoContent();
        }
    }
}
