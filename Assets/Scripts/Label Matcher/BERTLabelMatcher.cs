using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using SenSim = SentenceSimilarity.SentenceSimilarity;

public class BERTLabelMatcher : LabelMatcher
{
    public SenSim brain;
    public IoTDeviceHandler IoTDeviceHandler;

    public string[] ToSpecificActionLabel;

    public float interval = 3;
    public float accu = 0;

    public string key;

    private void Update()
    {
        accu += Time.deltaTime;
    }

    public override void RunRightExecutor(ActionPairs aps)
    {
        List<ActionPair> apl = aps.pairs;
        StringBuilder sb = new StringBuilder();
        sb.Append(key);
        foreach (ActionPair ap in apl) //시험용이니 그냥 다 합쳐서 전달
            sb.Append((key.Length == 0) ? (ap.key.ToLower()) : (", " + ap.key.ToLower()));
        key = sb.ToString();

        if (accu < interval) return;
        accu -= interval;//0으로 잡아야 버그가 없을지도 모르지만, 일단 이렇게.

        //텍스트 길이가 512로 제한되어 있으므로
        if (key.Length > 512)
            key = key.Substring(0, 512);

        StartCoroutine(GetAction(key));
        key = "";
    }

    IEnumerator GetAction(string key)
    {
        //similarity.Item2에 해당 액션의 similarity score가 저장되어 있어 이를 근거로 사용할 수 있겠지만
        //현재 그 스코어가 그리 높지 않을 것이므로 일단 무시
        Tuple<int, float> similarity = brain.RankSimilarityScores(key, ToSpecificActionLabel);
        IoTDeviceHandler.SendActionToActiveDevice(ToSpecificActionLabel[similarity.Item1]);
        yield return null;
    }
}
