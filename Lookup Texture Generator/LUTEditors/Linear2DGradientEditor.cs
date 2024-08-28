using UnityEditor;
using UnityEngine;

namespace EW.LUTGenerator
{
    internal class Linear2DGradientEditor : ILUTEditor
    {
        private Gradient2D gradient2D;
        private Gradient gradient1 = new();
        private Gradient gradient2 = new();
        private BlendMode blendMode;    
        
        public Linear2DGradientEditor()
        {
            gradient2D = new Gradient2D(gradient1, gradient2);
        }
            
        public void ShowFields()
        {
            gradient1 = EditorGUILayout.GradientField("Gradient A", gradient1);
            gradient2 = EditorGUILayout.GradientField("Gradient B", gradient2);
            blendMode = (BlendMode)EditorGUILayout.EnumPopup("Blend Mode", blendMode);
            
            gradient2D = new Gradient2D(gradient1, gradient2);
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
                    result.SetPixel(x, y, gradient2D.Evaluate((float)x / _width, (float)y / _height, blendMode));
                }
            }

            result.Apply();

            return result;
        }
    }
}
