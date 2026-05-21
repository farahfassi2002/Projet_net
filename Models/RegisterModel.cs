using System.ComponentModel.DataAnnotations;

namespace PortefeuilleInvestissement.Models
{

    public class RegisterModel
    {
        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(50)]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50)]
        public string Prenom { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [MinLength(6, ErrorMessage = "Minimum 6 caractères")]
        public string MotDePasse { get; set; } = string.Empty;

        [Required]
        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmMotDePasse { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Le budget doit être positif")]
        public float BudgetInitial { get; set; } = 10000f;
    }
}
