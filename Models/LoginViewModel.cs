using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class LoginViewModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? ErrorMessage { get; set; }
    }
} 