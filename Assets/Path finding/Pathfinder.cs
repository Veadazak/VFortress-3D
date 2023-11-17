using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        public List<Vector3Int> lockedPoints = new List<Vector3Int>();
        public Vector3Int startingPoint;
        public List<Node> nodes = new List<Node>();
        [SerializeField] Node node;
        //public Vector3Int endPoint;

        private Vector3Int leftUp = new Vector3Int(-1, 0, 1);
        private Vector3Int up = new Vector3Int(0, 0, 1);
        private Vector3Int rightUp = new Vector3Int(1, 0, 1);
        private Vector3Int left = new Vector3Int(-1, 0, 0);
        private Vector3Int right = new Vector3Int(1, 0, 0);
        private Vector3Int leftDown = new Vector3Int(-1, 0, -1);
        private Vector3Int down = new Vector3Int(0, 0, -1);
        private Vector3Int rightDown = new Vector3Int(1, 0, -1);

        private void Start()
        {
            Debug.Log(node.coordinates);
            Debug.Log(node.isWalkable);
            nodes.Add(node);
        }

        public void PathFinding(Vector3Int endPoint)
        {
            startingPoint = Vector3Int.FloorToInt(transform.position);
            Vector3Int actPoint = startingPoint;
            for (int i = 0; i < 7; i++)
            {
                LookAround(actPoint);
            }
        }

        private void LookAround(Vector3Int actPoint)
        {
            if (!lockedPoints.Contains(actPoint + leftUp))
            {
                lockedPoints.Add(actPoint);
                
            }
            if (!lockedPoints.Contains(actPoint + up))
            {
                lockedPoints.Add(actPoint);
            }
            if (!lockedPoints.Contains(actPoint + rightUp))
            {
                lockedPoints.Add(actPoint);
            }
            if (!lockedPoints.Contains(actPoint + left))
            {
                lockedPoints.Add(actPoint);
            }
            if (!lockedPoints.Contains(actPoint + right))
            {
                lockedPoints.Add(actPoint);
            }
            if (!lockedPoints.Contains(actPoint + leftDown))
            {
                lockedPoints.Add(actPoint);
            }
            if (!lockedPoints.Contains(actPoint + down))
            {
                lockedPoints.Add(actPoint);
            }
            if (!lockedPoints.Contains(actPoint + rightDown))
            {
                lockedPoints.Add(actPoint);
            }
        }
    }
}

