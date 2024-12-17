using UnityEngine;
using System.Collections.Generic;

public class IoTGraphSearcher : MonoBehaviour
{
    public IoTGraphNode root;
    public MiniLM mlm;
    public List<string> dummyIoTNames;
    public List<string> dummyIoTActions;

    /*
     * 쿼리를 받았을 때:
     *   LeafNode가 아니면서 Child가 없는 녀석들은 skip
     *   LeafNode가 아니면서 Child를 가진 녀석들은 traverse
     *   LeafNode이면서 Child가 있는 녀석들은 일단 LeafNode만 고려
     */
    public void Query(string query)
    {
        Debug.Log(mlm.GetMostRelevant(query, dummyIoTNames));
        Debug.Log(mlm.GetMostRelevant(query, dummyIoTActions));
    }
}
