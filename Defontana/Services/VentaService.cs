using Defontana.Models;
using Microsoft.EntityFrameworkCore;

namespace Defontana.Services
{
    public class VentaService
    {
        public VentaService() { }

        public async Task<List<Ventum>> ObtenerVentas(int dia)
        {
            var ventas = new List<Ventum>(); ;

            using (var db = new PruebaContext())
            {
                ventas = await db.Venta
               .Include(l => l.IdLocalNavigation)
               .Include(x => x.VentaDetalles)
               .ThenInclude(vd => vd.IdProductoNavigation)
               .ThenInclude(pr => pr.IdMarcaNavigation)
               .Where(x => x.Fecha > DateTime.Now.AddDays(-dia)).ToListAsync();
            }


            return ventas;
        }
    }
}
