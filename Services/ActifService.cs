using Microsoft.EntityFrameworkCore;
using PortefeuilleInvestissement.Data;
using PortefeuilleInvestissement.Models;

namespace PortefeuilleInvestissement.Services
{
    public class ActifService : IActifService
    {
        private readonly AppDbContext _db;

        public ActifService(AppDbContext db) => _db = db;

        public async Task<List<Actif>> GetActifsAsync()
        {
            return await _db.Actifs
                .Include(a => a.Transactions)
                .OrderBy(a => a.Nom)
                .ToListAsync();
        }

        public async Task<Actif?> GetActifByIdAsync(int id)
        {
            return await _db.Actifs
                .Include(a => a.Transactions)
                    .ThenInclude(t => t.Investisseur)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AjouterActifAsync(Actif actif)
        {
            actif.DerniereMAJ = DateTime.Now;
            await _db.Actifs.AddAsync(actif);
            await _db.SaveChangesAsync();
        }

        public async Task ModifierActifAsync(Actif actif)
        {
            var existing = await _db.Actifs.FindAsync(actif.Id)
                ?? throw new KeyNotFoundException($"Actif ID {actif.Id} introuvable.");

            existing.Nom = actif.Nom;
            existing.Symbole = actif.Symbole;
            existing.Type = actif.Type;
            existing.PrixActuel = actif.PrixActuel;
            existing.PrixAchatMoyen = actif.PrixAchatMoyen;
            existing.QuantiteDisponible = actif.QuantiteDisponible;
            existing.Description = actif.Description;
            existing.DerniereMAJ = DateTime.Now;

            _db.Actifs.Update(existing);
            await _db.SaveChangesAsync();
        }

        public async Task SupprimerActifAsync(int id)
        {
            var actif = await _db.Actifs.FindAsync(id);
            if (actif != null)
            {
                _db.Actifs.Remove(actif);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Actif>> RechercherActifsAsync(string? searchText, TypeActif? type)
        {
            IQueryable<Actif> query = _db.Actifs.Include(a => a.Transactions);

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(a => a.Nom.Contains(searchText) || a.Symbole.Contains(searchText));

            if (type.HasValue)
                query = query.Where(a => a.Type == type.Value);

            return await query.OrderBy(a => a.Nom).ToListAsync();
        }

        public async Task<int> GetNombreActifsAsync()
            => await _db.Actifs.CountAsync();
    }
}
