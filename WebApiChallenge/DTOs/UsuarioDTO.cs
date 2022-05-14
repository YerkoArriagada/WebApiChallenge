namespace WebApiChallenge.DTOs
{
    public class UsuarioDTO : UsuarioCreacionDTO
    {
        public int id { get; set; }
        public List<ItemDTO> items { get; set; }
    }
}
