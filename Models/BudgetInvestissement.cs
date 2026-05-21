using System.ComponentModel.DataAnnotations;

namespace PortefeuilleInvestissement.Models
{
    /// <summary>
    /// Enregistrement d'une allocation de budget par un investisseur.
    /// </summary>
    public class BudgetInvestissement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être positif.")]
        public float Montant { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        //====== Entity Framework Core relationships ======

        // Clé étrangère vers Investisseur (1..*)
        [Required]
        public int InvestisseurId { get; set; }
        public Investisseur Investisseur { get; set; }
    }
}
