using UnityEngine;

namespace EW.FloodMenu
{
    /// <summary>
    /// This class is used on MenuCell prefabs to dynamically fill in content such as text or sprites.
    /// </summary>
    /// <see cref="Menu{TCell,TData}"/>
    /// <see cref="MenuCellData"/>
    public abstract class MenuCell<TData> : UIComponent where TData : MenuCellData
    {
        public TData data;

        public abstract void Refresh();
    }
}
