using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Pathfinding.Grid
{
    public class GridManager : MonoBehaviour
    {
        Dictionary<Vector3Int, Node> grid = new Dictionary<Vector3Int, Node>();
        public Dictionary<Vector3Int, Node> Grid { get { return grid; } }

        public List<Vector3Int> coordinates = new List<Vector3Int>();
        public Node GetNode(Vector3Int coordinates)
        {
            if (grid.ContainsKey(coordinates))
            {
                return grid[coordinates];
            }
            return null;
        }
        void CreateGrid()
        {   
            grid.Clear();
            foreach(Vector3Int vector in coordinates)
            {
                Vector3Int coordinates = vector;
                grid.Add(coordinates, new Node(coordinates, true));
            }
        }
        public void AddToList(Vector3Int summPos, bool isHiden)
        {
            Vector3Int lowerLVL = new Vector3Int(summPos.x, summPos.y - 1, summPos.z);
            Vector3Int upperLVL = new Vector3Int(summPos.x, summPos.y + 1, summPos.z);
            if (coordinates.Contains(summPos))
            {
                coordinates.Remove(summPos);
                coordinates.Add(summPos);
            }
            if (!coordinates.Contains(summPos) && !isHiden)
            {
                coordinates.Remove(lowerLVL);
                coordinates.Add(summPos);
            }
            if (coordinates.Contains(lowerLVL) && !coordinates.Contains(upperLVL))
            {
                coordinates.Remove(lowerLVL);
                coordinates.Add(summPos);
            }
            CreateGrid();
        }
        public void RemoveFromList(Vector3Int summPos, bool isHiden)
        {
            Vector3Int lowerLVL = new Vector3Int(summPos.x, summPos.y - 1, summPos.z);
            coordinates.Remove(summPos);
            if (isHiden)
            {
                coordinates.Remove(lowerLVL);
            }
            CreateGrid();
        }
        //------------------------------------for the future, to avoid blocks ------------------------
        /*public void BlockNode(Vector3Int coordinates)
        {
            if (grid.ContainsKey(coordinates))
            {
                grid[coordinates].isWalkable = false;
            }
        }*/
        //----------------------------------------------------------------------------------------------
    }
}

