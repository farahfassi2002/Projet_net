using Microsoft.EntityFrameworkCore;
using PortefeuilleInvestissement.Data;
using PortefeuilleInvestissement.Models;

namespace PortefeuilleInvestissement.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _db;

        public TransactionService(AppDbContext db) => _db = db;

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            return await _db.Transactions
                .Include(t => t.Actif)
                .Include(t => t.Investisseur).ThenInclude(i => i.User)
                .Include(t => t.Periode)
                .OrderByDescending(t => t.DateTransaction)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByInvestisseurAsync(int investisseurId)
        {
            return await _db.Transactions
                .Include(t => t.Actif)
                .Include(t => t.Periode)
                .Where(t => t.InvestisseurId == investisseurId)
                .OrderByDescending(t => t.DateTransaction)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _db.Transactions
                .Include(t => t.Actif)
                .Include(t => t.Investisseur).ThenInclude(i => i.User)
                .Include(t => t.Periode)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AjouterTransactionAsync(Transaction transaction)
        {
            transaction.MontantTotal = transaction.Quantite * transaction.PrixUnitaire;
            transaction.DateTransaction = DateTime.Now;

            var investisseur = await _db.Investisseurs.FindAsync(transaction.InvestisseurId);
            if (investisseur != null && transaction.Statut == StatutTransaction.Confirmee)
            {
                if (transaction.Type == TypeTransaction.Achat)
                    investisseur.BudgetDisponible -= transaction.MontantTotal;
                else
                    investisseur.BudgetDisponible += transaction.MontantTotal;

                _db.Investisseurs.Update(investisseur);
            }

            await _db.Transactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
        }

        public async Task ModifierTransactionAsync(Transaction transaction)
        {
            var existing = await _db.Transactions.FindAsync(transaction.Id)
                ?? throw new KeyNotFoundException($"Transaction ID {transaction.Id} introuvable.");

            existing.Statut = transaction.Statut;
            existing.PrixUnitaire = transaction.PrixUnitaire;
            existing.Quantite = transaction.Quantite;
            existing.MontantTotal = transaction.Quantite * transaction.PrixUnitaire;
            existing.Type = transaction.Type;
            existing.Notes = transaction.Notes;
            existing.ActifId = transaction.ActifId;
            existing.PeriodeId = transaction.PeriodeId;

            _db.Transactions.Update(existing);
            await _db.SaveChangesAsync();
        }

        public async Task SupprimerTransactionAsync(int id)
        {
            var t = await _db.Transactions.FindAsync(id);
            if (t != null)
            {
                _db.Transactions.Remove(t);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<float> GetValeurPortefeuilleAsync(int investisseurId)
        {
            var transactions = await _db.Transactions
                .Include(t => t.Actif)
                .Where(t => t.InvestisseurId == investisseurId && t.Statut == StatutTransaction.Confirmee)
                .ToListAsync();

            float valeur = 0f;
            var groupes = transactions.GroupBy(t => t.ActifId);
            foreach (var g in groupes)
            {
                float qte = g.Sum(t => t.Type == TypeTransaction.Achat ? t.Quantite : -t.Quantite);
                float prixActuel = g.First().Actif?.PrixActuel ?? 0f;
                valeur += qte * prixActuel;
            }
            return valeur;
        }

        public async Task<float> GetGainPerteGlobalAsync(int investisseurId)
        {
            var transactions = await _db.Transactions
                .Include(t => t.Actif)
                .Where(t => t.InvestisseurId == investisseurId && t.Statut == StatutTransaction.Confirmee)
                .ToListAsync();

            float valeurActuelle = 0f;
            float coutTotal = 0f;

            var groupes = transactions.GroupBy(t => t.ActifId);
            foreach (var g in groupes)
            {
                float qte = g.Sum(t => t.Type == TypeTransaction.Achat ? t.Quantite : -t.Quantite);
                if (qte <= 0) continue;

                float prixActuel = g.First().Actif?.PrixActuel ?? 0f;
                float prixMoyenAchat = g.First().Actif?.PrixAchatMoyen ?? 0f;

                valeurActuelle += qte * prixActuel;
                coutTotal += qte * prixMoyenAchat;
            }

            return valeurActuelle - coutTotal;
        }

        public async Task<List<StatActif>> GetPerformanceParActifAsync(int investisseurId)
        {
            var transactions = await _db.Transactions
                .Include(t => t.Actif)
                .Where(t => t.InvestisseurId == investisseurId && t.Statut == StatutTransaction.Confirmee)
                .ToListAsync();

            var stats = transactions
                .GroupBy(t => t.ActifId)
                .Select(g =>
                {
                    var actif = g.First().Actif;
                    float qte = g.Sum(t => t.Type == TypeTransaction.Achat ? t.Quantite : -t.Quantite);
                    float coutTotal = g.Where(t => t.Type == TypeTransaction.Achat).Sum(t => t.MontantTotal)
                                    - g.Where(t => t.Type == TypeTransaction.Vente).Sum(t => t.MontantTotal);
                    float valAct = qte * (actif?.PrixActuel ?? 0f);

                    return new StatActif
                    {
                        NomActif = actif?.Nom ?? "Inconnu",
                        Symbole = actif?.Symbole ?? "?",
                        Type = actif?.Type ?? TypeActif.Action,
                        Quantite = qte,
                        PrixMoyenAchat = actif?.PrixAchatMoyen ?? 0f,
                        PrixActuel = actif?.PrixActuel ?? 0f,
                        ValeurActuelle = valAct,
                        CoutTotal = coutTotal
                    };
                })
                .Where(s => s.Quantite > 0)
                .OrderByDescending(s => s.ValeurActuelle)
                .ToList();

            return stats;
        }

        public async Task<List<StatTypeActif>> GetRepartitionParTypeAsync(int investisseurId)
        {
            var perf = await GetPerformanceParActifAsync(investisseurId);
            float total = perf.Sum(s => s.ValeurActuelle);

            return perf
                .GroupBy(s => s.Type)
                .Select(g => new StatTypeActif
                {
                    TypeLabel = g.Key.ToString(),
                    ValeurTotale = g.Sum(s => s.ValeurActuelle),
                    Pourcentage = total > 0 ? (g.Sum(s => s.ValeurActuelle) / total) * 100f : 0f
                })
                .OrderByDescending(s => s.ValeurTotale)
                .ToList();
        }

        public async Task<List<Transaction>> RechercherTransactionsAsync(int investisseurId, TypeTransaction? type, StatutTransaction? statut, string? symbole)
        {
            IQueryable<Transaction> query = _db.Transactions
                .Include(t => t.Actif)
                .Include(t => t.Periode)
                .Where(t => t.InvestisseurId == investisseurId);

            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            if (statut.HasValue)
                query = query.Where(t => t.Statut == statut.Value);

            if (!string.IsNullOrWhiteSpace(symbole))
                query = query.Where(t => t.Actif.Symbole.Contains(symbole) || t.Actif.Nom.Contains(symbole));

            return await query.OrderByDescending(t => t.DateTransaction).ToListAsync();
        }
    }
}
