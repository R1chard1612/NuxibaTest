using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaAPI.Models
{
    [Table("ccUsers")]
    public class User
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public string? Login {  get; set; }
        public string? Nombres { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Password { get; set; }
        public int TipoUser_id { get; set; }
        public int Status { get; set; }
        public DateTime fCreate { get; set; }
        public int IDArea { get; set; }
        public DateTime LastLoginAttempt { get; set; }
    }
}
