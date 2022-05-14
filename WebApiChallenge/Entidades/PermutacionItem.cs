using System.ComponentModel.DataAnnotations;

namespace WebApiChallenge.Entidades
{
    public class PermutacionItem
    {
        [Required]
        public List<string> items_ids { get; set; }
        [Required]
        public double total { get; set; }
    }
}
