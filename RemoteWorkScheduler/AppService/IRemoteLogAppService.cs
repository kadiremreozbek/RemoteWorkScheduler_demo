using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteWorkScheduler.AppService
{
    public interface IRemoteLogAppService
    {
        Task<IEnumerable<RemoteLogDto>> GetRemoteLogsAS();
        Task<RemoteLogDto> GetRemoteLogAS(Guid id);
        Task<IEnumerable<RemoteLogDto>> GetRemoteLogsByDateAS(DateTime date);
        Task<IEnumerable<RemoteLogDto>> GetRemoteLogsByEmployeeIdAS(Guid employeeId);
        Task<RemoteLogDto> CreateRemoteLogAS(RemoteLogForCreationDto remoteLogForCreation);
        Task<bool> LogEligibleToPostAS(RemoteLogForCreationDto remoteLog);
        Task<bool> LogEligibleUpdateAS(RemoteLog remoteLog);
        Task UpdateRemoteLogAS(RemoteLog remoteLogForUpdate);
        Task DeleteRemoteLogAS(Guid id);
    }
}
