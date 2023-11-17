using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor.Overlays;
using UnityEngine;
using WorldGeneration.Layers;

namespace WorldGeneration.Layers
{
    public class GameWorld : MonoBehaviour
    {
        public List<LayerData> LayerDatas = new List<LayerData>();
        public List<Vector3Int> toDoList = new List<Vector3Int>();
      
        public Dictionary<int,List<Vector2Int>> blocksPoisitions = new Dictionary<int, List<Vector2Int>>();

        public int layerNum;
        public LayerRenderer layerPrefab;
        public TerrainGenerator Generator;
        public int ActiveLayer;
        public int activeLayerMax;
        public int activeLayerMin = 0;



        private Camera mainCamera;

        private void Awake()
        {
            layerNum =  (int)Generator.BaseHeight;
            
        }
        private void Start()
        {
            activeLayerMax = layerNum;
            ActiveLayer = layerNum;
            mainCamera = Camera.main;
            Generator.Init();
            for (int y = 0; y <= layerNum; y++)
            {
                Vector3Int position = new Vector3Int(0, y, 0);
                var layer = Instantiate(layerPrefab, position, Quaternion.identity);

                LayerData layerData = new LayerData();
                layerData.Blocks = Generator.GenerateTerrain(y);
                layerData.LayerNumber = y;
                layerData.Renderer = layer;
                layer.LayerData = layerData;
                layer.ParentWorld = this;
                LayerDatas.Add(layerData);
            }
           
        }

        
        private int CheckAround(int x, int y, int z, int res)
        {
            int resultat = res;

            int p5 = x + z * LayerRenderer.LayerWidthSq; ; // - center


            if (x > 2 && x < LayerRenderer.LayerWidth - 2 &&
                z > 2 && z < LayerRenderer.LayerWidth - 2)
            {
                int p1 = (x - 1) + (z + 1) * LayerRenderer.LayerWidthSq;
                int p2 = (x) + (z + 1) * LayerRenderer.LayerWidthSq;
                int p3 = (x + 1) + (z + 1) * LayerRenderer.LayerWidthSq;
                int p4 = (x - 1) + (z) * LayerRenderer.LayerWidthSq;
                int p6 = (x + 1) + (z) * LayerRenderer.LayerWidthSq;
                int p7 = (x - 1) + (z - 1) * LayerRenderer.LayerWidthSq;
                int p8 = (x) + (z - 1) * LayerRenderer.LayerWidthSq;
                int p9 = (x + 1) + (z - 1) * LayerRenderer.LayerWidthSq;
                if (LayerDatas[y - 1].Blocks[p1] == BlockType.Dirt) { resultat++; }
                if (LayerDatas[y - 1].Blocks[p2] == BlockType.Dirt) { resultat++; }
                if (LayerDatas[y - 1].Blocks[p3] == BlockType.Dirt) { resultat++; }
                if (LayerDatas[y - 1].Blocks[p4] == BlockType.Dirt) { resultat++; }
                if (LayerDatas[y - 1].Blocks[p6] == BlockType.Dirt) { resultat++; }
                if (LayerDatas[y - 1].Blocks[p7] == BlockType.Dirt) { resultat++; }
                if (LayerDatas[y - 1].Blocks[p8] == BlockType.Dirt) { resultat++; }
                if (LayerDatas[y - 1].Blocks[p9] == BlockType.Dirt) { resultat++; }
            }

            if (x == 0 || x == LayerRenderer.LayerWidth)
            {
                resultat += 3;
            }
            if (z == 0 || z == LayerRenderer.LayerWidth)
            {
                resultat += 3;
            }
            Debug.Log(resultat);
            return resultat;

        }
        private void Update()
        {
            CheckInput();
            if (LayerDatas[0].Renderer.activeLayer != true)
            {
                LayerActivation();
            }

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
                        blockCenter = hitInfo.point - hitInfo.normal / 2;
                    }
                    else
                    {
                        blockCenter = hitInfo.point + hitInfo.normal / 2;
                    }
                    Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter);
                    if (LayerDatas[blockWorldPos.y] != null)
                    {
                        toDoList.Add(blockWorldPos);
                    }
                    int index = blockWorldPos.x + blockWorldPos.z * LayerRenderer.LayerWidthSq;
                    if (isDestroing)
                    {
                        LayerDatas[blockWorldPos.y].Renderer.DestroyBlock(index);
                        if (LayerDatas[blockWorldPos.y - 1] != null)
                        {
                            LayerDatas[blockWorldPos.y - 1].Renderer.RegenerateMesh();
                        }
                        else { return; }
                    }
                    else
                    {
                        LayerDatas[blockWorldPos.y].Renderer.SpawnBlock(index);
                        if (LayerDatas[blockWorldPos.y-1] != null)
                        {
                            LayerDatas[blockWorldPos.y-1].Renderer.RegenerateMesh();
                        }
                        else { return; }
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                for(int y = 0;y<LayerDatas.Count;y++)
                {
                    LayerDatas[y].Renderer.RegenerateMesh();
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ActiveLayer++;
                if (ActiveLayer >= activeLayerMax)
                {
                    ActiveLayer = activeLayerMax;
                }
                LayerActivation();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ActiveLayer--;
                if (ActiveLayer <= activeLayerMin)
                {
                    ActiveLayer = activeLayerMin;
                }
                LayerActivation();
            }
        }
        public void RemoveAtL(int i)
        {
            int index = toDoList[i].x + toDoList[i].z * LayerRenderer.LayerWidthSq;
            LayerDatas[toDoList[i].y].Renderer.DestroyBlock(index);
            if (LayerDatas[toDoList[i].y - 1] != null)
            {
                LayerDatas[toDoList[i].y - 1].Renderer.RegenerateMesh();
            }
            else { return; }
            toDoList.RemoveAt(i);
            
        }
        private void LayerActivation()
        {
            for (int i = activeLayerMax; i >= 0; i--)
            {
                if (ActiveLayer >= i)
                {
                    LayerDatas[i].Renderer.ActivateLayer();
                }
                else
                {
                    LayerDatas[i].Renderer.DeactivateLayer();
                }
            }
        }
    }
}

