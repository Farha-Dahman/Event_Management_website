using System.ComponentModel.DataAnnotations;

namespace EventManagement_Application.Models
{
    public class AddNewRole
    {
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}
