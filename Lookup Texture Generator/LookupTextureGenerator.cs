using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EW.LUTGenerator
{
    public class LookupTextureGenerator : EditorWindow
    {
        private enum TextureType
        {
            GRADIENT_LINEAR_1D,
            GRADIENT_LINEAR_2D,
            GRADIENT_RADIAL,
            PERLIN_NOISE,
            WORLEY_NOISE,
        }

        // Editor
        private TextureType textureType;
        private Vector2 scrollPos;

        private readonly Dictionary<TextureType, ILUTEditor> editors = new()
        {
            { TextureType.GRADIENT_LINEAR_1D, new Linear1DGradientEditor() },
            { TextureType.GRADIENT_LINEAR_2D, new Linear2DGradientEditor() },
            { TextureType.GRADIENT_RADIAL, new RadialGradientEditor() },
            { TextureType.PERLIN_NOISE, new PerlinNoiseEditor() },
            { TextureType.WORLEY_NOISE, new WorleyNoiseEditor() },
        };
        
        private Texture2D lut;
        
        private Vector2Int dimensions = new(256, 256);
        private Vector2Int Dimensions
        {
            get => dimensions;
            set
            {
                if (value == dimensions)
                {
                    return;
                }
                
                dimensions = new Vector2Int(Mathf.Max(value.x, 1), Mathf.Max(value.y, 1));
                GenerateTexture();
            }
        }

        private FilterMode filterMode;

        [MenuItem("Eternal Wanderer/Lookup Texture Generator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(LookupTextureGenerator));
        }

        private void Awake()
        {
            GenerateTexture();
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            EditorGUI.BeginChangeCheck();
            textureType = (TextureType)EditorGUILayout.EnumPopup("Texture type", textureType);

            editors[textureType].ShowFields();

            if (EditorGUI.EndChangeCheck())
            {
                GenerateTexture();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(lut), GUILayout.Height(256));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            Dimensions = EditorGUILayout.Vector2IntField("Image dimensions", Dimensions);
            filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter mode", filterMode);
            
            if (GUILayout.Button("Export"))
            {
                SaveTexture2D(lut);
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void GenerateTexture()
        {
            lut = editors[textureType].ToTexture2D(Dimensions.x, Dimensions.y, filterMode);
        }

        private static void SaveTexture2D(Texture2D _texture)
        {
            string saveLocation = EditorUtility.SaveFilePanelInProject("Save as PNG", "texture", "png", "");
            
            if (string.IsNullOrEmpty(saveLocation))
            {
                return;
            }
            
            byte[] bytes = _texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(saveLocation, bytes);
            
            AssetDatabase.Refresh();
        }
    }
}
