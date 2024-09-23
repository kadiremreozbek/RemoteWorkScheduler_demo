using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;
using RemoteWorkScheduler.Services;

namespace RemoteWorkScheduler.AppService
{
    public class TeamAppService : ITeamAppService
    {
        private readonly IReWoSeRepository _reWoSeRepository;
        private readonly IMapper _mapper;
        private IValidator<TeamForCreationDto> _postValidator;
        private IValidator<TeamForUpdateDto> _updateValidator;

        public TeamAppService(IReWoSeRepository reWoSeRepository, IMapper mapper, IValidator<TeamForCreationDto> postValidator, IValidator<TeamForUpdateDto> updateValidator)
        {
            _reWoSeRepository = reWoSeRepository ?? throw new ArgumentNullException(nameof(reWoSeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postValidator = postValidator ?? throw new ArgumentNullException(nameof(postValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<TeamDto> AddTeamAS(TeamForCreationDto teamFC)
        {
            var teamEntity = _mapper.Map<Team>(teamFC);

            await _reWoSeRepository.AddTeamAsync(teamEntity);
            await _reWoSeRepository.SaveChangesAsync();
            var teamToReturn = _mapper.Map<TeamDto>(teamEntity);

            return teamToReturn;
        }
        public async Task<IEnumerable<TeamDto>> GetTeamsAS()
        {
            var teamsFromRepo = await _reWoSeRepository.GetTeamsListAsync();
            var teamsDto = _mapper.Map<IEnumerable<TeamDto>>(teamsFromRepo);
            return teamsDto;
        }
        public async Task<TeamDto> GetTeamAS(Guid teamId)
        {
            var teamFromRepo = await _reWoSeRepository.GetTeamAsync(teamId);
            var teamDto = _mapper.Map<TeamDto>(teamFromRepo);
            return teamDto;
        }
        public async Task<IEnumerable<EmployeeDto>> GetTeamEmployeesAS(Guid teamId)
        {
            var teamFromRepo = await _reWoSeRepository.GetTeamAsync(teamId);
            var sortedEmployees = teamFromRepo.Employees.OrderBy(e => e.Title).ThenBy(e => e.Name);
            var employeesList = _mapper.Map<IEnumerable<EmployeeDto>>(sortedEmployees);
            return employeesList;
        }
        public async Task<IActionResult> UpdateTeamAS(Guid teamId, TeamForUpdateDto teamFU)
        {
            var teamEntity = await _reWoSeRepository.GetTeamAsync(teamId);
            if (teamEntity == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(teamFU, teamEntity);
            await _reWoSeRepository.UpdateTeamAsync(teamEntity);
            await _reWoSeRepository.SaveChangesAsync();

            return new NoContentResult();
        }
        public async Task<IActionResult> DeleteTeamAS(Guid teamId)
        {
            var teamEntity = await _reWoSeRepository.GetTeamAsync(teamId);
            if (teamEntity == null)
            {
                return new NotFoundResult();
            }

            _reWoSeRepository.DeleteTeam(teamEntity);
            await _reWoSeRepository.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}

