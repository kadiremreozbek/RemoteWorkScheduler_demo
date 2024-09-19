using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.Services
{
    public interface IReWoSeRepository
    {
        Task AddTeamAsync(Team team);
        Task<IEnumerable<Team>> GetTeamsListAsync();
        Task<Team> GetTeamAsync(Guid teamId);
        Task UpdateTeamAsync(Team team);
        Task<bool> TeamExistsAsync(Guid teamId);
        Task<bool> TeamNameExistsAsync(string name);
        void DeleteTeam(Team team);
        Task AddEmployeeAsync(Employee employee);
        Task<IEnumerable<Employee>> GetEmployeesListAsync();
        Task<Employee> GetEmployeeAsync(Guid employeeId);
        Task UpdateEmployeeAsync(Employee employee);
        Task<bool> EmployeeExistsAsync(Guid employeeId);
        void DeleteEmployee(Employee employee);
        Task AddRemoteLogAsync(RemoteLog remoteLog);
        Task<IEnumerable<RemoteLog>> GetRemoteLogsListAsync();
        Task UpdateRemoteLogAsync(RemoteLog remoteLog);
        Task<RemoteLog> GetRemoteLogAsync(Guid id);
        Task<IEnumerable<RemoteLog>> GetRemoteLogsByDateAsync(DateTime date);
        Task<IEnumerable<RemoteLog>> GetRemoteLogsByEmployeeIdAsync(Guid employeeId);
        Task<bool> LogExistsAsync(DateTime date, Guid employeeId);
        Task<bool> LogEligibleToPost(RemoteLog remoteLog);
        Task<bool> LogEligibleUpdate(RemoteLog remoteLog);
        void DeleteRemoteLog(RemoteLog remoteLog);
        Task<bool> SaveChangesAsync();
    }
}
