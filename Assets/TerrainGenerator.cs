using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WorldGeneration.Layers
{
    [CreateAssetMenu(menuName = "Terrain generator")]
    public class TerrainGenerator : ScriptableObject
    {
        public float BaseHeight = 1;
        public NoiseOcataveSettings[] Octaves;
        [Serializable]
        public class NoiseOcataveSettings
        {
            public FastNoiseLite.NoiseType NoiseType;
            public float Frequency = 0.2f;
            public float Amplitude = 1;
        }
        private FastNoiseLite[] octaveNoises;

        public void Init()
        {
            octaveNoises = new FastNoiseLite[Octaves.Length];
            for (int i = 0; i < Octaves.Length; i++)
            {
                octaveNoises[i] = new FastNoiseLite();
                octaveNoises[i].SetNoiseType(Octaves[i].NoiseType);
                octaveNoises[i].SetFrequency(Octaves[i].Frequency);
            }
        }
        public BlockType[] GenerateTerrain(int y) // y - actual layer number
        {
            List<Vector2Int> blockPs = new List<Vector2Int>();
            GameWorld gameWorld = FindObjectOfType<GameWorld>();
            var result = new BlockType[LayerRenderer.LayerWidth * LayerRenderer.LayerWidth * LayerRenderer.LayerWidthSq];
            for (int x = 0; x < LayerRenderer.LayerWidth; x++)
            {
                for (int z = 0; z < LayerRenderer.LayerWidth; z++)
                {
                    float height = GetHeight(x, z);

                    int index = x + z * LayerRenderer.LayerWidthSq;
                    Vector2Int blockP = new Vector2Int(x,z);
                    if (y < height)
                    {
                        int r = Random.Range(0, 10);
                        if (r>8)
                        {
                            result[index] = BlockType.Stone;
                        }
                        else
                        {
                            result[index] = BlockType.Dirt;
                        }
                        blockPs.Add(blockP);
                        
                    }
                    else
                    {
                        result[index] = BlockType.Air;
                    }

                }
            }
            gameWorld.blocksPoisitions.Add(y, blockPs);
            return result;
        }
        private float GetHeight(float x, float y)
        {
            float result = BaseHeight;
            for (int i = 0; i < Octaves.Length; i++)
            {
                float noise = octaveNoises[i].GetNoise(x, y);
                result += noise * Octaves[i].Amplitude / 2;
            }
            return result;
        }
    }
}
