namespace EW.Items
{
    using FloodMenu;
    
    public class InventoryCellData : MenuCellData
    {
        public readonly Item item;
        public readonly int amount;
        
        internal readonly InventoryMenu inventoryMenu;

        internal InventoryCellData(Item _item, int _amount, InventoryMenu _inventoryMenu)
        {
            item = _item;
            amount = _amount;

            inventoryMenu = _inventoryMenu;
        }
    }
}
