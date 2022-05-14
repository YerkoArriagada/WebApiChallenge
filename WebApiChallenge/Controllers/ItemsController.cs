using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using WebApiChallenge.DTOs;
using WebApiChallenge.Entidades;
using WebApiChallenge.Helpers;

namespace WebApiChallenge.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ItemsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        #region Peticiones HTTP Items
        [HttpGet]
        public async Task<ActionResult<List<ItemDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Items.AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistrosPorPagina);
            var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
            var dtos = mapper.Map<List<ItemDTO>>(entidades);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerItem")]
        public async Task<ActionResult<ItemDTO>> Get(int id)
        {
            var entidades = await context.Items.FirstOrDefaultAsync(x => x.id == id);

            if (entidades == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<ItemDTO>(entidades);

            return dto;
        }

        [HttpGet("{id_item}")]
        public async Task<ActionResult<ItemDTO>> Get(string id_item)
        {
            var entidades = await context.Items.FirstOrDefaultAsync(x => x.id_item.Contains(id_item));

            if (entidades == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<ItemDTO>(entidades);

            return dto;
        }

        [HttpGet("/stats")]
        public async Task<ActionResult<List<StatsDTO>>> GetStats()
        {

            var stats = await (from item in context.Items
                                join ui in context.UsuariosItems on item.id equals ui.itemId
                                select new { id = item.id, id_item = item.id_item, ui = ui })
                                .GroupBy(x => x.id_item).Select(group => new
                                {
                                    id = group.Key,
                                    quantity = group.Count()
                                }).OrderByDescending(x => x.quantity).Take(5).ToListAsync();

            if (stats == null)
            {
                return NotFound();
            }

            List<StatsDTO> ListStatsDTO = new List<StatsDTO>();

            foreach (var item in stats)
            {
                dynamic stat = item;

                ListStatsDTO.Add(new StatsDTO()
                {
                    id = stat.id,
                    quantity = stat.quantity,
                });
            }

            var dto = mapper.Map<List<StatsDTO>>(ListStatsDTO);

            return dto;
        }

        [HttpPost("saveitem")]
        public async Task<ActionResult> Post([FromBody] ItemCreacionDTO itemCreacionDTO)
        {
            var entidad = mapper.Map<Item>(itemCreacionDTO);
            context.Add(entidad);
            await context.SaveChangesAsync();
            var itemDTO = mapper.Map<ItemDTO>(entidad);

            return new CreatedAtRouteResult("obtenerItem", new { id = itemDTO.id, itemDTO });
        }

        [HttpPost("saveitems")]
        public async Task<ActionResult> Post([FromBody] BusquedaItemsDTO busquedaItemsDTO)
        {
            List<ItemCreacionDTO> ListItemsCreacionDTO = await Meli_SavesItems(busquedaItemsDTO);
            foreach (var itemDTO in ListItemsCreacionDTO)
            {
                var entidad = mapper.Map<Item>(itemDTO);
                context.Add(entidad);
            }
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("/coupon")]
        public ActionResult<PermutacionItemDTO> Post([FromBody] FiltroItemsDTO filtroItemsDTO)
        {
            var entidades = mapper.Map<FiltroItems>(filtroItemsDTO);

            List<string> ids = entidades.items_ids.ToList();
            var itemList = context.Items.Where(x => ids.Contains(x.id_item)).ToList();

            var permutItemDTO = Coupon_ListItemsTotalCoupon(itemList, filtroItemsDTO.amount);

            if (permutItemDTO == null)
            {
                return NotFound();
            }

            return permutItemDTO;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ItemCreacionDTO itemCreacionDTO)
        {
            var entidad = mapper.Map<Item>(itemCreacionDTO);
            entidad.id = id;
            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Items.AnyAsync(x => x.id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Item() { id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Funciones
        private static PermutacionItemDTO Coupon_ListItemsTotalCoupon(List<Item> itemList, double amount)
        {

            //Con Linq comprobamos si el precio de alguno de los productos es igual al total de cupon.
            if (itemList.Select(x => x.price).Contains(amount))
            {
                //De ser verdadero devolvemos un item de la lista de manera aleatoria.
                //(Esto en caso de que mas de un item tenga el mismo valor que el cupon)
                var permutacion = new PermutacionItemDTO()
                {
                    items_ids = (from p in itemList where p.price == amount select p.id_item).OrderBy(x => Guid.NewGuid()).ToList(),
                    total = amount
                };

                return permutacion;
            }
            else
            {
                //Con Linq comprobamos si la suma total de precios de los items es menor o igual al valor del cupon.
                //De ser verdadero retornamos el listado completo y ya.

                //Sumamos los precios de la lista de items
                var sumItemsPrice = itemList.Select(x => x.price).Aggregate((a, b) => a + b);

                //Comparamos si el valor de cupon es menor o igual que el total de precios de la lista de items
                if (sumItemsPrice <= amount)
                {
                    //retornamos el listado completo
                    var permutacion = new PermutacionItemDTO()
                    {
                        items_ids = itemList.Select(x => x.id_item).ToList(),
                        total = sumItemsPrice
                    };

                    return permutacion;
                } 
                else
                {
                    //Si ninguna de la anteriores verificaciones se cumple, procedemos a realizar las permutaciones y verificaciones.

                    //1.- Generamos todas las Permutaciones posibles.
                    List<IEnumerable<Item>> permutacion = new List<IEnumerable<Item>>();

                    for (int i = 2; i <= itemList.Count - 1; i++)
                    {
                        //Con la funcion Permutaciones generaremos las permutaciones segun cada nivel 2, 3, 4...n
                        //Concat nos permite agregar las secuencias genereadas.
                        permutacion = permutacion.Concat(itemList.Permutaciones(i)).ToList();
                    }

                    //2. Debemos generar la suma total de los precios de cada permutacion
                    //para encontrar el valor mas cercano por debajo del precio del cupon
                    return SumAllPemutations(permutacion).Where(x => x.total <= amount).OrderBy(z => z.total).LastOrDefault();
                }
            }
        }

        private static List<PermutacionItemDTO> SumAllPemutations(List<IEnumerable<Item>> objPermutaciones)
        {
            List<PermutacionItemDTO> ListFinal = new List<PermutacionItemDTO>();

            foreach (var objPermutacion in objPermutaciones)
            {
                List<ItemDTO> ListItemsDTO = new List<ItemDTO>();
                double SumItemsPricePermut = 0;

                foreach (var item in objPermutacion)
                {
                    ListItemsDTO.Add(new ItemDTO()
                    {
                        id = item.id,
                        id_item = item.id_item,
                        price = item.price,
                    });

                    SumItemsPricePermut += item.price;
                }

                ListFinal.Add(new PermutacionItemDTO()
                {
                    items_ids = ListItemsDTO.Select(x => x.id_item).ToList(),
                    total = SumItemsPricePermut,
                });
            }

            return ListFinal;
        }

        private static async Task<List<ItemCreacionDTO>> Meli_SavesItems(BusquedaItemsDTO busquedaItemsDTO)
        {
            var client = new RestClient(busquedaItemsDTO.url);
            var request = new RestRequest()
            {
                Method = Method.Get,
                RequestFormat = DataFormat.Json,
            };

            var response = await client.GetAsync(request);
            var responseContent = response.Content.ToString();
            JObject jsonResponse = JObject.Parse(responseContent);
            JArray objResponse = (JArray)jsonResponse["results"];
            List<object> _Data = JsonConvert.DeserializeObject<List<object>>(objResponse.ToString());
            List<ItemCreacionDTO> ListCreacionDTO = new List<ItemCreacionDTO>();

            foreach (object item in _Data)
            {
                dynamic jsonItem = JObject.Parse(item.ToString());

                ListCreacionDTO.Add(new ItemCreacionDTO()
                {
                    id_item = jsonItem.id,
                    price = jsonItem.price,
                });
            }

            return ListCreacionDTO;
        }
        #endregion
    }
}
