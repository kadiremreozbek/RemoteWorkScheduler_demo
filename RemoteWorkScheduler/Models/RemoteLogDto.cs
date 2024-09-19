using RemoteWorkScheduler.Entities;

namespace RemoteWorkScheduler.Models
{
    public class RemoteLogDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
