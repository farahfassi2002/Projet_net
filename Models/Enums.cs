namespace PortefeuilleInvestissement.Models
{
    public enum TypeActif
    {
        Action,
        CryptoMonnaie,
        ETF,
        Obligation
    }

    public enum TypeTransaction
    {
        Achat,
        Vente
    }

    public enum StatutTransaction
    {
        En_attente,
        Confirmee,
        Annulee
    }
}
