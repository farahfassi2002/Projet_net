using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortefeuilleInvestissement.Models
{
    /// <summary>
    /// Actif financier (action, cryptomonnaie, ETF, obligation).
    /// Correspond à la classe Chambre du diagramme de classe.
    /// </summary>
    public class Actif
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit faire entre 2 et 100 caractères.")]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Le symbole est requis.")]
        public string Symbole { get; set; } = string.Empty;

        public TypeActif Type { get; set; } = TypeActif.Action;

        [Range(0.0001, double.MaxValue, ErrorMessage = "Le prix doit être positif.")]
        public float PrixActuel { get; set; }

        [Range(0, double.MaxValue)]
        public float PrixAchatMoyen { get; set; }

        [Range(0, double.MaxValue)]
        public float QuantiteDisponible { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime DerniereMAJ { get; set; } = DateTime.Now;

        //====== Entity Framework Core relationships ======

        // Un actif peut être impliqué dans plusieurs transactions (1..*)
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        //====== Méthodes métier ======
        public void AjouterActif()   { }
        public void ModifierActif()  { }
        public void SupprimerActif() { }

        //====== Propriétés calculées ======
        [NotMapped]
        public float QuantiteTotaleDetenue =>
            Transactions?.Where(t => t.Statut == StatutTransaction.Confirmee)
                         .Sum(t => t.Type == TypeTransaction.Achat ? t.Quantite : -t.Quantite) ?? 0f;

        [NotMapped]
        public float ValeurActuellePosition => QuantiteTotaleDetenue * PrixActuel;
    }
}
