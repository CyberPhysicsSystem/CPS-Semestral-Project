using UnityEngine;
using System.Collections.Generic;

public class IoTGraphSearcher : MonoBehaviour
{
    public IoTGraphNode root;
    public MiniLM mlm;
    public List<string> dummyIoTNames;
    public List<string> dummyIoTActions;

    /*
     * ������ �޾��� ��:
     *   LeafNode�� �ƴϸ鼭 Child�� ���� �༮���� skip
     *   LeafNode�� �ƴϸ鼭 Child�� ���� �༮���� traverse
     *   LeafNode�̸鼭 Child�� �ִ� �༮���� �ϴ� LeafNode�� ���
     */
    public void Query(string query)
    {
        Debug.Log(mlm.GetMostRelevant(query, dummyIoTNames));
        Debug.Log(mlm.GetMostRelevant(query, dummyIoTActions));
    }
}
