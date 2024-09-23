using Microsoft.AspNetCore.Mvc;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;

namespace RemoteWorkScheduler.AppService
{
    public interface ITeamAppService
    {
        Task<TeamDto> AddTeamAS(TeamForCreationDto teamFC);
        Task<IEnumerable<TeamDto>> GetTeamsAS();
        Task<TeamDto> GetTeamAS(Guid teamId);
        Task<IEnumerable<EmployeeDto>> GetTeamEmployeesAS(Guid teamId);
        Task<IActionResult> UpdateTeamAS(Guid teamId, TeamForUpdateDto teamFU);
        Task<IActionResult> DeleteTeamAS(Guid teamId);
    }
}

