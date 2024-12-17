using System.Collections.Generic;
using UnityEngine;
using Unity.Sentis;
using System.Text;
using Unity.Collections;

using UnityEngine.Events;
using System.Text.RegularExpressions;
using UnityEditor.Search;

public class RunWhisper : MonoBehaviour
{
    public BackendType backend = BackendType.GPUCompute;

    Worker decoder1, decoder2, encoder, spectrogram;
    Worker argmax;

    public AudioClip audioClip;

    // This is how many tokens you want. It can be adjusted.
    const int maxTokens = 100;

    // Special tokens see added tokens file for details
    const int END_OF_TEXT = 50257;
    const int START_OF_TRANSCRIPT = 50258;
    const int ENGLISH = 50259;
    const int GERMAN = 50261;
    const int FRENCH = 50265;
    const int TRANSCRIBE = 50359; //for speech-to-text in specified language
    const int TRANSLATE = 50358;  //for speech-to-text then translate to English
    const int NO_TIME_STAMPS = 50363;
    const int START_TIME = 50364;

    int numSamples;
    string[] tokens;

    int tokenCount = 0;
    NativeArray<int> outputTokens;

    // Used for special character decoding
    int[] whiteSpaceCharacters = new int[256];

    Tensor<float> encodedAudio;

    bool transcribe = false;
    string outputString = "";

    // Maximum size of audioClip (30s at 16kHz)
    const int maxSamples = 30 * 16000;

    public ModelAsset audioDecoder1, audioDecoder2;
    public ModelAsset audioEncoder;
    public ModelAsset logMelSpectro;

    public void Start()
    {
        InitializeModel();
        //Transcribe();
    }

    NativeArray<int> lastToken;
    Tensor<int> lastTokenTensor;
    Tensor<int> tokensTensor;
    Tensor<float> audioInput;

    public UnityEvent OnTranscriptionFinished;
    public UnityEvent<string> OnTextUpdate;

    void LoadAudio()
    {
        if (audioClip.frequency != 16000)
        {
            Debug.Log($"The audio clip should have frequency 16kHz. It has frequency {audioClip.frequency / 1000f}kHz");
            return;
        }

        numSamples = audioClip.samples;

        if (numSamples > maxSamples)
        {
            Debug.Log($"The AudioClip is too long. It must be less than 30 seconds. This clip is {numSamples / audioClip.frequency} seconds.");
            return;
        }

        var data = new float[maxSamples];
        numSamples = maxSamples;
        audioClip.GetData(data, 0);
        audioInput = new Tensor<float>(new TensorShape(1, numSamples), data);
    }

