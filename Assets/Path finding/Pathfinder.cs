using Bot;
using Pathfinding.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using WorldGeneration.Layers;

namespace Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        public Vector3Int startCoordinates;
        //public Vector3Int StartCoordinates { get { return startCoordinates; } }

        public Vector3Int destinationCoordinates;
        //public Vector3Int DestinationCoordinates { get { return destinationCoordinates; } }

        Node startNode;
        Node destinationNode;
        Node currentSearchNode;

        Queue<Node> frontier = new Queue<Node>();
        Dictionary<Vector3Int, Node> reached = new Dictionary<Vector3Int, Node>();

        public List<Vector3Int> directions = new List<Vector3Int>();
        GridManager gridManager;
        Dictionary<Vector3Int, Node> grid = new Dictionary<Vector3Int, Node>();
        Vector3Int botPos = new Vector3Int();
        public float checkTimer = 5;
        float timeSpend;
        public GameWorld world;
        void Start()
        {
            world = FindObjectOfType<GameWorld>();
        }
        private void Update()
        {
            
            timeSpend += Time.deltaTime;
            if (timeSpend > checkTimer&& world.toDoList[0]!=null)
            {
                botPos = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y - 1), Mathf.FloorToInt(transform.position.z));
                startCoordinates = botPos;
                PathFind();
                timeSpend = 0;
            }
        }
        public void PathFind()
        {
            directions.Clear();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3Int stop = new Vector3Int(0, 0, 0);
                        Vector3Int point = new Vector3Int(x, y, z);
                        if (stop != point)
                        {
                            directions.Add(point);
                        }
                    }
                }
            }
            gridManager = FindObjectOfType<GridManager>();
            if (gridManager != null)
            {

                Vector3Int botPos2 = new Vector3Int(Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(30), Mathf.FloorToInt(transform.position.z));
                grid = gridManager.Grid;
                startNode = grid[botPos];
                destinationNode = grid[world.toDoList[0]];
                destinationCoordinates = world.toDoList[0];

                /* {
                     destinationNode = grid[botPos];

                 }*/
                /* else
                 {
                     destinationNode = grid[world.toDoList[0]];
                     destinationCoordinates = world.toDoList[0];
                 }*/

            }
            GetNewPath(startCoordinates);
            gameObject.GetComponent<BotsControl>().RecalculatePath(true);
        }

        public void ResetNodes()
        {
            foreach (KeyValuePair<Vector3Int, Node> entry in grid)
            {
                entry.Value.connectedTo = null;
                entry.Value.isExplored = false;
                entry.Value.isPath = false;
            }
        }

        public List<Node> GetNewPath()
        {
            return GetNewPath(startCoordinates);
        }

        public List<Node> GetNewPath(Vector3Int coordinates)
        {
            ResetNodes();
            BreadthFirstSearch(coordinates);
            return BuildPath();
        }
        void BreadthFirstSearch(Vector3Int coordinates)
        {
            startNode.isWalkable = true;
            destinationNode.isWalkable = true;

            frontier.Clear();
            reached.Clear();

            bool isRunning = true;

            frontier.Enqueue(grid[coordinates]);
            reached.Add(coordinates, grid[coordinates]);

            while (frontier.Count >= 0 && isRunning)
            {
                currentSearchNode = frontier.Dequeue();
                currentSearchNode.isExplored = true;
                ExploreNeighbors();
                if (currentSearchNode.coordinates == destinationCoordinates)
                {
                    isRunning = false;
                }
            }
        }
        void ExploreNeighbors()
        {
            List<Node> neighbors = new List<Node>();

            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighborCoords = currentSearchNode.coordinates + direction;

                if (grid.ContainsKey(neighborCoords))
                {
                    neighbors.Add(grid[neighborCoords]);
                }
            }

            foreach (Node neighbor in neighbors)
            {
                if (!reached.ContainsKey(neighbor.coordinates) && neighbor.isWalkable)
                {
                    neighbor.connectedTo = currentSearchNode;
                    reached.Add(neighbor.coordinates, neighbor);
                    frontier.Enqueue(neighbor);
                }
            }
        }


        List<Node> BuildPath()
        {
            List<Node> path = new List<Node>();
            Node currentNode = destinationNode;
            path.Add(currentNode);
            currentNode.isPath = true;

            while (currentNode.connectedTo != null)
            {
                currentNode = currentNode.connectedTo;
                path.Add(currentNode);
                currentNode.isPath = true;
            }

            path.Reverse();

            return path;
        }

        /*public bool WillBlockPath(Vector3Int coordinates)
        {
            if (grid.ContainsKey(coordinates))
            {
                bool previousState = grid[coordinates].isWalkable;

                grid[coordinates].isWalkable = false;
                List<Node> newPath = GetNewPath();
                grid[coordinates].isWalkable = previousState;

                if (newPath.Count <= 1)
                {
                    GetNewPath();
                    return true;
                }
            }

            return false;
        }*/

        public void NotifyReceivers()
        {
            BroadcastMessage("RecalculatePath", false, SendMessageOptions.DontRequireReceiver);
        }

    }
}
