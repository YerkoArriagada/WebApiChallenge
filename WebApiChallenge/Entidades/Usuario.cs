namespace WebApiChallenge.Entidades
{
    public class Usuario
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public List<UsuarioItem> usuariosItems { get; set; }
    }
}
