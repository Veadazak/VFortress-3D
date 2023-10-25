using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using WorldGeneration.Layers;

namespace WorldGeneration
{
    public class GameWorld : MonoBehaviour
    {
        public List<LayerData> LayerDatas = new List<LayerData>();

        public int layerNum = 5;
        public LayerRenderer layerPrefab;
        public int ActiveLayer = 4;
        private int activeLayerMax = 4;
        private int activeLayerMin = 0;

        private void Start()
        {
            for (int i = 0; i <= layerNum; i++)
            {
                Vector3Int position = new Vector3Int(0, i, 0);
                var layer = Instantiate(layerPrefab, position, Quaternion.identity);

                LayerData layerData = new LayerData();
                layerData.Blocks = GenerateTerrain();
                layerData.LayerNumber = i;
                layerData.Renderer = layer;
                layer.LayerData = layerData;
                layer.ParentWorld = this;
                LayerDatas.Add(layerData);
            }

        }

        public BlockType[] GenerateTerrain()
        {
            var result = new BlockType[LayerRenderer.LayerWidth * LayerRenderer.LayerWidth];
            for (int x = 0; x < LayerRenderer.LayerWidth; x++)
            {
                for (int z = 0; z < LayerRenderer.LayerWidth; z++)
                {
                    int index = x + z * LayerRenderer.LayerWidth;
                    result[index] = BlockType.Dirt;
                }
            }
            return result;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ActiveLayer++;
                if (ActiveLayer >= activeLayerMax) { ActiveLayer = activeLayerMax; }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ActiveLayer--;
                if (ActiveLayer <= activeLayerMin) { ActiveLayer = activeLayerMin; }
            }
        }

    }
}

