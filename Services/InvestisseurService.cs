using Microsoft.EntityFrameworkCore;
using PortefeuilleInvestissement.Data;
using PortefeuilleInvestissement.Models;

namespace PortefeuilleInvestissement.Services
{
    public class InvestisseurService : IInvestisseurService
    {
        private readonly AppDbContext _db;

        public InvestisseurService(AppDbContext db) => _db = db;

        public async Task<Investisseur?> GetInvestisseurByUserIdAsync(string userId)
        {
            return await _db.Investisseurs
                .Include(i => i.User)
                .Include(i => i.Transactions).ThenInclude(t => t.Actif)
                .FirstOrDefaultAsync(i => i.UserId == userId);
        }

        public async Task<Investisseur?> GetInvestisseurByIdAsync(int id)
        {
            return await _db.Investisseurs
                .Include(i => i.User)
                .Include(i => i.Transactions).ThenInclude(t => t.Actif)
                .FirstOrDefaultAsync(i => i.InvestisseurId == id);
        }

        public async Task<List<Investisseur>> GetAllInvestisseursAsync()
        {
            return await _db.Investisseurs
                .Include(i => i.User)
                .OrderBy(i => i.User.Nom)
                .ToListAsync();
        }

        public async Task ModifierBudgetAsync(int investisseurId, float nouveauBudget)
        {
            var inv = await _db.Investisseurs.FindAsync(investisseurId)
                ?? throw new KeyNotFoundException("Investisseur introuvable.");
            inv.BudgetTotal = nouveauBudget;
            inv.BudgetDisponible = nouveauBudget;
            _db.Investisseurs.Update(inv);
            await _db.SaveChangesAsync();
        }

        public async Task AllouerBudgetAsync(int investisseurId, float montant, string description)
        {
            var inv = await _db.Investisseurs.FindAsync(investisseurId)
                ?? throw new KeyNotFoundException("Investisseur introuvable.");

            inv.BudgetTotal += montant;
            inv.BudgetDisponible += montant;
            _db.Investisseurs.Update(inv);

            var budget = new BudgetInvestissement
            {
                Montant = montant,
                Description = description,
                InvestisseurId = investisseurId,
                DateCreation = DateTime.Now
            };
            await _db.Budgets.AddAsync(budget);
            await _db.SaveChangesAsync();
        }

        public async Task<List<BudgetInvestissement>> GetHistoriqueBudgetsAsync(int investisseurId)
        {
            return await _db.Budgets
                .Where(b => b.InvestisseurId == investisseurId)
                .OrderByDescending(b => b.DateCreation)
                .ToListAsync();
        }

        public async Task<float> GetBudgetDisponibleAsync(int investisseurId)
        {
            var inv = await _db.Investisseurs.FindAsync(investisseurId);
            return inv?.BudgetDisponible ?? 0f;
        }
    }
}