    void EncodeAudio()
    {
        spectrogram.Schedule(audioInput);
        var logmel = spectrogram.PeekOutput() as Tensor<float>;
        encoder.Schedule(logmel);
        encodedAudio = encoder.PeekOutput() as Tensor<float>;
        logmel.Dispose();
    }
    async Awaitable InferenceStep()
    {
        decoder1.SetInput("input_ids", tokensTensor);
        decoder1.SetInput("encoder_hidden_states", encodedAudio);
        decoder1.Schedule();

        var past_key_values_0_decoder_key = decoder1.PeekOutput("present.0.decoder.key") as Tensor<float>;
        var past_key_values_0_decoder_value = decoder1.PeekOutput("present.0.decoder.value") as Tensor<float>;
        var past_key_values_1_decoder_key = decoder1.PeekOutput("present.1.decoder.key") as Tensor<float>;
        var past_key_values_1_decoder_value = decoder1.PeekOutput("present.1.decoder.value") as Tensor<float>;
        var past_key_values_2_decoder_key = decoder1.PeekOutput("present.2.decoder.key") as Tensor<float>;
        var past_key_values_2_decoder_value = decoder1.PeekOutput("present.2.decoder.value") as Tensor<float>;
        var past_key_values_3_decoder_key = decoder1.PeekOutput("present.3.decoder.key") as Tensor<float>;
        var past_key_values_3_decoder_value = decoder1.PeekOutput("present.3.decoder.value") as Tensor<float>;

        var past_key_values_0_encoder_key = decoder1.PeekOutput("present.0.encoder.key") as Tensor<float>;
        var past_key_values_0_encoder_value = decoder1.PeekOutput("present.0.encoder.value") as Tensor<float>;
        var past_key_values_1_encoder_key = decoder1.PeekOutput("present.1.encoder.key") as Tensor<float>;
        var past_key_values_1_encoder_value = decoder1.PeekOutput("present.1.encoder.value") as Tensor<float>;
        var past_key_values_2_encoder_key = decoder1.PeekOutput("present.2.encoder.key") as Tensor<float>;
        var past_key_values_2_encoder_value = decoder1.PeekOutput("present.2.encoder.value") as Tensor<float>;
        var past_key_values_3_encoder_key = decoder1.PeekOutput("present.3.encoder.key") as Tensor<float>;
        var past_key_values_3_encoder_value = decoder1.PeekOutput("present.3.encoder.value") as Tensor<float>;

        decoder2.SetInput("input_ids", lastTokenTensor);
        decoder2.SetInput("past_key_values.0.decoder.key", past_key_values_0_decoder_key);
        decoder2.SetInput("past_key_values.0.decoder.value", past_key_values_0_decoder_value);
        decoder2.SetInput("past_key_values.1.decoder.key", past_key_values_1_decoder_key);
        decoder2.SetInput("past_key_values.1.decoder.value", past_key_values_1_decoder_value);
        decoder2.SetInput("past_key_values.2.decoder.key", past_key_values_2_decoder_key);
        decoder2.SetInput("past_key_values.2.decoder.value", past_key_values_2_decoder_value);
        decoder2.SetInput("past_key_values.3.decoder.key", past_key_values_3_decoder_key);
        decoder2.SetInput("past_key_values.3.decoder.value", past_key_values_3_decoder_value);

        decoder2.SetInput("past_key_values.0.encoder.key", past_key_values_0_encoder_key);
        decoder2.SetInput("past_key_values.0.encoder.value", past_key_values_0_encoder_value);
        decoder2.SetInput("past_key_values.1.encoder.key", past_key_values_1_encoder_key);
        decoder2.SetInput("past_key_values.1.encoder.value", past_key_values_1_encoder_value);
        decoder2.SetInput("past_key_values.2.encoder.key", past_key_values_2_encoder_key);
        decoder2.SetInput("past_key_values.2.encoder.value", past_key_values_2_encoder_value);
        decoder2.SetInput("past_key_values.3.encoder.key", past_key_values_3_encoder_key);
        decoder2.SetInput("past_key_values.3.encoder.value", past_key_values_3_encoder_value);

        decoder2.Schedule();

        var logits = decoder2.PeekOutput("logits") as Tensor<float>;
        argmax.Schedule(logits);
        using var t_Token = await argmax.PeekOutput().ReadbackAndCloneAsync() as Tensor<int>;
        int index = t_Token[0];

        tokenCount++;
        outputTokens[tokenCount] = lastToken[0];
        lastToken[0] = index;
        tokensTensor.Reshape(new TensorShape(1, tokenCount));
        tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);
        lastTokenTensor.dataOnBackend.Upload<int>(lastToken, 1);

        if (index == END_OF_TEXT)
        {
            transcribe = false;
        }
        else if (index < tokens.Length)
        {
            var temp_s = GetUnicodeText(tokens[index]);
            temp_s = SanitizeQuery(temp_s);
            outputString += temp_s;
        }

