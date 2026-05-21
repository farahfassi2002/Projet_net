using System.ComponentModel.DataAnnotations;

namespace PortefeuilleInvestissement.Models
{
    /// <summary>
    /// Administrateur — sous-type d'ApplicationUser.
    /// Gère les utilisateurs, les actifs et les transactions.
    /// Correspond à la classe Administrateur du diagramme de classe.
    /// </summary>
    public class Administrateur
    {
        [Key]
        public int AdminId { get; set; }

        // Liste des investisseurs gérés par cet administrateur
        public List<string> InvestisseursGeres { get; set; } = new();

        //====== Entity Framework Core relationships ======

        // Clé étrangère vers ApplicationUser (1 à 1)
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }

        //====== Méthodes métier ======
        public void GererUtilisateurs() { }
        public void GererActifs()       { }
        public void GererTransactions() { }
    }
}
