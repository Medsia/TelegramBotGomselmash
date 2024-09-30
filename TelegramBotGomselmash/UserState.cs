namespace TelegramBotGomselmash
{
    class UserState
    {
        public UserHistory UserHistory { get; set; }
        
    }
    enum UserHistory
    {
        Menu,
        ProductCategoryPhoto,
        ProductCategoryCatalog,
        SendingBroadcast,
        ChangingAdmin,
        Purchase,
        PurchaseVenicles,
        PurchaseSpares,
        Service
    }
}
