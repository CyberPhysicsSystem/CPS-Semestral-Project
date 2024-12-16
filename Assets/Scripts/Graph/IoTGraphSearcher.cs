using UnityEngine;

public class IoTGraphSearcher : MonoBehaviour
{
    public IoTGraphNode root;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * 쿼리를 받았을 때:
     *   LeafNode가 아니면서 Child가 없는 녀석들은 skip
     *   LeafNode가 아니면서 Child를 가진 녀석들은 traverse
     *   LeafNode이면서 Child가 있는 녀석들은 일단 LeafNode만 고려
     */
}
