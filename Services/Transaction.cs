using System.ComponentModel.DataAnnotations;

namespace PortefeuilleInvestissement.Models
{
  
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public StatutTransaction Statut { get; set; } = StatutTransaction.En_attente;

        [Range(0.0001, double.MaxValue, ErrorMessage = "Le prix unitaire doit être positif.")]
        public float PrixUnitaire { get; set; }

        public TypeTransaction Type { get; set; } = TypeTransaction.Achat;

        [Range(0.0001, double.MaxValue, ErrorMessage = "La quantité doit être positive.")]
        public float Quantite { get; set; }

        [Range(0, double.MaxValue)]
        public float MontantTotal { get; set; }

        public DateTime DateTransaction { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;


        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Veuillez sélectionner un actif valide.")]
        public int ActifId { get; set; }
        public Actif Actif { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Veuillez sélectionner un investisseur valide.")]
        public int InvestisseurId { get; set; }
        public Investisseur Investisseur { get; set; }

        public int? PeriodeId { get; set; }
        public PeriodeInvestissement? Periode { get; set; }

        public void AjouterTransaction()   { }
        public void ModifierTransaction()  { }
        public void SupprimerTransaction() { }
    }
}
