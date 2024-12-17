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
            //파이썬에서 정렬해서 전송하므로 반드시 가중치 높은 것이 먼저 옴
            //FromGenericActionKeys에 이미 아래의 전략을 반영하였으므로 호출만 하면 됨
            //선택전략:
            //1. 확실하게 인식 가능하고 오인이 없는 동작 drink와 hand wave 두개를 사용해서 on과 off 매칭
            //2. 하나의 이벤트를 실행했다면 가중치 낮은 쪽은 실행하지 않아야 하므로 루프 종료
            key = ap.key.ToLower();
            if (!FromGenericActionKeys.Contains(key))
                continue;
            foreach (string specKey in ToSpecificActionLabel[FromGenericActionKeys.IndexOf(key)].labels)
                //지금은 Open '또는' On만 있고 둘 다 있는 경우가 없으므로 단순히 전부 실행
                IoTDeviceHandler.SendActionToActiveDevice(specKey);
            break;
        }
    }
}
