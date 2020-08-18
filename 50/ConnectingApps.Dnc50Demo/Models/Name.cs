using System.ComponentModel.DataAnnotations;

namespace ConnectingApps.Dnc50Demo.Models
{
    public class Name
    {
        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
