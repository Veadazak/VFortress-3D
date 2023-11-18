using Pathfinding.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Profiling;
using UnityEngine;
using WorldGeneration;

namespace WorldGeneration.Layers
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class LayerRenderer : MonoBehaviour
    {
        [SerializeField] public const int LayerWidth = 20;
        public const int LayerWidthSq = LayerWidth * LayerWidth;
        public GridManager gridManager;

        private Mesh blockMesh;
        private List<Vector3> verticies = new List<Vector3>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<int> triangles = new List<int>();

        public BlockDatabase BlocksData;
        public LayerData LayerData;
        public GameWorld ParentWorld;

        private LayerData upperLayer;
        private LayerData lowerLayer;
        public int UP;
        public int DOWN;
        public bool activeLayer = false;
        private static ProfilerMarker GenerationMarker = new ProfilerMarker(ProfilerCategory.Loading, name: "Generating");

        private static ProfilerMarker MeshingMaker = new ProfilerMarker(ProfilerCategory.Loading, "Meshing");

        void Start()
        {
            gridManager = FindObjectOfType<GridManager>();
            if (LayerData.LayerNumber > 0)
            {
                lowerLayer = ParentWorld.LayerDatas[LayerData.LayerNumber - 1];
                DOWN = lowerLayer.LayerNumber;
            }
            if (LayerData.LayerNumber < ParentWorld.activeLayerMax)
            {
                upperLayer = ParentWorld.LayerDatas[LayerData.LayerNumber + 1];
                UP = upperLayer.LayerNumber;
            }
            blockMesh = new Mesh();
            RegenerateMesh();
            GetComponent<MeshFilter>().sharedMesh = blockMesh;
        }        
        public void ActivateLayer()
        {
            activeLayer = true;
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            RegenerateMesh();

        }
        public void DeactivateLayer()
        {
            activeLayer = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            RegenerateMesh();
        }
        public void SpawnBlock(int index)
        {
            if (LayerData.Blocks[index] == BlockType.Air)
            {
                LayerData.Blocks[index] = BlockType.Stone;
                RegenerateMesh();
            }
        }
        public void DestroyBlock(int index)
        {
            LayerData.Blocks[index] = BlockType.Air;
            RegenerateMesh();
        }

        public void RegenerateMesh()
        {

            MeshingMaker.Begin();
            verticies.Clear();
            uvs.Clear();
            triangles.Clear();
            for (int x = 0; x < LayerWidth; x++)
            {
                for (int z = 0; z < LayerWidth; z++)
                {
                    GenerateBlock(x, 0, z);
                }
            }
            blockMesh.triangles = Array.Empty<int>();
            blockMesh.vertices = verticies.ToArray();
            blockMesh.uv = uvs.ToArray();
            blockMesh.triangles = triangles.ToArray();

            blockMesh.Optimize();

            blockMesh.RecalculateBounds();
            blockMesh.RecalculateNormals();
            GetComponent<MeshCollider>().sharedMesh = blockMesh;
            GetComponent<MeshFilter>().mesh = blockMesh;
            MeshingMaker.End();
        }
        //----------------------------- Generate block ---------------
        private void GenerateBlock(int x, int y, int z)
        {

            Vector3Int blockPosition = new Vector3Int(x, y, z);
            Vector3Int blPos = new Vector3Int(x, LayerData.LayerNumber, z);
            Vector2Int bPosition = new Vector2Int(x, z);

            BlockType blockType = GetBlockAtPosition(blockPosition);
            
            if (blockType == BlockType.Air)
            {                
                gridManager.RemoveFromList(blPos, false);
                return;
            }


            if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0)
            {
                GenerateRightSide(blockPosition);
                AddUvs(blockType, Vector2Int.right);
            }
            if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
            {
                GenerateLeftSide(blockPosition);
                AddUvs(blockType, Vector2Int.right);
            }
            if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0 || GetBlockAtPosition(blockPosition + Vector3Int.up) == BlockType.Hiden)
            {
                if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)
                {
                    GenerateTopSide(blockPosition);
                    AddUvs(blockType, Vector2Int.up);
                    gridManager.AddToList(blPos, false);
                }
                else
                {
                    GenerateTopSide(blockPosition);
                    AddUvs(BlockType.Hiden, Vector2Int.up);
                }
            }
            if(GetBlockAtPosition(blockPosition + Vector3Int.up)!=0)
            {
                gridManager.RemoveFromList(blPos, true);
            }
            /*if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)
            {
                GenerateDownSide(blockPosition);
                AddUvs(blockType, Vector2Int.down);
            }*/

            if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
            {
                GenerateFrontSide(blockPosition);
                AddUvs(blockType, Vector2Int.right);
            }
            if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)
            {
                GenerateBackSide(blockPosition);
                AddUvs(blockType, Vector2Int.right);
            }
        }

        //----------------------------- Checking bloks around to reduce meshes ---------------
        private BlockType GetBlockAtPosition(Vector3Int blockPosition)
        {
            if (blockPosition.x >= 0 && blockPosition.x < LayerWidth &&
                blockPosition.y == 0 &&
                blockPosition.z >= 0 && blockPosition.z < LayerWidth)
            {
                int index = blockPosition.x + blockPosition.z * LayerWidthSq;
                return LayerData.Blocks[index];
            }
            if (blockPosition.y < 0 )
            {
                if (lowerLayer == null) { return BlockType.Air; }
                blockPosition.y++;
                int index = blockPosition.x + blockPosition.z * LayerWidthSq;
                return lowerLayer.Blocks[index];
            }
            if (blockPosition.y > 0 )
            {
                if (upperLayer == null) { return BlockType.Air; }
                blockPosition.y--;
                int index = blockPosition.x + blockPosition.z * LayerWidthSq;

                if (upperLayer.Renderer.activeLayer == false && upperLayer.Blocks[index] != BlockType.Air && upperLayer != null)
                {
                    return BlockType.Hiden;
                }
                else
                {
                    return upperLayer.Blocks[index];
                }
            }
            return BlockType.Air;
        }

        //----------------------------- Generate sides ---------------
        private void GenerateLeftSide(Vector3Int blockPosition)
        {
            verticies.Add((new Vector3(0, 1, 0) + blockPosition));
            verticies.Add((new Vector3(0, 0, 0) + blockPosition));
            verticies.Add((new Vector3(0, 1, 1) + blockPosition));
            verticies.Add((new Vector3(0, 0, 1) + blockPosition));
            AddLastVerticiesSquare();
        }
        private void GenerateRightSide(Vector3Int blockPosition)
        {
            verticies.Add((new Vector3(1, 1, 1) + blockPosition));
            verticies.Add((new Vector3(1, 0, 1) + blockPosition));
            verticies.Add((new Vector3(1, 1, 0) + blockPosition));
            verticies.Add((new Vector3(1, 0, 0) + blockPosition));
            AddLastVerticiesSquare();
        }
        private void GenerateTopSide(Vector3Int blockPosition)
        {
            verticies.Add((new Vector3(0, 1, 0) + blockPosition));
            verticies.Add((new Vector3(0, 1, 1) + blockPosition));
            verticies.Add((new Vector3(1, 1, 0) + blockPosition));
            verticies.Add((new Vector3(1, 1, 1) + blockPosition));
            AddLastVerticiesSquare();
        }
        private void GenerateDownSide(Vector3Int blockPosition)
        {
            verticies.Add((new Vector3(1, 0, 1) + blockPosition));
            verticies.Add((new Vector3(0, 0, 1) + blockPosition));
            verticies.Add((new Vector3(1, 0, 0) + blockPosition));
            verticies.Add((new Vector3(0, 0, 0) + blockPosition));
            AddLastVerticiesSquare();
        }
        private void GenerateFrontSide(Vector3Int blockPosition)
        {
            verticies.Add((new Vector3(0, 1, 1) + blockPosition));
            verticies.Add((new Vector3(0, 0, 1) + blockPosition));
            verticies.Add((new Vector3(1, 1, 1) + blockPosition));
            verticies.Add((new Vector3(1, 0, 1) + blockPosition));
            AddLastVerticiesSquare();
        }
        private void GenerateBackSide(Vector3Int blockPosition)
        {
            verticies.Add((new Vector3(1, 1, 0) + blockPosition));
            verticies.Add((new Vector3(1, 0, 0) + blockPosition));
            verticies.Add((new Vector3(0, 1, 0) + blockPosition));
            verticies.Add((new Vector3(0, 0, 0) + blockPosition));
            AddLastVerticiesSquare();
        }
        private void AddLastVerticiesSquare()
        {
            triangles.Add(verticies.Count - 4);
            triangles.Add(verticies.Count - 3);
            triangles.Add(verticies.Count - 2);

            triangles.Add(verticies.Count - 3);
            triangles.Add(verticies.Count - 1);
            triangles.Add(verticies.Count - 2);
        }
        private void AddUvs(BlockType blockType, Vector2Int normal)
        {
            BlockInfo info = BlocksData.GetInfo(blockType);
            if (info != null)
            {
                if (info.difTop)
                {
                    if (normal == Vector2Int.up)
                    {
                        uvs.Add(new Vector2((info.topTextureNumber.x - 1) / 16, (info.topTextureNumber.y - 1) / 16));
                        uvs.Add(new Vector2((info.topTextureNumber.x - 1) / 16, info.topTextureNumber.y / 16));
                        uvs.Add(new Vector2(info.topTextureNumber.x / 16, (info.topTextureNumber.y - 1) / 16));
                        uvs.Add(new Vector2(info.topTextureNumber.x / 16, info.topTextureNumber.y / 16));
                    }
                    else
                    {
                        uvs.Add(new Vector2((info.textureNumber.x) / 16, (info.textureNumber.y) / 16));
                        uvs.Add(new Vector2((info.textureNumber.x) / 16, (info.textureNumber.y - 1) / 16));
                        uvs.Add(new Vector2((info.textureNumber.x - 1) / 16, (info.textureNumber.y) / 16));
                        uvs.Add(new Vector2((info.textureNumber.x - 1) / 16, (info.textureNumber.y - 1) / 16));
                    }
                }
                else
                {
                    uvs.Add(new Vector2((info.textureNumber.x) / 16, (info.textureNumber.y) / 16));
                    uvs.Add(new Vector2((info.textureNumber.x) / 16, (info.textureNumber.y - 1) / 16));
                    uvs.Add(new Vector2((info.textureNumber.x - 1) / 16, (info.textureNumber.y) / 16));
                    uvs.Add(new Vector2((info.textureNumber.x - 1) / 16, (info.textureNumber.y - 1) / 16));
                }


            }

            else
            {
                uvs.Add(new Vector2(160f / 256, 208f / 256));
                uvs.Add(new Vector2(160f / 256, 224f / 256));
                uvs.Add(new Vector2(176f / 256, 208f / 256));
                uvs.Add(new Vector2(176f / 256, 224f / 256));
            }
        }
    }
}

