using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.Models
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public JobTitle Title { get; set; }
        public Guid TeamId { get; set; }
    }
}
