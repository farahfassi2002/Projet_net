using System.ComponentModel.DataAnnotations;

namespace PortefeuilleInvestissement.Models
{
   
    public class Administrateur
    {
        [Key]
        public int AdminId { get; set; }

        public List<string> InvestisseursGeres { get; set; } = new();

  
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }

        public void GererUtilisateurs() { }
        public void GererActifs()       { }
        public void GererTransactions() { }
    }
}
