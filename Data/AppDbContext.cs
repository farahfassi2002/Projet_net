using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortefeuilleInvestissement.Models;

namespace PortefeuilleInvestissement.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Investisseur> Investisseurs { get; set; }
        public DbSet<Administrateur> Administrateurs { get; set; }
        public DbSet<Actif> Actifs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PeriodeInvestissement> Periodes { get; set; }
        public DbSet<BudgetInvestissement> Budgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation ApplicationUser -> Investisseur (1 à 1)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Investisseur)
                .WithOne(i => i.User)
                .HasForeignKey<Investisseur>(i => i.UserId);

            // Relation ApplicationUser -> Administrateur (1 à 1)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Administrateur)
                .WithOne(a => a.User)
                .HasForeignKey<Administrateur>(a => a.UserId);

            // Relation Investisseur -> Transaction (1 à plusieurs)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Investisseur)
                .WithMany(i => i.Transactions)
                .HasForeignKey(t => t.InvestisseurId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Actif -> Transaction (1 à plusieurs)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Actif)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.ActifId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation PeriodeInvestissement -> Transaction (optionnelle)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Periode)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PeriodeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation Investisseur -> BudgetInvestissement (1 à plusieurs)
            modelBuilder.Entity<BudgetInvestissement>()
                .HasOne(b => b.Investisseur)
                .WithMany()
                .HasForeignKey(b => b.InvestisseurId)
                .OnDelete(DeleteBehavior.Cascade);

            // Stocker la liste InvestisseursGeres comme JSON
            modelBuilder.Entity<Administrateur>()
                .Property(a => a.InvestisseursGeres)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        }
    }
}
