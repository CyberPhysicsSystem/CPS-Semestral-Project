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
        foreach (ActionPair ap in apl) //������̴� �׳� �� ���ļ� ����
            sb.Append((key.Length == 0) ? (ap.key.ToLower()) : (", " + ap.key.ToLower()));
        key = sb.ToString();

        if (accu < interval) return;
        accu -= interval;//0���� ��ƾ� ���װ� �������� ������, �ϴ� �̷���.

        //�ؽ�Ʈ ���̰� 512�� ���ѵǾ� �����Ƿ�
        if (key.Length > 512)
            key = key.Substring(0, 512);

        StartCoroutine(GetAction(key));
        key = "";
    }

    IEnumerator GetAction(string key)
    {
        //similarity.Item2�� �ش� �׼��� similarity score�� ����Ǿ� �־� �̸� �ٰŷ� ����� �� �ְ�����
        //���� �� ���ھ �׸� ���� ���� ���̹Ƿ� �ϴ� ����
        Tuple<int, float> similarity = brain.RankSimilarityScores(key, ToSpecificActionLabel);
        IoTDeviceHandler.SendActionToActiveDevice(ToSpecificActionLabel[similarity.Item1]);
        yield return null;
    }
}
