using UnityEditor;
using UnityEngine;

namespace EW.LUTGenerator
{
    internal class RadialGradientEditor : ILUTEditor
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

            Vector2 center = new Vector2(_width, _height) * 0.5f;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / Mathf.Max(_width, _height);
                    result.SetPixel(x, y, gradient.Evaluate(distance));
                }
            }

            result.Apply();

            return result;
        }
    }
}
