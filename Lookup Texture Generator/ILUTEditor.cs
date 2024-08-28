using UnityEngine;

namespace EW.LUTGenerator
{
    internal interface ILUTEditor
    {
        void ShowFields();
        Texture2D ToTexture2D(int _width, int _height, FilterMode _filterMode = FilterMode.Bilinear);
    }
}
