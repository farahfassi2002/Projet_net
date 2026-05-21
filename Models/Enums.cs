namespace PortefeuilleInvestissement.Models
{
    /// <summary>Type d'actif financier</summary>
    public enum TypeActif
    {
        Action,
        CryptoMonnaie,
        ETF,
        Obligation
    }

    /// <summary>Type d'opération sur un actif</summary>
    public enum TypeTransaction
    {
        Achat,
        Vente
    }

    /// <summary>Statut d'une transaction</summary>
    public enum StatutTransaction
    {
        En_attente,
        Confirmee,
        Annulee
    }
}
