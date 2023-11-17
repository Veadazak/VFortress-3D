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
        public GameWorld World;
        public Vector3Int targetPos = new Vector3Int();
        public NavMeshAgent NavM;
        public float checkTime = 2f;
        float actTime = 0;
        private void Start()
           
        {
            //NavM = GetComponent<NavMeshAgent>();
            World = FindObjectOfType<GameWorld>();
        }
        private void Update()
        {
            
            actTime += Time.deltaTime;
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
                //NavM.SetDestination(targetPos);
            }            
        }
    }
}

