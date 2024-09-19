using AutoMapper;
using RemoteWorkScheduler.Entities;
using RemoteWorkScheduler.Models;

namespace RemoteWorkScheduler.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeDto, Employee>();
            CreateMap<EmployeeForCreationDto, Employee>();
            CreateMap<Employee, EmployeeForCreationDto>();
            CreateMap<EmployeeForUpdateDto, Employee>();
            CreateMap<Employee, EmployeeForUpdateDto>();
        }
    }
}