        OnTextUpdate?.Invoke(outputString);
        Debug.Log(outputString);
    }

    // Tokenizer
    public TextAsset jsonFile;
    void GetTokens()
    {
        var vocab = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonFile.text);
        tokens = new string[vocab.Count];
        foreach (var item in vocab)
        {
            tokens[item.Value] = item.Key;
        }
    }

    string GetUnicodeText(string text)
    {
        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(ShiftCharacterDown(text));
        return Encoding.UTF8.GetString(bytes);
    }

    string ShiftCharacterDown(string text)
    {
        string outText = "";
        foreach (char letter in text)
        {
            outText += ((int)letter <= 256) ? letter :
                (char)whiteSpaceCharacters[(int)(letter - 256)];
        }
        return outText;
    }

    void SetupWhiteSpaceShifts()
    {
        for (int i = 0, n = 0; i < 256; i++)
        {
            if (IsWhiteSpace((char)i)) whiteSpaceCharacters[n++] = i;
        }
    }

    bool IsWhiteSpace(char c)
    {
        return !(('!' <= c && c <= '~') || (' ' <= c && c <= ' ') || (' ' <= c && c <= ' '));
    }

    private void OnDestroy()
    {
        FullCleanUp();
    }

    void FullCleanUp()
    {
        decoder1.Dispose();
        decoder2.Dispose();
        encoder.Dispose();
        spectrogram.Dispose();
        argmax.Dispose();
        CleanUp();
    }

    void CleanUp()
    {
        encodedAudio?.Dispose();
        audioInput?.Dispose();
        if(lastToken != null)
            lastToken.Dispose();
        lastTokenTensor?.Dispose();
        tokensTensor?.Dispose();
        outputString = "";
    }

    void InitializeModel()
    {
        SetupWhiteSpaceShifts();
        GetTokens();

        // decoder1 = new Worker(ModelLoader.Load(audioDecoder1), backend);
        // decoder2 = new Worker(ModelLoader.Load(audioDecoder2), backend);

        decoder1 = new Worker(ModelLoader.Load(Application.streamingAssetsPath + "/decoder_model.sentis"), backend);
        decoder2 = new Worker(ModelLoader.Load(Application.streamingAssetsPath + "/decoder_with_past_model.sentis"), backend);

        FunctionalGraph graph = new FunctionalGraph();
        var input = graph.AddInput(DataType.Float, new DynamicTensorShape(1, 1, 51865));
        var amax = Functional.ArgMax(input, -1, false);
        var selectTokenModel = graph.Compile(amax);
        argmax = new Worker(selectTokenModel, backend);

        // encoder = new Worker(ModelLoader.Load(audioEncoder), backend);
        // spectrogram = new Worker(ModelLoader.Load(logMelSpectro), backend);
        encoder = new Worker(ModelLoader.Load(Application.streamingAssetsPath + "/encoder_model.sentis"), backend);
        spectrogram = new Worker(ModelLoader.Load(Application.streamingAssetsPath + "/logmel_spectrogram.sentis"), backend);
    }

    private async void Transcribe()
    {
        outputTokens = new NativeArray<int>(maxTokens, Allocator.Persistent);

        outputTokens[0] = START_OF_TRANSCRIPT;
        outputTokens[1] = ENGLISH;// GERMAN;//FRENCH;//
        outputTokens[2] = TRANSCRIBE; //TRANSLATE;//
        outputTokens[3] = NO_TIME_STAMPS;// START_TIME;//
        tokenCount = 3;

        LoadAudio();
        EncodeAudio();
        transcribe = true;

        tokensTensor = new Tensor<int>(new TensorShape(1, maxTokens));
        ComputeTensorData.Pin(tokensTensor);
        tokensTensor.Reshape(new TensorShape(1, tokenCount));
        tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);

        lastToken = new NativeArray<int>(1, Allocator.Persistent);
        lastToken[0] = NO_TIME_STAMPS;
        lastTokenTensor = new Tensor<int>(new TensorShape(1, 1), new[] { NO_TIME_STAMPS });

        while (true)
        {
            if (!transcribe || tokenCount >= (outputTokens.Length - 1))
            {
                OnTranscriptionFinished?.Invoke();
                return;
            }
            m_Awaitable = InferenceStep();
            await m_Awaitable;
        }
    }
    Awaitable m_Awaitable;

    public void ReplaceAudio(AudioClip ac)
    {
        audioClip = ac;
        CleanUp();
        Transcribe();
    }
    string SanitizeQuery(string rawQuery)
    {
        string pattern = @"[\p{C}\p{Zl}\p{Zp}]";
        return Regex.Replace(rawQuery, pattern, " ");
    }
}
