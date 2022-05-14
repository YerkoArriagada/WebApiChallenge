namespace WebApiChallenge.Entidades
{
    public class UsuarioItem
    {
        public int itemId { get; set; }
        public int usuarioId { get; set; }
        public int orden { get; set; }
        public Item item { get; set; }
        public Usuario usuario { get; set; }
    }
}
