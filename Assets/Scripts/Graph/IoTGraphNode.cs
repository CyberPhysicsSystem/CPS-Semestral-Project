using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class IoTGraphNode : MonoBehaviour
{
    [SerializeField] protected bool isLeafNode;
    public bool IsLeafNode => isLeafNode;
    public IoTGraphNode parent;
    public IoTGraphNode[] children;

    void Awake()
    {
        foreach (var child in children)
            child.SetParent(this);
    }

    public bool HasChildren()
    {
        return children != null && children.Length > 0;
    }

    public void SetParent(IoTGraphNode ign)
    {
        parent = ign;
    }

    public List<IoTGraphNode> GetAllLeafNodes()
    {
        List<IoTGraphNode> t = new List<IoTGraphNode>();
        if (isLeafNode)
            t.Add(this);
        else
            foreach (var ign in children)
                t.AddRange(ign.GetAllLeafNodes());
        return t;
    }

    public string GetChainName()
    {
        StringBuilder sb = new StringBuilder();
        if (parent.parent)
        {
            sb.Append(parent.GetChainName());
            sb.Append("_");
        }
        sb.Append(transform.name);
        return sb.ToString();
    }
}
