using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EW.Items
{
    using Extensions;
    using FloodMenu;
    
    [RequireComponent(typeof(Button))]
    public partial class InventoryCell : MenuCell<InventoryCellData>
    {
        [SerializeField] private Image itemThumbnailImageComponent;
        [SerializeField] private TextMeshProUGUI displayNameTextComponent;
        [SerializeField] private TextMeshProUGUI valueTextComponent;
        [SerializeField] private TextMeshProUGUI amountHeldTextComponent;
        [SerializeField] private RectTransform itemContextMenuTransform;

        internal Item Item => data.item;
        internal int Amount => data.amount;
        
        public override void Refresh()
        {
            itemThumbnailImageComponent.sprite = data.item.Sprite;
            displayNameTextComponent.text = data.item.DisplayName;
            valueTextComponent.text = data.item.Sellable ? data.item.Value.ToString() : "--";
            amountHeldTextComponent.text = $"{data.amount}/{data.item.MaxCarryAmount}";
            
            itemContextMenuTransform.gameObject.SetActive(data.inventoryMenu.Inventory.Equipment.HasItemEquipped(Item));
        }
    }

    public partial class InventoryCell : ISelectHandler
    {
        private Button button;
        internal Button Button => gameObject.GetAndCacheComponent(ref button);
        
        public void OnSelect(BaseEventData _eventData)
        {
            data.inventoryMenu.OnCellSelected(this);
        }
    }
}
