using Defontana.Services;


Console.WriteLine();

Console.WriteLine("RESPUESTAS");
Console.WriteLine("------------------------------");

var ventas = await new VentaService().ObtenerVentas(30);

//•	El total de ventas de los últimos 30 días (monto total y cantidad total de ventas).


Console.WriteLine("•	El total de ventas de los últimos 30 días (monto total y cantidad total de ventas)");
Console.WriteLine(" ");

var totalVentas = ventas
          .GroupBy(v => 1)
          .Select(g => new
          {
              TotalVentas = g.Count(),
              MontoTotal = g.Sum(v => v.Total)
          })
          .FirstOrDefault();

if (totalVentas != null)
{
    Console.WriteLine($"Cantidad total de ventas: {totalVentas.TotalVentas}");
    Console.WriteLine($"Monto total de ventas: {totalVentas.MontoTotal.ToString("N")}");
}

//•	El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto).

Console.WriteLine(" ");
Console.WriteLine("•	El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto).");
Console.WriteLine(" ");


var ventaAlta = ventas
          .OrderByDescending(v => v.Total)
          .Select(v => new
          {
              Fecha = v.Fecha,
              MontoVenta = v.Total
          })
          .FirstOrDefault();

if (ventaAlta != null)
{
    Console.WriteLine($"Día y hora de la venta más alta: {ventaAlta.Fecha.ToString("dd-MMM-yyyy hh:mm:ss")}");
    Console.WriteLine($"Monto de la venta más alta: {ventaAlta.MontoVenta.ToString("N")}");
}

//•		Indicar cuál es el producto con mayor monto total de ventas.

Console.WriteLine(" ");
Console.WriteLine("•	Indicar cuál es el producto con mayor monto total de ventas");
Console.WriteLine(" ");


try
{

    var productoMasVendido = ventas
            .SelectMany(v => v.VentaDetalles)
            .GroupBy(vd => vd.IdProductoNavigation)
            .Select(g => new
            {
                Producto = g.Key.Nombre,
                IdProducto = g.Key.IdProducto,
                MontoTotalVentas = g.Sum(vd => vd.TotalLinea)
            })
            .OrderByDescending(x => x.MontoTotalVentas)
            .FirstOrDefault();



    Console.WriteLine($"Producto con mayor monto total de ventas Id: {productoMasVendido.IdProducto}  Nombre: {productoMasVendido.Producto}");


    //•	Indicar el local con mayor monto de ventas

    Console.WriteLine(" ");
    Console.WriteLine("•	Indicar el local con mayor monto de ventas");
    Console.WriteLine(" ");

    var localMayorVenta = ventas
            .GroupBy(v => new { v.IdLocalNavigation.Nombre })
            .Select(g => new
            {
                Local = g.Key,
                MontoTotalVentas = g.Sum(v => v.Total)
            })
            .OrderByDescending(x => x.MontoTotalVentas)
            .FirstOrDefault();



    Console.WriteLine($"Local con mayor monto de ventas: {localMayorVenta.Local}");


    //•	¿Cuál es la marca con mayor margen de ganancias?

    Console.WriteLine(" ");
    Console.WriteLine("•	¿Cuál es la marca con mayor margen de ganancias?");
    Console.WriteLine(" ");


    var marca = ventas
          .SelectMany(v => v.VentaDetalles)
          .GroupBy(vd => vd.IdProductoNavigation.IdMarcaNavigation.Nombre)
          .Select(g => new
          {
              Marca = g.Key,
              MargenGanancias = g.Sum(vd => vd.PrecioUnitario - vd.IdProductoNavigation.CostoUnitario)
          })
          .OrderByDescending(x => x.MargenGanancias)
          .FirstOrDefault();

    Console.WriteLine($"Marca con mayor margen ventas: {marca.Marca}");



    //•	¿Cómo obtendrías cuál es el producto que más se vende en cada local?

    Console.WriteLine(" ");
    Console.WriteLine("•	¿Cómo obtendrías cuál es el producto que más se vende en cada local?");
    Console.WriteLine(" ");

    var productosMasVendidosPorLocal = ventas
           .SelectMany(v => v.VentaDetalles)
           .GroupBy(vd => new { vd.IdVentaNavigation.IdLocal, vd.IdProductoNavigation })
           .Select(g => new
           {
               IdLocal = g.Key.IdLocal,
               IdProducto = g.Key.IdProductoNavigation.IdProducto,
               Nombre = g.Key.IdProductoNavigation.Nombre,
               CantidadTotal = g.Sum(vd => vd.Cantidad)
           })
           .GroupBy(x => x.IdLocal)
           .Select(g => new
           {
               IdLocal = g.Key,
               ProductoMasVendido = g.OrderByDescending(x => x.CantidadTotal).FirstOrDefault()?.Nombre,
               ProductoIdMasVendido = g.OrderByDescending(x => x.CantidadTotal).FirstOrDefault()?.IdProducto
           })
           .OrderBy(x => x.IdLocal)
           .ToList();

    productosMasVendidosPorLocal.ForEach(x => Console.WriteLine($"Local id: {x.IdLocal} producto mas vendido: {x.ProductoMasVendido}"));

}
catch (Exception ex)
{
    throw new Exception(ex.Message);
}


Console.ReadLine();
Console.ReadKey();
