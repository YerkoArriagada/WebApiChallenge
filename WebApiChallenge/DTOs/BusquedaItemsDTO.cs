using System.ComponentModel.DataAnnotations;

namespace WebApiChallenge.DTOs
{
    public class BusquedaItemsDTO
    {
        [Required]
        public string url { get; set; }
    }
}
