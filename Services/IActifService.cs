using PortefeuilleInvestissement.Models;

namespace PortefeuilleInvestissement.Services
{
    public interface IActifService
    {
        Task<List<Actif>> GetActifsAsync();
        Task<Actif?> GetActifByIdAsync(int id);
        Task AjouterActifAsync(Actif actif);
        Task ModifierActifAsync(Actif actif);
        Task SupprimerActifAsync(int id);
        Task<List<Actif>> RechercherActifsAsync(string? searchText, TypeActif? type);
        Task<int> GetNombreActifsAsync();
    }
}
