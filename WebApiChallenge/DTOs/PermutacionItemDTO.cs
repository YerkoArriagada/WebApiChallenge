using System.ComponentModel.DataAnnotations;

namespace WebApiChallenge.DTOs
{
    public class PermutacionItemDTO
    {
        [Required]
        public List<string> items_ids { get; set; }
        [Required]
        public double total { get; set; }
    }
}
