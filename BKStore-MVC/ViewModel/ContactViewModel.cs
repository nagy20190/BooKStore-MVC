using System.ComponentModel.DataAnnotations;

namespace BKStore_MVC.ViewModel
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }
    }
}
