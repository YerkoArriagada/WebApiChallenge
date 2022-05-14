using System.ComponentModel.DataAnnotations;

namespace WebApiChallenge.DTOs
{
    public class UsuarioCreacionDTO
    {
        [Required]
        [StringLength(50)]
        public string nombre { get; set; }
        public List<int> itemsIds { get; set; }

    }
}
