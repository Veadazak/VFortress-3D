using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pathfinding
{
    [System.Serializable]
    public class Node
    {
        public Vector3Int coordinates;
        public bool isWalkable;
        public bool isExplorable;
        public bool isPath;
        public Node connectedTo;

        public Node(Vector3Int coordiantes, bool isExplorable)
        {
            this.coordinates = coordiantes;
            this.isExplorable = isExplorable;
        }
    }
}

