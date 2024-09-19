using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;
using RemoteWorkScheduler.Services;

namespace RemoteWorkScheduler.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IReWoSeRepository _reWoSeRepository;
        private readonly IMapper _mapper;
        public EmployeeController(IReWoSeRepository reWoSeRepository, IMapper mapper)
        {
            _reWoSeRepository = reWoSeRepository ?? throw new ArgumentNullException(nameof(reWoSeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employeesFromRepo = await _reWoSeRepository.GetEmployeesListAsync();
            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employeesFromRepo));
        }
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployee(Guid employeeId)
        {
            var employeeFromRepo = await _reWoSeRepository.GetEmployeeAsync(employeeId);
            if (employeeFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<EmployeeDto>(employeeFromRepo));
        }
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeForCreationDto employeeForCreation)
        {
            var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

            await _reWoSeRepository.AddEmployeeAsync(employeeEntity);
            await _reWoSeRepository.SaveChangesAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return CreatedAtAction(nameof(GetEmployee), new { employeeId = employeeToReturn.Id }, employeeToReturn);
        }
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(Guid employeeId, EmployeeForUpdateDto employeeForUpdate)
        {
            if (await _reWoSeRepository.EmployeeExistsAsync(employeeForUpdate.Id))
            {
                return BadRequest("Employee already exists.");
            }

            var employeeFromRepo = await _reWoSeRepository.GetEmployeeAsync(employeeId);
            if (employeeFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(employeeForUpdate, employeeFromRepo);

            await _reWoSeRepository.UpdateEmployeeAsync(employeeFromRepo);
            await _reWoSeRepository.SaveChangesAsync();

            return NoContent();
        }
        [HttpPatch("{employeeId}")]
        public async Task<IActionResult> PartiallyUpdateEmployee(Guid employeeId, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var employeeFromRepo = await _reWoSeRepository.GetEmployeeAsync(employeeId);
            if (employeeFromRepo == null)
            {
                return NotFound();
            }

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeFromRepo);
            patchDocument.ApplyTo(employeeToPatch, ModelState);

            if (!TryValidateModel(employeeToPatch))
            {
                return ValidationProblem(ModelState);
            }

            if (await _reWoSeRepository.EmployeeExistsAsync(employeeToPatch.Id))
            {
                return BadRequest("Employee already exists.");
            }

            _mapper.Map(employeeToPatch, employeeFromRepo);

            await _reWoSeRepository.UpdateEmployeeAsync(employeeFromRepo);
            var saveResult = await _reWoSeRepository.SaveChangesAsync();

            if (!saveResult)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(Guid employeeId)
        {
            var employeeFromRepo = await _reWoSeRepository.GetEmployeeAsync(employeeId);
            if (employeeFromRepo == null)
            {
                return NotFound();
            }

            _reWoSeRepository.DeleteEmployee(employeeFromRepo);
            await _reWoSeRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
