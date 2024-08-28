using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EW.Items
{
    using FloodMenu;
    using Input;
    using Weapons;

    public class InventoryMenu : Menu<InventoryCell, InventoryCellData>
    {
        /// <summary>
        /// A wrapper class that holds all functionality that has to do with the item context menu.
        /// </summary>
        [System.Serializable]
        private class ItemContextMenu
        {
            [SerializeField] private TextMeshProUGUI itemNameTextComponent;
            [SerializeField] private TextMeshProUGUI itemDescriptionTextComponent;

            internal void Refresh(InventoryCell _cell)
            {
                if (!_cell)
                {
                    return;
                }

                Item item = _cell.data.item;
                
                itemNameTextComponent.text = item.DisplayName;
                itemDescriptionTextComponent.text = item.Description;
            }
        }
        
        [SerializeField] private Inventory inventory; 
        [SerializeField] private ItemContextMenu itemContextMenu;
        [Space]
        [SerializeField] private PickupInteractable pickupInteractablePrefab;
        [SerializeField] private int amountOfCells = 4;

        private InventoryCell selectedCell;
        private readonly Dictionary<int, InventoryCell> storedItemDictionary = new();

        private InputMaster inputMaster;

        internal Inventory Inventory => inventory;
        
        private void Awake()
        {
            inputMaster = GameManager.Instance.InputMaster;

            for (int i = 1; i < amountOfCells; i++)
            {
                storedItemDictionary.Add(i, null);
            }
        }
        
        private void OnEnable()
        {
            SetInventoryData();

            if (Cells.Any())
            {
                Cells.First().Button.Select();
            }

            inputMaster.UI.Submit.performed += OnSubmitPerformed;
            inputMaster.UI.Store.performed += OnStorePerformed;
            inputMaster.UI.Context.performed += OnContextPerformed;
            
            InputPrompts.OnCurrentDeviceChanged += OnCurrentDeviceChanged;
        }

        private void OnDisable()
        {
            inputMaster.UI.Submit.performed -= OnSubmitPerformed;
            inputMaster.UI.Store.performed -= OnStorePerformed;
            inputMaster.UI.Context.performed -= OnContextPerformed;
            
            InputPrompts.OnCurrentDeviceChanged -= OnCurrentDeviceChanged;
        }
        
        private void OnCurrentDeviceChanged(InputDeviceType _device)
        {
            if (!Cells.Any())
            {
                return;
            }
            
            switch (InputPrompts.CurrentInputDeviceType)
            {
                case InputDeviceType.GAMEPAD:
                {
                    // If no button is selected, select the first interactable button.
                    Cells.First().Button.Select();
                
                    GameManager.EnableCursor(false);
                    break;
                }
                
                default:
                    GameManager.EnableCursor(true);
                    break;
            }
        }

        private void SetInventoryData()
        {
            List<InventoryCellData> data = new();
            
            foreach (KeyValuePair<Item, int> item in inventory.Items)
            {
                data.Add(new InventoryCellData(item.Key, item.Value, this));
            }
            
            SetData(data);
        }

        public void DropItem(int _amount)
        {
            if (!Cells.Any() || !selectedCell.Item.Droppable)
            {
                return;
            }
            
            inventory.Remove(selectedCell.Item, _amount);
            SpawnPickupInteractable(selectedCell.Item, _amount);
            
            // Before removing the cell that belongs to the item we dropped,
            // save the cell index so it can be used to select the next item when this one has been removed
            int selectedCellIndex = Cells.IndexOf(selectedCell);
            SetInventoryData();
            
            if (Cells.Any())
            {
                int nextCellIndex = Mathf.Clamp(selectedCellIndex, 0, Cells.Count - 1);

                OnCellSelected(Cells[nextCellIndex]);
            }
        }

        private void SpawnPickupInteractable(Item _item, int _amount = 1)
        {
            // TODO: Test if standing next to a collider will drop the interactables into the void to never return again.
            Transform ownerTransform = inventory.Owner.transform;
            Vector3 spawnPosition = ownerTransform.position + ownerTransform.forward + ownerTransform.up;
            
            PickupInteractable interactable = Instantiate(pickupInteractablePrefab, spawnPosition, Quaternion.identity);
            interactable.Init(_item, _amount);
        }
        
        internal void OnCellSelected(InventoryCell _cell)
        {
            selectedCell = _cell;
            itemContextMenu.Refresh(selectedCell);
        }

        private void OnSubmitPerformed(InputAction.CallbackContext _context)
        {
            if (selectedCell)
            {
                inventory.Use(selectedCell.Item);
                selectedCell.Refresh();
            }
        }

        private void OnStorePerformed(InputAction.CallbackContext _context)
        {
            if (selectedCell && !selectedCell.Item.Model.GetComponent<Weapon>())
            {
                int index = int.Parse(_context.action.activeControl.name);

                if (storedItemDictionary[index] == selectedCell)
                {
                    storedItemDictionary[index] = null;
                }
                else
                {
                    storedItemDictionary[index] = selectedCell;
                }

                inventory.Use(selectedCell.Item, index);
            }
        }
        
        private void OnContextPerformed(InputAction.CallbackContext _context)
        {
            if (selectedCell)
            {
                DropItem(1);
            }
        }
    }
}
