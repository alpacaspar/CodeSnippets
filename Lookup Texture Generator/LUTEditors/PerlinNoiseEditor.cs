using UnityEditor;
using UnityEngine;

namespace EW.LUTGenerator
{
    internal class PerlinNoiseEditor : ILUTEditor
    {
        private int seed;
        
        private float scale = 50;

        private int octaves = 1;
        private float lacunarity = 2;
        private float persistence = 0.5f;
        
        private Vector2 offset;
        
        public void ShowFields()
        {
            seed = EditorGUILayout.IntField("Seed", seed);
            EditorGUILayout.Space();
            scale = EditorGUILayout.FloatField("Scale", scale);
            EditorGUILayout.Space();
            octaves = EditorGUILayout.IntField("Octaves", octaves);
            lacunarity = EditorGUILayout.FloatField("Lacunarity", lacunarity);
            persistence = EditorGUILayout.FloatField("Persistence", persistence);
            EditorGUILayout.Space();
            offset = EditorGUILayout.Vector2Field("Offset", offset);
        }

        public Texture2D ToTexture2D(int _width, int _height, FilterMode _filterMode = FilterMode.Bilinear)
        {
            Texture2D result = new(_width, _height)
            {
                filterMode = _filterMode,
            };

            float[][] map = GeneratePerlinMap(new Vector2Int(_width, _height), offset, scale, octaves, lacunarity, persistence, seed);
            for (int x = 0; x < map.Length; x++)
            {
                for (int y = 0; y < map[x].Length; y++)
                {
                    result.SetPixel(x, y, new Color(map[x][y], map[x][y], map[x][y]));
                }
                
            }
            
            result.Apply();

            return result;
        }
        
        private static float[][] GeneratePerlinMap(Vector2Int _textureSize, Vector2 _offset, float _scale, int _octaves, float _lacunarity, float _persistence, int _seed)
        {
            float[][] result = new float[_textureSize.x][];
            for (int i = 0; i < _textureSize.x; i++)
            {
                result[i] = new float[_textureSize.y];
            }
            
            System.Random rng = new(_seed);
            Vector2[] offsets = new Vector2[_octaves];
            
            for (int i = 0; i < offsets.Length; i++)
            {
                float offsetX = rng.Next (-100000, 100000) + _offset.x;
                float offsetY = rng.Next (-100000, 100000) + _offset.y;
                offsets[i] = new Vector2(offsetX, offsetY);
            }
            
            float min = float.MaxValue;
            float max = float.MinValue;

            for (int x = 0; x < _textureSize.x; x++)
            {
                for (int y = 0; y < _textureSize.y; y++)
                {
                    float frequency = 1;
                    float amplitude = 1;
                    float noise = 0;
                    
                    for (int octave = 0; octave < _octaves; octave++)
                    {
                        float sampleX = x / _scale * frequency + offsets[octave].x;
                        float sampleY = y / _scale * frequency + offsets[octave].y;

                        float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
                        noise += perlinValue * amplitude;

                        amplitude *= _persistence;
                        frequency *= _lacunarity;
                    }
                    
                    max = Mathf.Max(noise, max);
                    min = Mathf.Min(noise, min);
                    
                    result[x][y] = noise;
                }                
            }

            for (int x = 0; x < _textureSize.x; x++)
            {
                for (int y = 0; y < _textureSize.y; y++)
                {
                    result[x][y] = Mathf.InverseLerp(min, max, result[x][y]);
                }
            }

            return result;
        }
    }
}
