using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace NuxibaAPI.DTOs
{
    public class UpdateLoginDto
    {
        [Required]
        [Range(0,1)]
        public int TipoMov { get ; set; }
    }
}
