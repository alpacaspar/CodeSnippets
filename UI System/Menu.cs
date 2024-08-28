using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EW.FloodMenu
{
    /// <summary>
    /// A class for creating dynamic and responsive menu's.
    /// </summary>
    /// <see cref="MenuCell{TData}"/>
    /// <see cref="MenuCellData"/>
    public abstract class Menu<TCell, TData> : UIComponent
        where TCell : MenuCell<TData>
        where TData : MenuCellData
    {
        [SerializeField] private RectTransform container;
        [SerializeField] private TCell cellPrefab;

        protected List<TCell> Cells { get; private set; }
        protected List<TData> Data { get; private set; }

        private Stack<TCell> cellPool;

        /// <summary>
        /// Set the data for the menu to use for creating cells.
        /// </summary>
        protected void SetData(List<TData> _data)
        {
            Data = _data;
            Refresh();
        }

        protected void SetData(params TData[] _data)
        {
            Data = _data.ToList();

            Refresh();
        }
        
        /// <summary>
        /// Refresh the menu, this creates new cells.
        /// </summary>
        private void Refresh()
        {
            Cells ??= new List<TCell>();
            cellPool ??= new Stack<TCell>();

            // Clear all cells already present.
            for (int i = Cells.Count - 1; i >= 0; i--)
            {
                Cells[i].gameObject.SetActive(false);
                cellPool.Push(Cells[i]);
                Cells.Remove(Cells[i]);
            }

            // Create new cells based on the set data.
            foreach (TData item in Data)
            {
                TCell newCell;
                
                if (cellPool.Count > 0)
                {
                    newCell = cellPool.Pop();
                    newCell.gameObject.SetActive(true);
                }
                else
                {
                    newCell = Instantiate(cellPrefab, container);
                }
                
                newCell.data = item;
                Cells.Add(newCell);
                
                newCell.Refresh();
            }
        }
    }
}
