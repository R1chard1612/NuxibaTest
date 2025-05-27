using System.ComponentModel.DataAnnotations;

namespace NuxibaAPI.DTOs
{
    public class CreateLoginDto
    {
        [Required]
        public int User_id { get; set; }
        [Required]
        public int Extension {  get; set; }
        [Required]
        [Range(0, 1)]
        public int TipoMov { get; set; }
    }
}
