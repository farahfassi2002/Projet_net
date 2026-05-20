using System.ComponentModel.DataAnnotations;

namespace PortefeuilleInvestissement.Models
{
 
    public class Investisseur
    {
        [Key]
        public int InvestisseurId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le budget total ne peut pas être négatif.")]
        public float BudgetTotal { get; set; } = 0f;

        [Range(0, double.MaxValue)]
        public float BudgetDisponible { get; set; } = 0f;

        public int PointsFidelite { get; set; } = 0;

   
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public void GererTransactions() {  }
        public void RechercherActif()   { }
    }
}
