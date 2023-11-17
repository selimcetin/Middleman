
namespace Middleman_1
{
    public enum GameState
    {
        Input,
        TurnStart,
        Buying_Product_Selection,
        Buying_Amount,
        Selling_Product_Selection,
        Selling_Amount,
        UpgradeStorage_Amount,
        Transaction,
        TurnEnd,
        GameEnd
    }

    public enum TransactionType
    {
        Buying,
        Selling,
        StorageUpgrade
    }
}