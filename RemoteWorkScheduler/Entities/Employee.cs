using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RemoteWorkScheduler.Entities
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public JobTitle Title { get; set; }

        [Required]
        [ForeignKey("TeamId")]
        public Team Team { get; set; }
        public Guid TeamId { get; set; }

        // Parameterless constructor for EF Core
        public Employee() { }

        // Constructor for your own use
        public Employee(string name, JobTitle title, Team team)
        {
            Name = name;
            Title = title;
            Team = team;
            TeamId = team.Id;
        }
    }
}
