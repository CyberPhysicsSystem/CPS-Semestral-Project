using UnityEngine;
using Unity.Sentis;
using static Unity.Sentis.Model;

/*
 *    Model to turn 44kHz and 22kHz audio to 16kHz 
 *    ============================================
 *    
 *  Place the audioClip in the inputAudio field and press play 
 *  The results will appear in the console
 */

public class AudioResample : MonoBehaviour
{
    public BackendType backend = BackendType.GPUCompute;

    //Place the audio clip to resample here
    public AudioClip inputAudio;
    public AudioClip outputAudio;

    public bool playFinalAudio = true;
    public bool transcribeFinalAudio = true;

    Worker engine;

    void Start()
    {
        //ConvertAudio();
    }

    // Update is called once per frame
    void Update()
    {
        //if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        //{
        //    ConvertAudio();
        //}
    }

    void ConvertAudio()
    {
        Debug.Log($"The frequency of the input audio clip is {inputAudio.frequency} Hz with {inputAudio.channels} channels.");
        Model model;
        if (inputAudio.frequency == 44100)
            model = ModelLoader.Load(Application.streamingAssetsPath + "/audio_resample_44100_16000.sentis");
        else if (inputAudio.frequency == 22050)
            model = ModelLoader.Load(Application.streamingAssetsPath + "/audio_resample_22050_16000.sentis");
        else
        {
            Debug.Log("Only frequencies of 44kHz and 22kHz are compatible");
            return;
        }

        int channels = inputAudio.channels;
        int size = inputAudio.samples * channels;
        float[] data = new float[size];
        inputAudio.GetData(data, 0);
        using var input = new Tensor<float>(new TensorShape(1, size), data);

        FunctionalGraph graph = new FunctionalGraph();
        FunctionalTensor[] inputs = graph.AddInputs(model);
        FunctionalTensor[] outputs = Functional.Forward(model, inputs);
        var runtimeModel = graph.Compile(outputs);

        engine = new Worker(runtimeModel, backend);
        engine.Schedule(input);

        float[] outData;
        var output = engine.PeekOutput() as Tensor<float>;
        var output_cpu = output.ReadbackAndClone();
        //if (inputAudio.frequency == 44100)
        //{
        //    //The model gives 2x as many samples as we would like so we fix it:
        //    //We need to pad it if it has odd number of samples
        //    int n = output.shape[1] % 2;
        //    //using var output2 = new Tensor<float>(new TensorShape(1, output.shape[1] + n));
        //    backend.Pad(output, output2, new int[] { 0, n }, Unity.Sentis.Layers.PadMode.Constant, 0);

        //    //Now we take every second sample:
        //    output2.Reshape(new TensorShape(output2.shape[1] / 2, 2));
        //    using var output3 = TensorFloat.AllocNoData(new TensorShape(output2.shape[0], 1));
        //    backend.Slice(output2, output3, new[] { 0 }, new[] { 1 }, new[] { 1 });
        //    output3.CompleteOperationsAndDownload();
        //    outData = output3.ToReadOnlyArray();
        //}
        //else
        //{
        //    output.CompleteOperationsAndDownload();
        //    outData = output.ToReadOnlyArray();
        //}

        //if (inputAudio.frequency == 44100)
        //{
        //    int n = output.shape[1] % 2;
        //    FunctionalGraph graph2 = new FunctionalGraph();
        //    FunctionalTensor x = graph.AddInput<float>(output.shape);
        //    FunctionalTensor padded = x.Pad(new int[] { 0, n }, 0);
        //    FunctionalTensor reshaped = padded.Reshape(new int[] { n / 2, 2 });
        //    FunctionalTensor sliced = reshaped.Split(new[] { 1, 1 }, dim: 1)[0];
        //    var padmodel = graph2.Compile(padded, reshaped, sliced);
        //    var w = new Worker(padmodel, backend);
        //    w.Schedule(output);
        //}
        //outData = output.ReadbackAndClone().DownloadToArray();

        if (inputAudio.frequency == 44100)
        {
            // The model gives 2x as many samples as we would like, so we fix it:
            // We need to pad it if it has an odd number of samples
            int n = output_cpu.shape[1] % 2;

            // Padding manually using a new Tensor<float>
            Tensor<float> paddedOutput;
            if (n == 1) // Odd number of samples, needs padding
            {
                var paddedShape = new TensorShape(1, output_cpu.shape[1] + 1);
                paddedOutput = new Tensor<float>(paddedShape);

                // Copy data from output to paddedOutput and pad with zero
                for (int i = 0; i < output_cpu.shape[1]; i++)
                {
                    paddedOutput[0, i] = output_cpu[0, i];
                }
                paddedOutput[0, output_cpu.shape[1]] = 0; // Add a 0 at the end
            }
            else
            {
                paddedOutput = output_cpu; // If even, no need to pad
            }

            // Reshape to group into pairs (take every two samples)
            var reshapedShape = new TensorShape(paddedOutput.shape[1] / 2, 2);
            var reshapedOutput = new Tensor<float>(reshapedShape);

            for (int i = 0; i < reshapedShape[0]; i++)
            {
                reshapedOutput[i, 0] = paddedOutput[0, i * 2];     // Take first sample of pair
                reshapedOutput[i, 1] = paddedOutput[0, i * 2 + 1]; // Take second sample of pair
            }

            // Extract the first column (similar to Slice)
            var output3Shape = new TensorShape(reshapedOutput.shape[0], 1);
            var output3 = new Tensor<float>(output3Shape);

            for (int i = 0; i < reshapedOutput.shape[0]; i++)
            {
                output3[i, 0] = reshapedOutput[i, 0]; // Take only the first sample of each pair
            }

            // Complete the operations and download the result
            outData = output3.ReadbackAndClone().DownloadToArray();

            // Dispose temporary tensors
            if (n == 1) paddedOutput.Dispose();
            reshapedOutput.Dispose();
            output3.Dispose();
        }
        else
        {
            // Direct download if no processing is needed
            outData = output.ReadbackAndClone().DownloadToArray();
        }

        input.Dispose();
        output.Dispose();
        output_cpu.Dispose();

        int samplesOut = outData.Length / channels;

        outputAudio = AudioClip.Create("outputAudio", samplesOut, channels, 16000, false);
        outputAudio.SetData(outData, 0);
        outputAudio.LoadAudioData();

        Debug.Log($"The audio has been converted to 16Khz with {channels} channels.");

        if (playFinalAudio)
        {
            GetComponent<AudioSource>().PlayOneShot(outputAudio);
        }

        if (transcribeFinalAudio)
        {
            var rw = GetComponent<RunWhisper>();
            rw.ReplaceAudio(outputAudio);
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    void CleanUp()
    {
        engine?.Dispose();
    }

    public void ReplaceAudio(AudioClip ac)
    {
        inputAudio = ac;
        CleanUp();
        ConvertAudio();
    }
}