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
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IReWoSeRepository _reWoSeRepository;
        private readonly IEmployeeAppService _employeeAppService;
        private readonly IMapper _mapper;
        private IValidator<EmployeeForCreationDto> _postValidator;
        private IValidator<EmployeeForUpdateDto> _updateValidator;
        private Employee employeeToReturn;

        public EmployeeController(IEmployeeAppService _emploeeAppService, IReWoSeRepository reWoSeRepository, IMapper mapper, IValidator<EmployeeForCreationDto> postValidator, IValidator<EmployeeForUpdateDto> updateValidator)
        {
            _employeeAppService = _emploeeAppService ?? throw new ArgumentNullException(nameof(_emploeeAppService));
            _reWoSeRepository = reWoSeRepository ?? throw new ArgumentNullException(nameof(reWoSeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postValidator = postValidator ?? throw new ArgumentNullException(nameof(postValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employeesList = await _employeeAppService.GetEmployeesAS();
            return Ok(employeesList);
        }
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployee(Guid employeeId)
        {
            var employeeFromRepo = await _employeeAppService.GetEmployeeAS(employeeId);
            if (employeeFromRepo == null)
            {
                return NotFound();
            }

            return Ok(employeeFromRepo);
        }
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeForCreationDto employeeForCreation)
        {
            ValidationResult validationResult = await _postValidator.ValidateAsync(employeeForCreation);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var employeeToReturn = await _employeeAppService.AddEmployeeAS(employeeForCreation);

            return CreatedAtAction(nameof(GetEmployee), new { employeeId = employeeToReturn.Id }, employeeToReturn);
        }
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(Guid employeeId, EmployeeForUpdateDto employeeForUpdate)
        {
            ValidationResult validationResult = await _updateValidator.ValidateAsync(employeeForUpdate);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (employeeId != employeeForUpdate.Id)
            {
                return BadRequest("Employee Id is not same.");
            }

            await _employeeAppService.UpdateEmployeeAS(employeeId, employeeForUpdate);

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

            ValidationResult validationResult = await _updateValidator.ValidateAsync(employeeToPatch);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (!TryValidateModel(employeeToPatch))
            {
                return ValidationProblem(ModelState);
            }

            if (employeeId != employeeToPatch.Id)
            {
                return BadRequest("Employee Id is not same.");
            }

            await _employeeAppService.UpdateEmployeeAS(employeeId, employeeToPatch);


            return NoContent();
        }
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(Guid employeeId)
        {
            await _employeeAppService.DeleteEmployeeAS(employeeId);

            return NoContent();
        }
    }
}
