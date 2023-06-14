namespace ChallengeBackend.Data

{
    using System.Threading.Tasks;
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public class VentasRepository : IVentasRepository
    {
        private readonly ApplicationDbContext context;

        public VentasRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task<List<Ventum>> GetVentas(int days)
        {
            DateTime date = DateTime.Today.AddDays(-days);

            return this.context.Venta
                .Include( z => z.VentaDetalles)
                    .ThenInclude(det => det.IdProductoNavigation)
                    .ThenInclude(det => det.IdMarcaNavigation)
                .Include(z => z.IdLocalNavigation)
                .Where(x => x.Fecha >= date).ToListAsync();
        }
    }
}

