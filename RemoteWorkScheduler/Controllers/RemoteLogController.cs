using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;
        public RemoteLogController(IReWoSeRepository reWoSeRepository, IMapper mapper)
        {
            _reWoSeRepository = reWoSeRepository ?? throw new ArgumentNullException(nameof(reWoSeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RemoteLogDto>>> GetRemoteLogs()
        {
            var remoteLogsFromRepo = await _reWoSeRepository.GetRemoteLogsListAsync();
            return Ok(_mapper.Map<IEnumerable<RemoteLogDto>>(remoteLogsFromRepo));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRemoteLog(Guid id)
        {
            var remoteLogFromRepo = await _reWoSeRepository.GetRemoteLogAsync(id);
            if (remoteLogFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RemoteLogDto>(remoteLogFromRepo));
        }
        [HttpGet("bydate/{date}")]
        public async Task<IActionResult> GetRemoteLogsByDate(DateTime date)
        {
            var remoteLogsFromRepo = await _reWoSeRepository.GetRemoteLogsByDateAsync(date);
            return Ok(_mapper.Map<IEnumerable<RemoteLogDto>>(remoteLogsFromRepo)); // Corrected line
        }
        [HttpGet("byemployee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<RemoteLogDto>>> GetRemoteLogsByEmployeeId(Guid employeeId)
        {
            var remoteLogsFromRepo = await _reWoSeRepository.GetRemoteLogsByEmployeeIdAsync(employeeId);
            return Ok(_mapper.Map<IEnumerable<RemoteLogDto>>(remoteLogsFromRepo));
        }
        [HttpPost]
        public async Task<ActionResult<RemoteLogDto>> CreateRemoteLog(RemoteLogForCreationDto remoteLogForCreation)
        {
            if (await _reWoSeRepository.LogExistsAsync(remoteLogForCreation.Date, remoteLogForCreation.EmployeeId))
            {
                return BadRequest("Remote log already exists.");
            }

            var remoteLogEntity = _mapper.Map<RemoteLog>(remoteLogForCreation);

            if (!await _reWoSeRepository.LogEligibleToPost(remoteLogEntity))
            {
                return BadRequest("This log does not follow the rules.");
            }

            await _reWoSeRepository.AddRemoteLogAsync(remoteLogEntity);



            await _reWoSeRepository.SaveChangesAsync();

            var remoteLogToReturn = _mapper.Map<RemoteLogDto>(remoteLogEntity);

            return CreatedAtAction(nameof(GetRemoteLog) ,new { id = remoteLogToReturn.Id }, remoteLogToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRemoteLog(Guid id, RemoteLogForCreationDto remoteLogForUpdate)
        {
            if (await _reWoSeRepository.LogExistsAsync(remoteLogForUpdate.Date, remoteLogForUpdate.EmployeeId))
            {
                return BadRequest("Remote log already exists.");
            }


            var remoteLogFromRepo = await _reWoSeRepository.GetRemoteLogAsync(id);
            if (remoteLogFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(remoteLogForUpdate, remoteLogFromRepo);

            if (!await _reWoSeRepository.LogEligibleUpdate(remoteLogFromRepo))
            {
                return BadRequest("This log does not follow the rules.");
            }

            await _reWoSeRepository.UpdateRemoteLogAsync(remoteLogFromRepo);
            await _reWoSeRepository.SaveChangesAsync();

            return NoContent();
        }
        [HttpPatch("{id}")]
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

            if (!TryValidateModel(remoteLogToPatch))
            {
                return ValidationProblem(ModelState);
            }

            if (await _reWoSeRepository.LogExistsAsync(remoteLogToPatch.Date, remoteLogToPatch.EmployeeId))
            {
                return BadRequest("Remote log already exists.");
            }

            _mapper.Map(remoteLogToPatch, remoteLogFromRepo);

            if (!await _reWoSeRepository.LogEligibleUpdate(remoteLogFromRepo))
            {
                return BadRequest("This log does not follow the rules.");
            }

            await _reWoSeRepository.UpdateRemoteLogAsync(remoteLogFromRepo);
            var saveResult = await _reWoSeRepository.SaveChangesAsync();

            if (!saveResult)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRemoteLog(Guid id)
        {
            var remoteLogFromRepo = await _reWoSeRepository.GetRemoteLogAsync(id);
            if (remoteLogFromRepo == null)
            {
                return NotFound();
            }

            _reWoSeRepository.DeleteRemoteLog(remoteLogFromRepo);
            await _reWoSeRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
