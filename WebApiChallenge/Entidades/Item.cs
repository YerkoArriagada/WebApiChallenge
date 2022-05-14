using System.ComponentModel.DataAnnotations;

namespace WebApiChallenge.Entidades
{
    public class Item
    {
        public int id { get; set; }
        [Required]
        [StringLength(50)]
        public string id_item { get; set; }
        [Required]
        public double price { get; set; }
        public List<UsuarioItem> usuariosItems { get; set; }
    }
}
