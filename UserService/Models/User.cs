using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;
using UserService.Enums;
using UserService.Models;

namespace UserService.Models
{
    public class User : EntityBaseDomain
    {
        [JsonIgnore]
        [Required]
        public string Username { get; set; } = string.Empty;

        [JsonIgnore]
        [Required]
        public string HashPassword { get; set; } = string.Empty;
    }

}