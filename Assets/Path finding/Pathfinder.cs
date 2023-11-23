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
        
        public GameWorld world;
        void Start()
        {
            gridManager = FindObjectOfType<GridManager>();
            world = FindObjectOfType<GameWorld>();
            directions.Clear();
            directions.Add(new Vector3Int(-1, 0, 0));
            directions.Add(new Vector3Int(0, 0, 1));
            directions.Add(new Vector3Int(1, 0, 0));
            directions.Add(new Vector3Int(0, 0, -1));

            directions.Add(new Vector3Int(-1, 1, 0));
            directions.Add(new Vector3Int(0, 1, 1));
            directions.Add(new Vector3Int(1, 1, 0));
            directions.Add(new Vector3Int(0, 1, -1));

            directions.Add(new Vector3Int(-1, -1, 0));
            directions.Add(new Vector3Int(0, -1, 1));
            directions.Add(new Vector3Int(1, -1, 0));
            directions.Add(new Vector3Int(0, -1, -1));
            /*for (int x = -1; x <= 1; x++)
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
            }*/
        }
        private void Update()
        {            
            

            if( gridManager.gridGenerated == !gridManager.gridGenerated)
            {
                PathFind();
            }
        }
        public void PathFind()
        {
            botPos = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y - 1), Mathf.FloorToInt(transform.position.z));
            startCoordinates = botPos;
            
            if (gridManager != null)
            {                
                grid = gridManager.Grid;
                if(grid[botPos]==null)
                {
                    transform.position = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y+1), Mathf.FloorToInt(transform.position.z));
                }
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
        public void ResetNodes()
        {
            foreach (KeyValuePair<Vector3Int, Node> entry in grid)
            {
                entry.Value.connectedTo = null;
                entry.Value.isExplored = false;
                entry.Value.isPath = false;
            }
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

            while (frontier.Count > 0 && isRunning)
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
                if(currentNode.coordinates.y<currentNode.connectedTo.coordinates.y)
                {
                    Vector3Int higherPosCoordinate = currentNode.coordinates + Vector3Int.up;
                    Node getHigherPosition = new Node(higherPosCoordinate,true);
                    path.Add(getHigherPosition);
                }
                if(currentNode.coordinates.y>currentNode.connectedTo.coordinates.y)
                {
                    Vector3Int higherPosCoordinate = currentNode.connectedTo.coordinates + Vector3Int.up;
                    Node getHigherPosition = new Node(higherPosCoordinate,true);
                    path.Add(getHigherPosition);
                }

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
