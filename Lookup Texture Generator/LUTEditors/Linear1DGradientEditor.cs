using UnityEditor;
using UnityEngine;

namespace EW.LUTGenerator
{
    internal class Linear1DGradientEditor : ILUTEditor
    {
        private Gradient gradient = new();
            
        public void ShowFields()
        {
            gradient = EditorGUILayout.GradientField("Gradient", gradient);
        }

        public Texture2D ToTexture2D(int _width, int _height, FilterMode _filterMode = FilterMode.Bilinear)
        {
            Texture2D result = new(_width, _height)
            {
                filterMode = _filterMode,
            };

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    result.SetPixel(x, y, gradient.Evaluate((float)x / _width));
                }
            }

            result.Apply();

            return result;
        }
    }
}
