using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;
using RemoteWorkScheduler.Services;

namespace RemoteWorkScheduler.AppService
{
    public class EmployeeAppService : IEmployeeAppService
    {
        private readonly IReWoSeRepository _reWoSeRepository;
        private readonly IMapper _mapper;
        private IValidator<EmployeeForCreationDto> _postValidator;
        private IValidator<EmployeeForUpdateDto> _updateValidator;

        public EmployeeAppService(IReWoSeRepository reWoSeRepository, IMapper mapper, IValidator<EmployeeForCreationDto> postValidator, IValidator<EmployeeForUpdateDto> updateValidator)
        {
            _reWoSeRepository = reWoSeRepository ?? throw new ArgumentNullException(nameof(reWoSeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postValidator = postValidator ?? throw new ArgumentNullException(nameof(postValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<EmployeeDto> AddEmployeeAS(EmployeeForCreationDto employeeFC)
        {
            var employeeEntity = _mapper.Map<Employee>(employeeFC);

            await _reWoSeRepository.AddEmployeeAsync(employeeEntity);
            await _reWoSeRepository.SaveChangesAsync();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);


            return employeeToReturn;
        }
        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAS()
        {
            var employeesFromRepo = await _reWoSeRepository.GetEmployeesListAsync();
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromRepo);
            return employeesDto;
        }
        public async Task<EmployeeDto> GetEmployeeAS(Guid employeeId)
        {
            var employeeFromRepo = await _reWoSeRepository.GetEmployeeAsync(employeeId);
            var employeeDto = _mapper.Map<EmployeeDto>(employeeFromRepo);
            return employeeDto;
        }
        public async Task<IActionResult> UpdateEmployeeAS(Guid employeeId, EmployeeForUpdateDto employeeFU)
        {
            var employeeEntity = await _reWoSeRepository.GetEmployeeAsync(employeeId);
            if (employeeEntity == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(employeeFU, employeeEntity);
            await _reWoSeRepository.UpdateEmployeeAsync(employeeEntity);
            await _reWoSeRepository.SaveChangesAsync();

            return new NoContentResult();
        } 
        public async Task<IActionResult> DeleteEmployeeAS(Guid employeeId)
        {
            var employeeEntity = await _reWoSeRepository.GetEmployeeAsync(employeeId);
            if (employeeEntity == null)
            {
                return new NotFoundResult();
            }

            _reWoSeRepository.DeleteEmployee(employeeEntity);
            await _reWoSeRepository.SaveChangesAsync();

            return new NoContentResult();
        }

    }
}
