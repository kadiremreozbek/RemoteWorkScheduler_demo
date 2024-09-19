using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RemoteWorkScheduler.Entities
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();

        public Team() { }

        public Team(string name)
        {
            Name = name;
        }
    }
}
