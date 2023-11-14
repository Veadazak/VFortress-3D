using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Bot
{
    public class BotState : MonoBehaviour
    {
        [SerializeField] Need Hunger ;
        [SerializeField] Need Sleep;
        [SerializeField] Need Mental;
        
            

    }
    [System.Serializable]
    public class Need
    {
        [HideInInspector]
        public float curValue;
        public float maxValue;
        public float startValue;
        public float regenRate;
        public float decayRate;
        //public Image uiBar;
        public void Add(float amount)
        {
            curValue = Mathf.Min(curValue + amount, maxValue);
        }

        public void Substract(float amount)
        {
            curValue = Mathf.Max(curValue - amount, 0.0f);
        }
    }
}

