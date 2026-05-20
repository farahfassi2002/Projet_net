using PortefeuilleInvestissement.Models;

namespace PortefeuilleInvestissement.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsAsync();
        Task<List<Transaction>> GetTransactionsByInvestisseurAsync(int investisseurId);
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task AjouterTransactionAsync(Transaction transaction);
        Task ModifierTransactionAsync(Transaction transaction);
        Task SupprimerTransactionAsync(int id);

        
        Task<float> GetValeurPortefeuilleAsync(int investisseurId);
        Task<float> GetGainPerteGlobalAsync(int investisseurId);
        Task<List<StatActif>> GetPerformanceParActifAsync(int investisseurId);
        Task<List<StatTypeActif>> GetRepartitionParTypeAsync(int investisseurId);
        Task<List<Transaction>> RechercherTransactionsAsync(int investisseurId, TypeTransaction? type, StatutTransaction? statut, string? symbole);
    }
}
