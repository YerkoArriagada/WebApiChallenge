using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiChallenge.DTOs;
using WebApiChallenge.Entidades;
using WebApiChallenge.Helpers;

namespace WebApiChallenge.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public UsuariosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        #region Peticiones HTTP Usuarios 
        [HttpGet]
        public async Task<ActionResult<List<UsuarioDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Usuarios
                .AsQueryable()
                .Include(itemDB => itemDB.usuariosItems)
                .ThenInclude(usuarioItemDB => usuarioItemDB.item);

            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistrosPorPagina);
            var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
            var dtos = mapper.Map<List<UsuarioDTO>>(entidades);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerUsuario")]
        public async Task<ActionResult<UsuarioDTO>> Get(int id)
        {
            var usuario = await context.Usuarios
                .Include(itemDB => itemDB.usuariosItems)
                .ThenInclude(usuarioItemDB => usuarioItemDB.item)
                .FirstOrDefaultAsync(x => x.id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            usuario.usuariosItems = usuario.usuariosItems.OrderBy(x => x.orden).ToList();

            var dto = mapper.Map<UsuarioDTO>(usuario);

            return dto;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<UsuarioDTO>> Get(string name)
        {
            var entidades = await context.Usuarios.FirstOrDefaultAsync(x => x.nombre.Contains(name));

            if (entidades == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<UsuarioDTO>(entidades);

            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UsuarioCreacionDTO usuarioCreacionDTO)
        {
            var entidad = mapper.Map<Usuario>(usuarioCreacionDTO);
            context.Add(entidad);
            await context.SaveChangesAsync();
            var usuarioDTO = mapper.Map<UsuarioDTO>(entidad);

            return new CreatedAtRouteResult("obtenerUsuario", new { id = usuarioDTO.id, usuarioDTO });
        }

        [HttpPost("saveuser")]
        public async Task<ActionResult> PostUser(UsuarioCreacionDTO usuarioCreacionDTO)
        {
            if (usuarioCreacionDTO.itemsIds == null)
            {
                return BadRequest("No se puede crear un usuario sin items");
            }

            var itemsIds = await context.Items
                .Where(itemDB => usuarioCreacionDTO.itemsIds
                .Contains(itemDB.id))
                .Select(x => x.id)
                .ToListAsync();

            if (usuarioCreacionDTO.itemsIds.Count != itemsIds.Count)
            {
                return BadRequest("Uno de los items enviados no existe");
            }

            var usuario = mapper.Map<Usuario>(usuarioCreacionDTO);

            if (usuario.usuariosItems != null)
            {
                for (int i = 0; i < usuario.usuariosItems.Count; i++)
                {
                    usuario.usuariosItems[i].orden = i;
                }
            }

            context.Add(usuario);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("saveusers")]
        public async Task<ActionResult> PostUsers([FromBody] List<UsuarioCreacionDTO> usuarioCreacionDTOs)
        {
            List<UsuarioCreacionDTO> ListUsuariosCreacionDTO = usuarioCreacionDTOs;
            foreach (var usuarioDTO in ListUsuariosCreacionDTO)
            {
                if (usuarioDTO.itemsIds == null)
                {
                    return BadRequest("No se puede crear un usuario sin items");
                }

                var itemsIds = await context.Items
                    .Where(itemDB => usuarioDTO.itemsIds
                    .Contains(itemDB.id))
                    .Select(x => x.id)
                    .ToListAsync();

                if (usuarioDTO.itemsIds.Count != itemsIds.Count)
                {
                    return BadRequest("Uno de los items enviados no existe");
                }

                var usuario = mapper.Map<Usuario>(usuarioDTO);

                if (usuario.usuariosItems != null)
                {
                    for (int i = 0; i < usuario.usuariosItems.Count; i++)
                    {
                        usuario.usuariosItems[i].orden = i;
                    }
                }

                context.Add(usuario);
            }
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] UsuarioCreacionDTO usuarioCreacionDTO)
        {
            var entidad = mapper.Map<Usuario>(usuarioCreacionDTO);
            entidad.id = id;
            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Usuarios.AnyAsync(x => x.id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Usuario() { id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
        #endregion
    }
}
