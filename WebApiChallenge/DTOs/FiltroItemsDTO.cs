using System.ComponentModel.DataAnnotations;

namespace WebApiChallenge.DTOs
{
    public class FiltroItemsDTO
    {
        [Required]
        public List<string> items_ids { get; set; }
        [Required]
        public double amount { get; set; }

    }
}
