using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;
using RemoteWorkScheduler.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteWorkScheduler.AppService
{
    public class RemoteLogAppService : IRemoteLogAppService
    {
        private readonly IReWoSeRepository _reWoSeRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<RemoteLogForCreationDto> _postValidator;

        public RemoteLogAppService(IReWoSeRepository reWoSeRepository, IMapper mapper, IValidator<RemoteLogForCreationDto> postValidator)
        {
            _reWoSeRepository = reWoSeRepository ?? throw new ArgumentNullException(nameof(reWoSeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postValidator = postValidator ?? throw new ArgumentNullException(nameof(postValidator));
        }

        public async Task<IEnumerable<RemoteLogDto>> GetRemoteLogsAS()
        {
            var remoteLogsFromRepo = await _reWoSeRepository.GetRemoteLogsListAsync();
            return _mapper.Map<IEnumerable<RemoteLogDto>>(remoteLogsFromRepo);
        }
        public async Task<RemoteLogDto> GetRemoteLogAS(Guid id)
        {
            var remoteLogFromRepo = await _reWoSeRepository.GetRemoteLogAsync(id);
            if (remoteLogFromRepo == null)
            {
                return null;
            }

            return _mapper.Map<RemoteLogDto>(remoteLogFromRepo);
        }
        public async Task<IEnumerable<RemoteLogDto>> GetRemoteLogsByDateAS(DateTime date)
        {
            var remoteLogsFromRepo = await _reWoSeRepository.GetRemoteLogsByDateAsync(date);
            return _mapper.Map<IEnumerable<RemoteLogDto>>(remoteLogsFromRepo);
        }
        public async Task<IEnumerable<RemoteLogDto>> GetRemoteLogsByEmployeeIdAS(Guid employeeId)
        {
            var remoteLogsFromRepo = await _reWoSeRepository.GetRemoteLogsByEmployeeIdAsync(employeeId);
            return _mapper.Map<IEnumerable<RemoteLogDto>>(remoteLogsFromRepo);
        }
        public async Task<RemoteLogDto> CreateRemoteLogAS(RemoteLogForCreationDto remoteLogForCreation)
        {
            var remoteLogEntity = _mapper.Map<RemoteLog>(remoteLogForCreation);
            await _reWoSeRepository.AddRemoteLogAsync(remoteLogEntity);
            await _reWoSeRepository.SaveChangesAsync();

            return _mapper.Map<RemoteLogDto>(remoteLogEntity);
        }
        public async Task UpdateRemoteLogAS(RemoteLog remoteLogForUpdate)
        {
            var remoteLogEntity = await _reWoSeRepository.GetRemoteLogAsync(remoteLogForUpdate.Id);
            if (remoteLogEntity == null)
            {
                throw new KeyNotFoundException("Remote log not found.");
            }

            _mapper.Map(remoteLogForUpdate, remoteLogEntity);
            await _reWoSeRepository.UpdateRemoteLogAsync(remoteLogEntity);
            await _reWoSeRepository.SaveChangesAsync();
        }
        public async Task<bool> LogExistsAS(DateTime date, Guid employeeId)
        {
            var remoteLog = await _reWoSeRepository.LogExistsAsync(date, employeeId);
            if (remoteLog)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> LogEligibleToPostAS(RemoteLogForCreationDto remoteLogEntry)
        {
            var remoteLog = _mapper.Map<RemoteLog>(remoteLogEntry);

            if (remoteLog == null)
            {
                throw new ArgumentNullException(nameof(remoteLog));
            }

            var employee = await _reWoSeRepository.GetEmployeeAsync(remoteLog.EmployeeId);

            DateTime startOfWeekdays = remoteLog.Date.AddDays(-(((int)remoteLog.Date.DayOfWeek + 5) % 7));
            DateTime endOfWeekdays = startOfWeekdays.AddDays(4).AddTicks(-1);

            var emploeeLogsForWeek = await _reWoSeRepository.GetEmployeeLogCountForWeek(remoteLog.EmployeeId);
            int emploeeLogCountForWeek = emploeeLogsForWeek.Count();
            if (emploeeLogCountForWeek >= 2)
            {
                return false;
            }

            if (emploeeLogsForWeek.Count(c => c.EmployeeId == remoteLog.EmployeeId && c.Date >= remoteLog.Date.AddDays(-1) && c.Date <= remoteLog.Date.AddDays(1)) > 0)
            {
                // If there is a log for the same employee for the day before or day after
                return false;
            }

            var logsTeamTitleThatDay = await _reWoSeRepository.LogsTeamTitleThatDay(remoteLog.Date, employee.TeamId, employee.Title);
            int logCountTeamTitleThatDay = logsTeamTitleThatDay.Count();

            var employeesList = await _reWoSeRepository.GetEmployeesListAsync();
            int countTeamTitle = employeesList.Count(c => c.Title == employee.Title && c.TeamId == employee.TeamId);

            if (logCountTeamTitleThatDay > Math.Ceiling(countTeamTitle / 2.0))
            {
                return false;
            }

            return true;
        }
        public async Task<bool> LogEligibleUpdateAS(RemoteLog remoteLog)
        {
            var dumpRemoteLog = await _reWoSeRepository.GetRemoteLogAsync(remoteLog.Id);

            if (remoteLog == null)
            {
                throw new ArgumentNullException(nameof(remoteLog));
            }

            var employee = await _reWoSeRepository.GetEmployeeAsync(remoteLog.EmployeeId);

            DateTime startOfWeekdays = remoteLog.Date.AddDays(-(((int)remoteLog.Date.DayOfWeek + 5) % 7));
            DateTime endOfWeekdays = startOfWeekdays.AddDays(4).AddTicks(-1);

            var emploeeLogsForWeek = await _reWoSeRepository.GetEmployeeLogCountForWeek(remoteLog.EmployeeId);
            var emploeeLogsForWeekWithoutDump = emploeeLogsForWeek.Where(c => c.Date != dumpRemoteLog.Date);
            int emploeeLogCountForWeek = emploeeLogsForWeekWithoutDump.Count();
            if (emploeeLogCountForWeek >= 2)
            {
                return false;
            }

            if (emploeeLogsForWeek.Count(c => c.EmployeeId == remoteLog.EmployeeId && c.Date >= remoteLog.Date.AddDays(-1) && c.Date <= remoteLog.Date.AddDays(1) && c.Date != dumpRemoteLog.Date) > 0)
            {
                // If there is a log for the same employee for the day before or day after
                return false;
            }

            var logsTeamTitleThatDay = await _reWoSeRepository.LogsTeamTitleThatDay(remoteLog.Date, employee.TeamId, employee.Title);
            var logsTeamTitleThatDayWithoutDump = logsTeamTitleThatDay.Where(c => c.Id != dumpRemoteLog.Id);
            int logCountTeamTitleThatDay = logsTeamTitleThatDayWithoutDump.Count();

            var employeesList = await _reWoSeRepository.GetEmployeesListAsync();
            int countTeamTitle = employeesList.Count(c => c.Title == employee.Title && c.TeamId == employee.TeamId);

            if (logCountTeamTitleThatDay > Math.Ceiling(countTeamTitle / 2.0))
            {
                return false;
            }

            return true;
        }
        public async Task DeleteRemoteLogAS(Guid id)
        {
            var remoteLogEntity = await _reWoSeRepository.GetRemoteLogAsync(id);
            if (remoteLogEntity == null)
            {
                throw new KeyNotFoundException("Remote log not found.");
            }

            _reWoSeRepository.DeleteRemoteLog(remoteLogEntity);
            await _reWoSeRepository.SaveChangesAsync();
        }
    }
}
