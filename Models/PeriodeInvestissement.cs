using System.ComponentModel.DataAnnotations;

namespace PortefeuilleInvestissement.Models
{
    
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


        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
