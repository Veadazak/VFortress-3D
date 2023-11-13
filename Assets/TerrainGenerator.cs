/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldGeneration.Layers
{
    public class TerrainGenerator : MonoBehaviour
    {
        private GameWorld parentWorld;

        private LayerData lowerLayer; 
        public LayerData LayerData;

        private void Start()
        {
            parentWorld = GetComponent<GameWorld>();
        }
        public void GenerateTerrain()
        {
            for (int y = 0; y <= parentWorld.activeLayerMax; y++)
            {
                for (int x = 0; x < LayerRenderer.LayerWidth; x++)
                {
                    for (int z = 0; z < LayerRenderer.LayerWidth; z++)
                    {
                        int index = x + z * LayerRenderer.LayerWidthSq;

                        if (y == 0 || y == 1)
                        {
                            parentWorld.LayerDatas[y].Blocks[index] = BlockType.Dirt;
                        }
                        if (y > 2)
                        {
                            parentWorld.LayerDatas[LayerData.LayerNumber -1] = lowerLayer;
                            {
                                if (lowerLayer.Blocks[index] == BlockType.Dirt)
                                {

                                    if (Random.Range(0, y) > 5)
                                    {
                                        parentWorld.LayerDatas[y].Blocks[index] = BlockType.Air;
                                    }
                                    else
                                    {
                                        parentWorld.LayerDatas[y].Blocks[index] = BlockType.Dirt;
                                    }
                                }
                            }
                        }
                        else { parentWorld.LayerDatas[y].Blocks[index] = BlockType.Air; }
                    }
                }
                parentWorld.LayerDatas[y].Renderer.RegenerateMesh();
            }
        }
    }

}
*/