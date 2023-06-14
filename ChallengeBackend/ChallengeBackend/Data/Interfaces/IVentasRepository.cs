namespace ChallengeBackend.Data

{
    using Entities;
    using System.Threading.Tasks;

    public interface IVentasRepository
    {
        Task<List<Ventum>> GetVentas(int days);
    }
}

