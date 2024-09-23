using Microsoft.AspNetCore.Mvc;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;

namespace RemoteWorkScheduler.AppService
{
    public interface IEmployeeAppService
    {
        Task<EmployeeDto> AddEmployeeAS(EmployeeForCreationDto employeeFC);
        Task<IEnumerable<EmployeeDto>> GetEmployeesAS();
        Task<EmployeeDto> GetEmployeeAS(Guid employeeId);
        Task<IActionResult> UpdateEmployeeAS(Guid employeeId, EmployeeForUpdateDto employeeFU);
        Task<IActionResult> DeleteEmployeeAS(Guid employeeId);
    }
}
