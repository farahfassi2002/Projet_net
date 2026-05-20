using PortefeuilleInvestissement.Models;

namespace PortefeuilleInvestissement.Services
{
    public interface IInvestisseurService
    {
        Task<Investisseur?> GetInvestisseurByUserIdAsync(string userId);
        Task<Investisseur?> GetInvestisseurByIdAsync(int id);
        Task<List<Investisseur>> GetAllInvestisseursAsync();
        Task ModifierBudgetAsync(int investisseurId, float nouveauBudget);
        Task AllouerBudgetAsync(int investisseurId, float montant, string description);
        Task<List<BudgetInvestissement>> GetHistoriqueBudgetsAsync(int investisseurId);
        Task<float> GetBudgetDisponibleAsync(int investisseurId);
    }
}
