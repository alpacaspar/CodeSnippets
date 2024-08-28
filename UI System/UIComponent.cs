using UnityEngine;

namespace EW.FloodMenu
{
    /// <summary>
    /// Wrapper class for all types of Menu, useful for storing references in code or in the inspector.
    /// </summary>
    public abstract class UIComponent : MonoBehaviour
    {
        public bool Visible => gameObject.activeSelf;
        
        public void Toggle(bool _value)
        {
            gameObject.SetActive(_value);
        }
    }
}
