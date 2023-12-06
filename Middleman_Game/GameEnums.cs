namespace Middleman_Game
{
    public enum GameState
    {
        TurnStart,
        Menu,
        Buying_Product_Selection,
        Buying_Amount,
        Selling_Product_Selection,
        Selling_Amount,
        UpgradeStorage_Amount,
        Transaction,
        TurnEnd,
    }

    public enum TransactionType
    {
        Buying,
        Selling,
        StorageUpgrade
    }
}