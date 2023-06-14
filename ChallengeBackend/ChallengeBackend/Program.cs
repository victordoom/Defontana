using ChallengeBackend;
using ChallengeBackend.Data;
using ChallengeBackend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
});

builder.ConfigureServices((hostContext, services) =>
{
    IConfiguration configuration = hostContext.Configuration;

    string? connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");

    services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

    services.AddScoped<IVentasRepository, VentasRepository>();

    services.AddSingleton<IServiceProvider>(provider => provider.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
});

var host = builder.Build();

await ObtenerVentas(host, 30);

host.Run();

async Task ObtenerVentas(IHost host, int numberDays)
{
    using (var scope = host.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;

        var ventasRepository = serviceProvider.GetRequiredService<IVentasRepository>();

        //  Ventas de los ultimos n dias en este caso 30 dias
        var ventas = await ventasRepository.GetVentas(numberDays);

        //El total de ventas de los últimos 30 días (monto total y cantidad total de ventas).
        //monto total
        var TotalVentas = ventas.Sum(x => x.Total);
        //cantidad total
        var cantidadTotal = ventas.Sum(x => x.VentaDetalles.Sum(z => z.Cantidad));

        Console.WriteLine("El total de ventas de los últimos 30 días (monto total y cantidad total de ventas).");
        Console.WriteLine($"Monto Total: {TotalVentas}, Cantidad Total: {cantidadTotal}");
        Console.WriteLine();

        //El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto)
        var bestDay = ventas.OrderByDescending(x => x.Total).Select(z => new { Fecha = z.Fecha, Monto = z.Total }).FirstOrDefault();

        Console.WriteLine("El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto)");
        Console.WriteLine($"Fecha y Hora: {bestDay?.Fecha}, Monto: {bestDay?.Monto}");
        Console.WriteLine();

        //Indicar cuál es el producto con mayor monto total de ventas
        var bestMontoProduct = ventas.SelectMany(x => x.VentaDetalles)
            .GroupBy(x => x.IdProductoNavigation)
            .Select(z => new {
                IdProducto = z.Key.IdProducto,
                NombreProducto = z.Key.Nombre,
                NombreMarca = z.Key.IdMarcaNavigation.Nombre,
                TotalSum = z.Select(y => y.TotalLinea).Sum()
            })
            .OrderByDescending(y => y.TotalSum)
            .First();


        Console.WriteLine("Indicar cuál es el producto con mayor monto total de ventas");
        Console.WriteLine($"Producto: {bestMontoProduct.NombreProducto}, Monto: {bestMontoProduct.TotalSum}");
        Console.WriteLine();

        //Indicar el local con mayor monto de ventas.
        var bestLocal = ventas.GroupBy(x => x.IdLocalNavigation)
            .Select(x => new
            {
                localId = x.Key.IdLocal,
                NombreLocal = x.Key.Nombre,
                Total = x.Select(x => x.Total).Sum()
            })
            .OrderByDescending(y => y.Total)
            .First();

        Console.WriteLine("Indicar el local con mayor monto de ventas.");
        Console.WriteLine($"ID_Local: {bestLocal.localId}, Local: {bestLocal.NombreLocal}, Monto: {bestLocal.Total}");
        Console.WriteLine();

        //¿Cuál es la marca con mayor margen de ganancias?
        var bestMarca = bestMontoProduct.NombreMarca;

        Console.WriteLine("¿Cuál es la marca con mayor margen de ganancias?");
        Console.WriteLine($"Marca: {bestMontoProduct.NombreMarca}, Monto: {bestMontoProduct.TotalSum}");
        Console.WriteLine();



        //¿Cómo obtendrías cuál es el producto que más se vende en cada local?
        var detallewithLocal = ventas
            .Select(x => new
            {
                detalles = x.VentaDetalles.GroupBy(y => y.IdProductoNavigation)
                                .Select(z => new {
                                    IdProduc = z.Key.IdProducto,
                                    NombreP = z.Key.Nombre,
                                    TotalSum = z.Select(q => q.TotalLinea).Sum(),
                                    IdLocal = x.IdLocal,
                                    NombreLocal = x.IdLocalNavigation.Nombre
                                })
            });

        
        var bestProductforLocal = detallewithLocal.SelectMany(x => x.detalles)
            .GroupBy(y => y.IdLocal)
            .Select(z => z.OrderByDescending(t => t.TotalSum).FirstOrDefault());


        Console.WriteLine("¿Cómo obtendrías cuál es el producto que más se vende en cada local?");
        foreach (var probestlocal in bestProductforLocal)
        {
            Console.WriteLine($"Local: {probestlocal?.NombreLocal}, Producto: {probestlocal?.NombreP}, Monto: {probestlocal?.TotalSum}");
        }
        Console.WriteLine();



        //Producto mas ventido por cantidad
        var bestProducto = (from ven in ventas
                           from detail in ven.VentaDetalles
                           group detail by detail.IdProductoNavigation into p
                           select new
                           {
                               productID = p.Key.IdProducto,
                               Nombre = p.Key.Nombre,
                               TotalCantidad = p.Sum(x => x.Cantidad)
                           } into plist
                           orderby plist.TotalCantidad descending
                           select plist).First();


        Console.WriteLine("EXTRA: Indicar cuál es el producto con mayor monto total de ventas");
        Console.WriteLine($"Producto: {bestProducto.Nombre}, Cantidad: {bestProducto.TotalCantidad}");

    }
}
