using Microsoft.EntityFrameworkCore;
using RemoteWorkScheduler.DbContexts;
using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.Services
{
    public class ReWoSeRepository : IReWoSeRepository
    {
        private readonly RemoteWorkSchedulerContext _context;
        public ReWoSeRepository(RemoteWorkSchedulerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddTeamAsync(Team team)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            await _context.Teams.AddAsync(team);
        }
        public async Task<IEnumerable<Team>> GetTeamsListAsync()
        {
            return await _context.Teams.OrderBy(c => c.Name).ToListAsync();
        }
        public async Task<Team> GetTeamAsync(Guid teamId)
        {
            return await _context.Teams.Include(t => t.Employees).FirstOrDefaultAsync(c => c.Id == teamId);
        }
        public async Task UpdateTeamAsync(Team team)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            _context.Entry(team).State = EntityState.Modified;
            await Task.CompletedTask;
        }
        public async Task<bool> TeamExistsAsync(Guid teamId)
        {
            return await _context.Teams.AnyAsync(c => c.Id == teamId);
        }
        public async Task<bool> TeamNameExistsAsync(string name)
        {
            return await _context.Teams.AnyAsync(c => c.Name == name);
        }
        public void DeleteTeam(Team team)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            _context.Teams.Remove(team);
        }
        public async Task AddEmployeeAsync(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }
            await _context.Employees.AddAsync(employee);
        }
        public async Task<IEnumerable<Employee>> GetEmployeesListAsync()
        {
            return await _context.Employees.OrderBy(c => c.TeamId).ThenBy(c => c.Title).ThenBy(c => c.Name).ToListAsync();
        }
        public async Task<Employee> GetEmployeeAsync(Guid employeeId)
        {
            return await _context.Employees.FirstOrDefaultAsync(c => c.Id == employeeId);
        }
        public async Task UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }
            await Task.Run(() => _context.Entry(employee).State = EntityState.Modified);
        }
        public async Task<bool> EmployeeExistsAsync(Guid employeeId)
        {
            return await _context.Employees.AnyAsync(c => c.Id == employeeId);
        }
        public void DeleteEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }
            _context.Employees.Remove(employee);
        }
        public async Task AddRemoteLogAsync(RemoteLog remoteLog)
        {
            if (remoteLog == null)
            {
                throw new ArgumentNullException(nameof(remoteLog));
            }
            await _context.RemoteLogs.AddAsync(remoteLog);
        }
        public async Task<IEnumerable<RemoteLog>> GetRemoteLogsListAsync()
        {
            return await _context.RemoteLogs.OrderBy(c => c.Date).ToListAsync();
        }
        public async Task<RemoteLog> GetRemoteLogAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await _context.RemoteLogs.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task UpdateRemoteLogAsync(RemoteLog remoteLog)
        {
            if (remoteLog == null)
            {
                throw new ArgumentNullException(nameof(remoteLog));
            }
            await Task.Run(() => _context.Entry(remoteLog).State = EntityState.Modified);
        }
        public async Task<IEnumerable<RemoteLog>> GetRemoteLogsByDateAsync(DateTime date)
        {
            return await _context.RemoteLogs.Where(c => c.Date == date).ToListAsync();
        }
        public async Task<IEnumerable<RemoteLog>> GetRemoteLogsByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.RemoteLogs.Where(c => c.EmployeeId == employeeId).ToListAsync();
        }
        public async Task<bool> LogExistsAsync(DateTime date, Guid employeeId)
        {
            return await _context.RemoteLogs.AnyAsync(c => c.Date == date && c.EmployeeId == employeeId);
        }
        public async Task<bool> LogEligibleToPost(RemoteLog remoteLog)
        {
            if (remoteLog == null)
            {
                throw new ArgumentNullException(nameof(remoteLog));
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(c => c.Id == remoteLog.EmployeeId);

            DateTime startOfWeekdays = remoteLog.Date.AddDays(-(((int)remoteLog.Date.DayOfWeek + 5) % 7));
            DateTime endOfWeekdays = startOfWeekdays.AddDays(4).AddTicks(-1);

            int emploeeLogCountForWeek = await _context.RemoteLogs.Where(c => c.EmployeeId == remoteLog.EmployeeId && c.Date >= startOfWeekdays && c.Date <= endOfWeekdays).CountAsync();
            if (emploeeLogCountForWeek >= 2)
            {
                return false;
            }

            if (await _context.RemoteLogs.Where(c => c.EmployeeId == remoteLog.EmployeeId && c.Date >= remoteLog.Date.AddDays(-1) && c.Date <= remoteLog.Date.AddDays(1)).CountAsync() > 0)
            {
                // If there is a log for the same employee for the day before or day after
                return false;
            }

            int logCountTeamTitleThatDay = await _context.RemoteLogs.Where(c => c.Employee.Title == employee.Title && c.Employee.TeamId == employee.TeamId && c.Date == remoteLog.Date).CountAsync();
            int countTeamTitle = await _context.Employees.Where(c => c.Title == employee.Title && c.TeamId == employee.TeamId).CountAsync();

            if (logCountTeamTitleThatDay > Math.Ceiling(countTeamTitle / 2.0))
            {
                return false;
            }

            return true;
        }
        public async Task<bool> LogEligibleUpdate(RemoteLog remoteLog)
        {
            if (remoteLog == null)
            {
                throw new ArgumentNullException(nameof(remoteLog));
            }

            var dumpRemoteLog = await _context.RemoteLogs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == remoteLog.Id);

            var employee = await _context.Employees.FirstOrDefaultAsync(c => c.Id == remoteLog.EmployeeId);

            DateTime startOfWeekdays = remoteLog.Date.AddDays(-(((int)remoteLog.Date.DayOfWeek + 5) % 7));
            DateTime endOfWeekdays = startOfWeekdays.AddDays(4).AddTicks(-1);

            int emploeeLogCountForWeek = await _context.RemoteLogs.Where(c => c.EmployeeId == remoteLog.EmployeeId && c.Date >= startOfWeekdays && c.Date <= endOfWeekdays && c.Date != dumpRemoteLog.Date).CountAsync();
            if (emploeeLogCountForWeek >= 2)
            {
                return false;
            }

            if (await _context.RemoteLogs.Where(c => c.EmployeeId == remoteLog.EmployeeId && c.Date >= remoteLog.Date.AddDays(-1) && c.Date <= remoteLog.Date.AddDays(1) && c.Date != dumpRemoteLog.Date).CountAsync() > 0)
            {
                // If there is a log for the same employee for the day before or day after
                return false;
            }

            int logCountTeamTitleThatDay = await _context.RemoteLogs.Where(c => c.Employee.Title == employee.Title && c.Employee.TeamId == employee.TeamId && c.Date == dumpRemoteLog.Date).CountAsync();
            int countTeamTitle = await _context.Employees.Where(c => c.Title == employee.Title && c.TeamId == employee.TeamId).CountAsync();

            if (logCountTeamTitleThatDay > Math.Ceiling(countTeamTitle / 2.0))
            {
                return false;
            }

            return true;
        }
        public void DeleteRemoteLog(RemoteLog remoteLog)
        {
            if (remoteLog == null)
            {
                throw new ArgumentNullException(nameof(remoteLog));
            }
            _context.RemoteLogs.Remove(remoteLog);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
