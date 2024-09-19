using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.Models
{
    public class EmployeeForCreationDto
    {
        public string Name { get; set; }
        public JobTitle Title { get; set; }
        public Guid TeamId { get; set; }
    }
}
