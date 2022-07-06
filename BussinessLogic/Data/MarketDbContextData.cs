using Core.Entities;
using Core.Entities.OrdenCompra;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BussinessLogic.Data
{
    public class MarketDbContextData
    {
        public static async Task CargarDataAsync(MarketDbContext context, ILoggerFactory loggerFactory) {
            try
            {
                //Para agragar las marcas a la bd
                if(!context.Marca.Any())
                {
                    var marcaData = File.ReadAllText("../BussinessLogic/CargarData/marca.json");
                    var marcas = JsonSerializer.Deserialize<List<Marca>>(marcaData);

                    foreach (var marca in marcas)
                    {
                        context.Marca.Add(marca);
                    }

                    await context.SaveChangesAsync();
                }

                //Para agregar las categorias a la bd
                if (!context.Categoria.Any())
                {
                    var categoriaData = File.ReadAllText("../BussinessLogic/CargarData/categoria.json");
                    var categorias = JsonSerializer.Deserialize<List<Categoria>>(categoriaData);

                    foreach (var categoria in categorias)
                    {
                        context.Categoria.Add(categoria);
                    }

                    await context.SaveChangesAsync();
                }

                //Para agregar los productos a la bd
                if (!context.Producto.Any())
                {
                    var productoData = File.ReadAllText("../BussinessLogic/CargarData/producto.json");
                    var productos = JsonSerializer.Deserialize<List<Producto>>(productoData);

                    foreach (var producto in productos)
                    {
                        context.Producto.Add(producto);
                    }

                    await context.SaveChangesAsync();
                }

                //Para agregra tipos de envios a la bd
                if (!context.TipoEnvios.Any())
                {
                    var tipoenvioData = File.ReadAllText("../BussinessLogic/CargarData/tipoenvio.json");
                    var tipoEnvios = JsonSerializer.Deserialize<List<TipoEnvio>>(tipoenvioData);

                    foreach (var tipoEnvio in tipoEnvios)
                    {
                        context.TipoEnvios.Add(tipoEnvio);
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex) { 
                var logger = loggerFactory.CreateLogger<MarketDbContextData>();
                logger.LogError(ex.Message);
            }
        }
    }
}
