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

        public int layerNum;
        public LayerRenderer layerPrefab;
        public int ActiveLayer;
        private int activeLayerMax;
        private int activeLayerMin = 0;

        private Camera mainCamera;

        private void Awake()
        {
            activeLayerMax = layerNum;
            ActiveLayer = layerNum;
        }
        private void Start()
        {
            mainCamera = Camera.main;

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
            CheckInput();
        }


        private void CheckInput()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                bool isDestroing = Input.GetMouseButtonDown(0);
                Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if (Physics.Raycast(ray, out var hitInfo))
                {
                    Vector3 blockCenter;
                    if (isDestroing)
                    {
                        blockCenter = hitInfo.point - hitInfo.normal;
                    }
                    else
                    {
                        blockCenter = hitInfo.point + hitInfo.normal;
                    }
                    Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter);
                    int index = blockWorldPos.x + blockWorldPos.z * LayerRenderer.LayerWidth;
                    if (isDestroing)
                    {
                        LayerDatas[blockWorldPos.y].Renderer.DestroyBlock(index);
                    }
                    else
                    {

                        LayerDatas[blockWorldPos.y].Renderer.SpawnBlock(index);
                    }

                }
            }

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

