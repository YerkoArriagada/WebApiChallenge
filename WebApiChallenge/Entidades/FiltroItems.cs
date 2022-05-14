using System.ComponentModel.DataAnnotations;

namespace WebApiChallenge.Entidades
{
    public class FiltroItems
    {
        [Required]
        public List<string> items_ids { get; set; }
        [Required]
        public double amount { get; set; }
    }
}
