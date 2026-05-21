using System.ComponentModel.DataAnnotations;

namespace PortefeuilleInvestissement.Models
{
    /// <summary>
    /// Période d'investissement définie par une date de début et une date de fin.
    /// Correspond à la classe Periode_de_location du diagramme de classe.
    /// </summary>
    public class PeriodeInvestissement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateDebut { get; set; } = DateTime.Now;

        [Required]
        public DateTime DateFin { get; set; } = DateTime.Now.AddMonths(12);

        [StringLength(100)]
        public string Libelle { get; set; } = string.Empty;

        //====== Entity Framework Core relationships ======

        // Une période peut contenir plusieurs transactions (1..*)
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
