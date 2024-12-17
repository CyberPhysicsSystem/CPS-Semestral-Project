using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class IoTGraphSearcher : MonoBehaviour
{
    public IoTGraphNode root;
    public IoTDeviceHandler dhr;
    public MiniLM mlm;
    public List<string> IoTNames;
    public List<string> dummyIoTActions;

    void Awake()
    {
        if(!mlm) mlm = GetComponent<MiniLM>();
    }

    private void Start()
    {
        FillDeviceNames();
    }

    /*
     *   LeafNode�� �ƴϸ鼭 Child�� ���� �༮���� skip
     *   LeafNode�� �ƴϸ鼭 Child�� ���� �༮���� traverse
     *   LeafNode�̸鼭 Child�� �ִ� �༮���� �ϴ� LeafNode�� ���
    **/
    void FillDeviceNames()
    {
        IoTNames.Clear();
        var leaves = root.GetAllLeafNodes();
        foreach (var leave in leaves)
            IoTNames.Add(leave.GetChainName());
    }

    /*
     * ������ �޾��� ��:
     */
    public void Query(string query)
    {
        var device = GetBestNode(query, IoTNames);
        Debug.Log(device.GetChainName());
        dummyIoTActions.Clear();
        dummyIoTActions.AddRange((device as BaseIoTDevice).GetActionList());
        var action = mlm.GetMostRelevant(query, dummyIoTActions);
        Debug.Log(action);
        dhr.ChangeActiveDeviceTo(device as BaseIoTDevice);
        dhr.SendActionToActiveDevice(action);
    }

    IoTGraphNode GetBestNode(string query, List<string> IoTNames)
    {
        return InverseSearch(mlm.GetMostRelevant(query, IoTNames));
    }

    IoTGraphNode InverseSearch(string chainName)
    {
        IoTGraphNode current = root;
        List<string> splitted = chainName.Split('_').ToList<string>();
        while (splitted.Count > 0)
        {
            foreach (var ign in current.children)
            {
                if (ign.transform.name == splitted[0])
                {
                    current = ign;
                    splitted.RemoveAt(0);
                    break;
                }
            }
        }
        return current;
    }
}
