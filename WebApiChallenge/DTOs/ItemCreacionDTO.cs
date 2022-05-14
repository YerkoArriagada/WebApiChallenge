using System.ComponentModel.DataAnnotations;

namespace WebApiChallenge.DTOs
{
    public class ItemCreacionDTO
    {
        [Required]
        [StringLength(50)]
        public string id_item { get; set; }
        [Required]
        public double price { get; set; }
    }
}
