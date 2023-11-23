using Pathfinding.Grid;
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
        GridManager gridManager;
        public GameObject bot;
        List<Vector3Int> coosrdiantesForBot = new List<Vector3Int>();



        private Camera mainCamera;

        private void Awake()
        {
            layerNum =  (int)Generator.BaseHeight;
            gridManager = FindObjectOfType<GridManager>();
            
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
        public void InstBot(Vector3Int position)
        {
            Instantiate(bot, position, Quaternion.identity);   
        }

        private void Update()
        {
            CheckInput();
            if (LayerDatas[0].Renderer.activeLayer != true)
            {
                LayerActivation();
                coosrdiantesForBot = gridManager.coordinates;
                for (int i = 0; i < 2; i++)
                {
                    Vector3Int prePos = coosrdiantesForBot[Random.Range(0, coosrdiantesForBot.Count)];
                    Vector3Int finPos = new Vector3Int(prePos.x, prePos.y+1, prePos.z);
                    Instantiate(bot,finPos ,Quaternion.identity);
                }
            }
        }

        private void CheckInput()
        {
            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButtonDown(1))
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
                        gridManager.PathSearch();
                        /*LayerDatas[blockWorldPos.y].Renderer.DestroyBlock(index);
                        if (LayerDatas[blockWorldPos.y - 1] != null)
                        {
                            LayerDatas[blockWorldPos.y - 1].Renderer.RegenerateMesh();
                        }
                        else { return; }*/
                    }
                    else
                    {
                        LayerDatas[blockWorldPos.y].Renderer.SpawnBlock(index);
                        if (LayerDatas[blockWorldPos.y-1] != null)
                        {
                            LayerDatas[blockWorldPos.y-1].Renderer.RegenerateMesh();
                            gridManager.PathSearch();
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
                gridManager.PathSearch();
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

