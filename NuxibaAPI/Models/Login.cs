using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaAPI.Models
{
    [Table("ccloglogin")]
    public class Login
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public int Extension { get; set; }
        [Required]
        [Range(0,1)]
        public int TipoMov { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
    }
}
