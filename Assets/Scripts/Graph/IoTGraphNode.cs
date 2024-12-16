using UnityEngine;

public class IoTGraphNode : MonoBehaviour
{
    [SerializeField] protected bool isLeafNode;
    public bool IsLeafNode => isLeafNode;
    public IoTGraphNode[] children;

    public bool HasChildren()
    {
        return children != null && children.Length > 0;
    }
}
