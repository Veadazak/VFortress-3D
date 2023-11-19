using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pathfinding
{
    [System.Serializable]
    public class Node
    {
        public Vector3Int coordinates;
        public bool isWalkable = true;
        public bool isExplored;
        public bool isPath;
        public Node connectedTo;

        public Node(Vector3Int coordinates, bool isExplored)
        {
            this.coordinates = coordinates;
            this.isExplored = isExplored;
        }
    }
}

