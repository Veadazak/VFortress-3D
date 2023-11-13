using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldGeneration.Layers;

namespace Bot
{
    public class BotSearching : MonoBehaviour
    {
        public float vRange = 50f;
        private void Update()
        {
            Collider[] inRange = Physics.OverlapSphere(transform.position, vRange);
            foreach (var hitCollider in inRange)
            {
                if (hitCollider.GetComponent<LayerRenderer>().BlocksData.GetInfo(BlockType.Stone) == true)
                {

                }
            }
        }
    }
}
