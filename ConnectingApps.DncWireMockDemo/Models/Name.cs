using System.ComponentModel.DataAnnotations;

namespace ConnectingApps.DncWireMockDemo.Models
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
