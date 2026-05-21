using System.ComponentModel.DataAnnotations;

namespace PortefeuilleInvestissement.Models
{
    /// <summary>
    /// Investisseur — sous-type d'ApplicationUser.
    /// Possède un budget, des points fidélité et un historique de transactions.
    /// Correspond à la classe Client du diagramme de classe.
    /// </summary>
    public class Investisseur
    {
        [Key]
        public int InvestisseurId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le budget total ne peut pas être négatif.")]
        public float BudgetTotal { get; set; } = 0f;

        [Range(0, double.MaxValue)]
        public float BudgetDisponible { get; set; } = 0f;

        public int PointsFidelite { get; set; } = 0;

        //====== Entity Framework Core relationships ======

        // Clé étrangère vers ApplicationUser (1 à 1)
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }

        // Un investisseur possède plusieurs transactions (1..*)
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        //====== Méthodes métier ======
        public void GererTransactions() { /* géré via ITransactionService */ }
        public void RechercherActif() { /* géré via IActifService */ }
    }
}
