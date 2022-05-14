using AutoMapper;
using WebApiChallenge.DTOs;
using WebApiChallenge.Entidades;

namespace WebApiChallenge.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Item, ItemDTO>().ReverseMap();
            CreateMap<ItemCreacionDTO, Item>();

            CreateMap<UsuarioCreacionDTO, Usuario>()
                .ForMember(usuario => usuario.usuariosItems, opciones => opciones.MapFrom(MapUsuariosItems));
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(usuarioDTO => usuarioDTO.items, opciones => opciones.MapFrom(MapUsuariosDTOItems));

            CreateMap<FiltroItems, FiltroItemsDTO>().ReverseMap();

            CreateMap<PermutacionItem, PermutacionItemDTO>().ReverseMap();

            CreateMap<BusquedaItems, BusquedaItemsDTO>().ReverseMap();

            CreateMap<Stats, StatsDTO>().ReverseMap();
        }

        private List<ItemDTO> MapUsuariosDTOItems(Usuario usuario, UsuarioDTO usuarioDTO)
        {
            var resultado = new List<ItemDTO>();

            if (usuario.usuariosItems == null) { return resultado; }

            foreach (var usuarioItem in usuario.usuariosItems)
            {
                resultado.Add(new ItemDTO()
                {
                    id = usuarioItem.item.id,
                    id_item = usuarioItem.item.id_item,
                    price = usuarioItem.item.price
                });
            }

            return resultado;
        }

        private List<UsuarioItem> MapUsuariosItems(UsuarioCreacionDTO usuarioCreacionDTO, Usuario usuario)
        {
            var resultado = new List<UsuarioItem>();

            if(usuarioCreacionDTO.itemsIds == null) { return resultado; }

            foreach (var itemId in usuarioCreacionDTO.itemsIds)
            {
                resultado.Add(new UsuarioItem() { itemId = itemId });
            }

            return resultado;
        }
    }
}
