using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.Models
{
    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }
}
