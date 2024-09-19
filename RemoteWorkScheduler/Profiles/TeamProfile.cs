using AutoMapper;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;

namespace RemoteWorkScheduler.Profiles
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<Team, TeamDto>();
            CreateMap<TeamDto, Team>();
            CreateMap<TeamForCreationDto, Team>();
            CreateMap<Team, TeamWithoutEmployeesDto>();
            CreateMap<Team, TeamForCreationDto>();
            CreateMap<TeamForUpdateDto, Team>();
            CreateMap<Team, TeamForUpdateDto>();
        }
    }
}
