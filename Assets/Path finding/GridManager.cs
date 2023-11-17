using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GridManager : MonoBehaviour
{

    public Dictionary<int, List<Vector2Int>> blocksCoordinates = new Dictionary<int, List<Vector2Int>>();

    public List<Vector2Int> check = new List<Vector2Int>();
    public List<Vector3Int> test = new List<Vector3Int>();
    public void AddToDictionary(int layer, List<Vector2Int> pos)
    {

        if (blocksCoordinates.ContainsKey(layer) == true)
        {
            blocksCoordinates.Remove(layer);
            blocksCoordinates.Add(layer, pos);
        }
        else blocksCoordinates.Add(layer, pos);
    }
    public void AddToList(Vector3Int summPos , bool isHiden)
    {
        Vector3Int lowerLVL = new Vector3Int(summPos.x, summPos.y - 1, summPos.z);
        Vector3Int upperLVL = new Vector3Int(summPos.x, summPos.y + 1, summPos.z);
        if (test.Contains(summPos))
        {
            test.Remove(summPos);
            test.Add(summPos);
        }

        if (!test.Contains(summPos) && !isHiden)
        {
            test.Remove(lowerLVL);
            test.Add(summPos);
        } 

        /*if (test.Contains(lowerLVL) && test.Contains(upperLVL))
        {
            test.Remove(lowerLVL);
            test.Remove(summPos);
        }*/
        if (test.Contains(lowerLVL) && !test.Contains(upperLVL))
        {
            test.Remove(lowerLVL);
            test.Add(summPos);
        }

    }
    public void RemoveFromList(Vector3Int summPos, bool isHiden)
    {

        test.Remove(summPos);

        //else test.Add(summPos);
    }
    public void RemoveHidenFromList(Vector3Int summPos, bool isHiden)
    {
        Vector3Int lowerLVL = new Vector3Int(summPos.x, summPos.y - 1, summPos.z);
        Vector3Int upperLVL = new Vector3Int(summPos.x, summPos.y + 1, summPos.z);
        test.Remove(lowerLVL);
        test.Remove(summPos);
    }

}
