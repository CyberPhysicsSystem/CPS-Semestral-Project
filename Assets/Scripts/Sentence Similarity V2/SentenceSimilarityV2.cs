using UnityEngine;
using Unity.Sentis;
using System.Collections.Generic;

namespace SentenceSimilarityV2
{
    using SSU = SentenceSimilarity.SentenceSimilarityUtils.SentenceSimilarityUtils_;
    public class SentenceSimilarityV2 : MonoBehaviour
    {
        public ModelAsset modelAsset;
        Model runtimeModel;
        Worker worker;

        public List<string> TestStringToTokenize;
        public List<float> TestStringScore;


        async private void Awake()
        {
            TestStringScore.Clear();
            for (int i = 0; i< TestStringToTokenize.Count; i++)
                TestStringScore.Add(0);

            Model sourceModel = ModelLoader.Load(modelAsset);

            // Create a functional graph that runs the input model
            FunctionalGraph graph = new FunctionalGraph();
            FunctionalTensor[] inputs = graph.AddInputs(sourceModel);
            FunctionalTensor[] outputs = Functional.Forward(sourceModel, inputs);

            // Create a model by compiling the functional graph.
            runtimeModel = graph.Compile(outputs);

            // Create input data as a tensor
            Dictionary<string, Tensor> input = SSU.TokenizeInput(TestStringToTokenize);

            // Create an engine
            worker = new Worker(runtimeModel, BackendType.GPUCompute);

            int idx = 0;
            foreach (var key in input.Keys)
            {
                if (input.TryGetValue(key, out Tensor value))
                {
                    worker.Schedule(value);
                    var gt = worker.PeekOutput();
                    var ct = await gt.ReadbackAndCloneAsync() as Tensor<float>;
                    TestStringScore[idx] = ct[0];
                    ct.Dispose();
                }
                idx++;
            }
        }


        void OnDisable()
        {
            // Tell the GPU we're finished with the memory the engine used
            worker.Dispose();
        }
    }
}