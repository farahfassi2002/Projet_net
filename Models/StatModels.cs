namespace PortefeuilleInvestissement.Models
{
    /// <summary>
    /// Statistique de performance pour un actif donné du portefeuille.
    /// </summary>
    public class StatActif
    {
        public string NomActif { get; set; } = string.Empty;
        public string Symbole { get; set; } = string.Empty;
        public TypeActif Type { get; set; }
        public float Quantite { get; set; }
        public float PrixMoyenAchat { get; set; }
        public float PrixActuel { get; set; }
        public float ValeurActuelle { get; set; }
        public float CoutTotal { get; set; }

        // Propriétés calculées
        public float GainPerte    => ValeurActuelle - CoutTotal;
        public float GainPertePct => CoutTotal != 0 ? (GainPerte / CoutTotal) * 100f : 0f;
    }

    /// <summary>
    /// Répartition du portefeuille par type d'actif.
    /// </summary>
    public class StatTypeActif
    {
        public string TypeLabel { get; set; } = string.Empty;
        public float ValeurTotale { get; set; }
        public float Pourcentage { get; set; }
    }
}
