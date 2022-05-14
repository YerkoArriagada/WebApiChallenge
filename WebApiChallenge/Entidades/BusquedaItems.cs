using System.ComponentModel.DataAnnotations;

namespace WebApiChallenge.Entidades
{
    public class BusquedaItems
    {
        [Required]
        public string url { get; set; }
    }
}
