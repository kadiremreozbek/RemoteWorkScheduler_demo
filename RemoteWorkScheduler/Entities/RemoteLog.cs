using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RemoteWorkScheduler.Entities
{
    public class RemoteLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        public Guid EmployeeId { get; set; }

        // Parameterless constructor for EF Core
        public RemoteLog() { }

        // Constructor for your own use
        public RemoteLog(DateTime date)
        {
            Date = date;
        }
    }
}
