using System.Collections.Generic;
using UnityEngine;

public class TemporaryLabelMatcher : LabelMatcher
{
    public IoTDeviceHandler IoTDeviceHandler;

    public List<string> FromGenericActionKeys;
    public List<MultiActionLabelMatch> ToSpecificActionLabel;

    public override void RunRightExecutor(ActionPairs aps)
    {
        List<ActionPair> apl = aps.pairs;
        string key;
        foreach(ActionPair ap in apl)
        {
            //���̽㿡�� �����ؼ� �����ϹǷ� �ݵ�� ����ġ ���� ���� ���� ��
            //FromGenericActionKeys�� �̹� �Ʒ��� ������ �ݿ��Ͽ����Ƿ� ȣ�⸸ �ϸ� ��
            //��������:
            //1. Ȯ���ϰ� �ν� �����ϰ� ������ ���� ���� drink�� hand wave �ΰ��� ����ؼ� on�� off ��Ī
            //2. �ϳ��� �̺�Ʈ�� �����ߴٸ� ����ġ ���� ���� �������� �ʾƾ� �ϹǷ� ���� ����
            key = ap.key.ToLower();
            if (!FromGenericActionKeys.Contains(key))
                continue;
            foreach (string specKey in ToSpecificActionLabel[FromGenericActionKeys.IndexOf(key)].labels)
                //������ Open '�Ǵ�' On�� �ְ� �� �� �ִ� ��찡 �����Ƿ� �ܼ��� ���� ����
                IoTDeviceHandler.SendActionToActiveDevice(specKey);
            break;
        }
    }
}
