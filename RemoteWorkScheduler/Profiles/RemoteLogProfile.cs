using AutoMapper;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;

namespace RemoteWorkScheduler.Profiles
{
    public class RemoteLogProfile : Profile
    {
        public RemoteLogProfile()
        {
            CreateMap<RemoteLog, RemoteLogDto>();
            CreateMap<RemoteLogDto, RemoteLog>();
            CreateMap<RemoteLogForCreationDto, RemoteLog>();
            CreateMap<RemoteLog, RemoteLogForCreationDto>();
        }
    }
}
