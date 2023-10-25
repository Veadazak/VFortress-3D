using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Test/Block info")]
public class BlockInfo : ScriptableObject
{
    public BlockType Type;
    public Vector2 textureNumber;
    public Vector2 topTextureNumber;
    public bool difTop;
    //public Vector2 P

    public AudioClip StepSound;
    public float TimeToBrak = 0.3f;
}