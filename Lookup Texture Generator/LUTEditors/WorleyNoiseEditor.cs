using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace EW.LUTGenerator
{
    internal class WorleyNoiseEditor : ILUTEditor
    {
        private const int ComputeThreadGroupSize = 8;
        
        private ComputeShader compute;
        private RenderTexture renderTexture;
        
        private List<ComputeBuffer> buffersToRelease;

        private int seed;
        private int cellDensity = 1;
        private float lacunarity = 2;
        private float persistence = 0.5f;
        
        private int tile = 1;
        
        private static readonly int Resolution = Shader.PropertyToID("Resolution");
        private static readonly int Result = Shader.PropertyToID("Result");
        
        private static readonly int NumCellsA = Shader.PropertyToID("NumCellsA");
        private static readonly int NumCellsB = Shader.PropertyToID("NumCellsB");
        private static readonly int NumCellsC = Shader.PropertyToID("NumCellsC");
        private static readonly int Persistence = Shader.PropertyToID("Persistence");
        
        private static readonly int Tile = Shader.PropertyToID("Tile");
        
        private static readonly int MinMax = Shader.PropertyToID("MinMax");
        
        public void ShowFields()
        {
            seed = EditorGUILayout.IntField("Seed", seed);
            cellDensity = Mathf.Max(EditorGUILayout.IntField("Cell Density", cellDensity), 1);
            lacunarity = Mathf.Max(EditorGUILayout.FloatField("Lacunarity", lacunarity), 1);
            persistence = EditorGUILayout.Slider("Persistence", persistence, 0f, 1f);
            tile = Mathf.Max(EditorGUILayout.IntField("Tile", tile), 1);
        }

        public Texture2D ToTexture2D(int _width, int _height, FilterMode _filterMode = FilterMode.Bilinear)
        {
            compute ??= (ComputeShader)EditorGUIUtility.Load("LUTGenerator/WorleyCompute.compute");
            
            renderTexture = new RenderTexture(_width, _height, GraphicsFormat.R32G32B32A32_SFloat, 0)
            {
                enableRandomWrite = true,
            };
            renderTexture.Create();

            buffersToRelease = new List<ComputeBuffer>();
            
            compute.SetFloat(Persistence, persistence);
            compute.SetVector(Resolution, new Vector4(renderTexture.width, renderTexture.height));

            // Noise Generation Kernel
            compute.SetTexture(0, Result, renderTexture);
            ComputeBuffer minMaxBuffer = CreateBuffer (new uint[] { uint.MaxValue, 0 }, sizeof(uint), "MinMax");
            
            System.Random random = new(seed);
            
            CreateWorleyPointsBuffer(random, cellDensity, "PointsA");
            CreateWorleyPointsBuffer(random, Mathf.CeilToInt(cellDensity * lacunarity), "PointsB");
            CreateWorleyPointsBuffer(random, Mathf.CeilToInt(cellDensity * lacunarity * lacunarity), "PointsC");

            compute.SetInt(NumCellsA, cellDensity);
            compute.SetInt(NumCellsB, Mathf.CeilToInt(cellDensity * lacunarity));
            compute.SetInt(NumCellsC, Mathf.CeilToInt(cellDensity * lacunarity * lacunarity));
            compute.SetInt(Tile, tile);
            compute.SetTexture(0, Result, renderTexture);
            
            int numThreadGroupsX = Mathf.CeilToInt(renderTexture.width / (float)ComputeThreadGroupSize);
            int numThreadGroupsY = Mathf.CeilToInt(renderTexture.height / (float)ComputeThreadGroupSize);
            compute.Dispatch(0, numThreadGroupsX, numThreadGroupsY, 1);

            // Normalization Kernel
            compute.SetBuffer(1, MinMax, minMaxBuffer);
            compute.SetTexture(1, Result, renderTexture);
            
            compute.Dispatch(1, numThreadGroupsX, numThreadGroupsY, 1);

            foreach (ComputeBuffer buffer in buffersToRelease)
            {
                buffer.Release();
            }
            
            Texture2D result = new(_width, _height, TextureFormat.RGBA32, false)
            {
                filterMode = _filterMode,
            };
            
            RenderTexture lastRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            result.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            result.Apply();
            RenderTexture.active = lastRenderTexture;
            
            return result;
        }
        
        private ComputeBuffer CreateBuffer(System.Array data, int stride, string bufferName, int kernel = 0)
        {
            ComputeBuffer buffer = new(data.Length, stride, ComputeBufferType.Structured);
            buffersToRelease.Add(buffer);
            
            buffer.SetData(data);
            compute.SetBuffer(kernel, bufferName, buffer);
            
            return buffer;
        }
        
        void CreateWorleyPointsBuffer(System.Random random, int numCellsPerAxis, string bufferName)
        {
            Vector2[] points = new Vector2[numCellsPerAxis * numCellsPerAxis];
            float cellSize = 1f / numCellsPerAxis;

            for (int x = 0; x < numCellsPerAxis; x++)
            {
                for (int y = 0; y < numCellsPerAxis; y++)
                {
                        float randomX = (float)random.NextDouble();
                        float randomY = (float)random.NextDouble();
                        
                        Vector2 randomOffset = new Vector2(randomX, randomY) * cellSize;
                        Vector2 cellCorner = new Vector2(x, y) * cellSize;

                        int index = x + y * numCellsPerAxis;
                        points[index] = cellCorner + randomOffset;
                }
            }

            CreateBuffer(points, sizeof(float) * 2, bufferName);
        }
    }
}
