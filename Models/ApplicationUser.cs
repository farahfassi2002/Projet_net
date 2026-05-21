using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PortefeuilleInvestissement.Models
{
  
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Prenom { get; set; } = string.Empty;

        [StringLength(20)]
        public string NumTel { get; set; } = string.Empty;

        public string Role { get; set; } = "Investisseur";


        public Investisseur? Investisseur { get; set; }

        public Administrateur? Administrateur { get; set; }
    }
}
