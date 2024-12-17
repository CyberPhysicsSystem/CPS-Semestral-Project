using System.Collections.Generic;
using UnityEngine;
using Unity.Sentis;
using System.IO;
using FF = Unity.Sentis.Functional;

/*
 *              Tiny Stories Inference Code
 *              ===========================
 *  
 *  Put this script on the Main Camera
 *  
 *  In Assets/StreamingAssets put:
 *  
 *  MiniLMv6.sentis
 *  vocab.txt
 * 
 *  Install package com.unity.sentis
 * 
 */


public class MiniLM : MonoBehaviour
{
    const BackendType backend = BackendType.GPUCompute;

    string string1 = "That is a happy person";          // similarity = 1

    //Choose a string to comapre string1  to:
    string string2 = "That is a happy dog";             // similarity = 0.695
    //string string2 = "That is a very happy person";   // similarity = 0.943
    //string string2 = "Today is a sunny day";          // similarity = 0.257

    //Special tokens
    const int START_TOKEN = 101; 
    const int END_TOKEN = 102; 

    //Store the vocabulary
    string[] tokens;

    const int FEATURES = 384; //size of feature space

    Worker engine, dotScore;

    Model runtimeModel;

    void Start()
    {
        tokens = File.ReadAllLines(Application.streamingAssetsPath + "/vocab.txt");

        engine = CreateMLModel();

        dotScore = CreateDotScoreModel();

        var tokens1 = GetTokens(string1);
        var tokens2 = GetTokens(string2);

        using Tensor<float> embedding1 = GetEmbedding(tokens1);
        using Tensor<float> embedding2 = GetEmbedding(tokens2);

        float score = GetDotScore(embedding1, embedding2);

        Debug.Log("Similarity Score: " + score);
    }

    float GetDotScore(Tensor<float> A, Tensor<float> B)
    {
        dotScore.Schedule(A, B);
        var output = dotScore.PeekOutput() as Tensor<float>;
        var output_cpu = output.ReadbackAndClone();
        return output_cpu[0];
    }

    Tensor<float> GetEmbedding(List<int> tokens)
    {
        int N = tokens.Count;
        using var input_ids = new Tensor<int>(new TensorShape(1, N), tokens.ToArray());
        using var token_type_ids = new Tensor<int>(new TensorShape(1, N), new int[N]);
        int[] mask = new int[N];
        for (int i = 0; i < mask.Length; i++)
        {
            mask[i] = 1;
        }
        using var attention_mask = new Tensor<int>(new TensorShape(1, N), mask);

        engine.Schedule(input_ids, attention_mask, token_type_ids);

        var output = new Tensor<float>(new TensorShape(384)) as Tensor;
        engine.CopyOutput("output_0", ref output);

        return output as Tensor<float>;
    }

    Worker CreateMLModel()
    {
        Model model = ModelLoader.Load(Application.streamingAssetsPath + "/MiniLMv6.sentis");

        FunctionalGraph graph = new FunctionalGraph();
        FunctionalTensor[] inputs = graph.AddInputs(model);
        FunctionalTensor[] outputs = Functional.Forward(model, inputs);

        FunctionalTensor meanPooled = MeanPooling(outputs[0], inputs[1]);

        var modelWithMeanPooling = graph.Compile(meanPooled);
        return new Worker(modelWithMeanPooling, backend);
    }

    //Get average of token embeddings taking into account the attention mask
    FunctionalTensor MeanPooling(FunctionalTensor tokenEmbeddings, FunctionalTensor attentionMask)
    {
        var mask = attentionMask.Unsqueeze(-1).BroadcastTo(new[] { FEATURES });     //shape=(1,N,FEATURES)
        var A = FF.ReduceSum(tokenEmbeddings * mask, 1, false);                     //shape=(1,FEATURES)       
        var B = A / (FF.ReduceSum(mask, 1, false) + 1e-9f);                         //shape=(1,FEATURES)
        var C = FF.Sqrt(FF.ReduceSum(FF.Square(B), 1, true));                       //shape=(1,FEATURES)
        return B / C;                                                               //shape=(1,FEATURES)
    }

    Worker CreateDotScoreModel()
    {
        FunctionalGraph functionalGraph = new FunctionalGraph();
        FunctionalTensor input1 = functionalGraph.AddInput<float>(new TensorShape(1, FEATURES));
        FunctionalTensor input2 = functionalGraph.AddInput<float>(new TensorShape(1, FEATURES));

        FunctionalTensor reduce = Functional.ReduceSum(input1 * input2, 1);

        Model dotScoreModel = functionalGraph.Compile(reduce);

        return new Worker(dotScoreModel, backend);
    }

    List<int> GetTokens(string text)
    {
        //split over whitespace
        string[] words = text.ToLower().Split(null);

        var ids = new List<int>
        {
            START_TOKEN
        };

        string s = "";

        foreach (var word in words)
        {
            int start = 0;
            for(int i = word.Length; i >= 0;i--)
            {
                string subword = start == 0 ? word.Substring(start, i) : "##" + word.Substring(start, i-start);
                int index = System.Array.IndexOf(tokens, subword);
                if (index >= 0)
                {
                    ids.Add(index);
                    s += subword + " ";
                    if (i == word.Length) break;
                    start = i;
                    i = word.Length + 1;
                }
            }
        }

        ids.Add(END_TOKEN);

        Debug.Log("Tokenized sentece = " + s);

        return ids;
    }

    private void OnDestroy()
    { 
        dotScore?.Dispose();
        engine?.Dispose();
    }

}
