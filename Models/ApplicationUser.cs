using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PortefeuilleInvestissement.Models
{
    /// <summary>
    /// Utilisateur de l'application — hérite d'IdentityUser.
    /// Correspond à la classe Utilisateur du diagramme de classe.
    /// </summary>
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

        // Rôle applicatif (Investisseur / Administrateur)
        public string Role { get; set; } = "Investisseur";

        //====== Entity Framework Core relationships ======

        // Un utilisateur peut être un Investisseur (1 à 1)
        public Investisseur? Investisseur { get; set; }

        // Un utilisateur peut être un Administrateur (1 à 1)
        public Administrateur? Administrateur { get; set; }
    }
}
