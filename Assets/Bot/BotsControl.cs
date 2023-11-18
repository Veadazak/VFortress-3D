using Pathfinding;
using Pathfinding.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using WorldGeneration.Layers;

namespace Bot
{
    public class BotsControl : MonoBehaviour
    {
        public float viewRange = 50f;

        [SerializeField] List<Node> path = new List<Node>();
        [SerializeField][Range(0f, 5f)] float speed = 1f;

        GridManager gridManager;
        Pathfinder pathfinder;

        private void Awake()
        {
            gridManager = GetComponent<GridManager>();
            pathfinder = GetComponent<Pathfinder>();
        }
        public void RecalculatePath(bool resetPath)
        {
            path.Clear();
            Vector3Int coordinates = new Vector3Int();

            if (resetPath)
            {
                coordinates = pathfinder.startCoordinates;
            }
            else
            {
                Vector3Int botPos = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));
                coordinates = botPos;
            }

            StopAllCoroutines();
           
            path = pathfinder.GetNewPath(coordinates);
            StartCoroutine(FollowPath());
        }


        void OnEnable()
        {
            StartCoroutine(FollowPath());
        }
        IEnumerator FollowPath()
        {
            for (int i = 1; i < path.Count; i++)
            {
                Vector3 startPosition = transform.position;
                Vector3 endPosition = path[i].coordinates+new Vector3Int(0,3/2,0);
                float travelPercent = 0f;

                transform.LookAt(endPosition);

                while (travelPercent < 1f)
                {
                    travelPercent += Time.deltaTime * speed;
                    transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                    yield return new WaitForEndOfFrame();
                }
            }

        }
        private void Update()
        {

            /*actTime += Time.deltaTime;
            if (actTime > checkTime)
            {
                float minDist = viewRange;
                for (int i = 0; i < World.toDoList.Count; i++)
                {
                    float ditance = Vector3.Distance(transform.position, World.toDoList[i]);
                    if (ditance < minDist)
                    {
                        minDist = ditance;
                        targetPos = World.toDoList[i];
                        if(ditance<2)
                        { 
                            int index = targetPos.x+targetPos.z*LayerRenderer.LayerWidthSq;
                            World.RemoveAtL(i);
                            //World.LayerDatas[targetPos.y].Renderer.DestroyBlock(index);
                        }                        
                    }
                    actTime = 0;
                }
            }         */
        }
    }
}

