using UnityEngine;
using Unity.Barracuda;

public class ModelRunner : MonoBehaviour
{
    [Header("Model")]
    public NNModel modelAsset;           // Drag your ONNX/Barracuda model here
    private IWorker worker;

    [Header("Input")]
    public CameraScript cameraInput;     // Reference to webcam script

    [Header("Player")]
    public PlayerController playerController; // drag your PlayerController object here

    [Header("Run Model")]
    public bool runModel; // drag your PlayerController object here

    // Adjust to your model input shape (check in Netron)
    public int inputWidth = 224;
    public int inputHeight = 224;

    

    void Start()
    {
        // Load model and create worker
        var model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, model);
    }

    void Update()
    {
        if (runModel)
        {
            if (cameraInput.webcam == null || !cameraInput.webcam.isPlaying)
                return;

            // Capture current frame from webcam
            Texture2D frame = new Texture2D(cameraInput.webcam.width, cameraInput.webcam.height, TextureFormat.RGB24, false);
            frame.SetPixels32(cameraInput.webcam.GetPixels32());
            frame.Apply();

            // Run inference
            int prediction;
            float[] probabilities = Predict(frame, out prediction);

            // Log probabilities and predicted class
            Debug.Log("Probabilities: " + string.Join(", ", probabilities));
            Debug.Log("Predicted class: " + prediction);

            if (prediction == 0)
            {
                playerController.Punch();
            }

            // Cleanup temp texture
            Destroy(frame);
        }
        
    }

    // Returns probabilities and sets predicted class via out parameter
    public float[] Predict(Texture2D frame, out int predictedClass)
    {
        // Preprocess input to correct size + normalization
        Tensor input = Preprocessor.Preprocess(frame, inputWidth, inputHeight);

        // Run inference
        worker.Execute(input);
        Tensor output = worker.PeekOutput();

        // Extract raw results
        float[] logits = output.ToReadOnlyArray();

        // Apply softmax to get probabilities
        float[] probs = Softmax(logits);

        // Get predicted class
        predictedClass = ArgMax(probs);

        // Dispose tensors to free memory
        input.Dispose();
        output.Dispose();

        return probs;
    }

    private int ArgMax(float[] arr)
    {
        int index = 0;
        float max = arr[0];
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] > max)
            {
                max = arr[i];
                index = i;
            }
        }
        return index;
    }

    private float[] Softmax(float[] logits)
    {
        float maxLogit = float.NegativeInfinity;
        foreach (var l in logits)
            if (l > maxLogit) maxLogit = l;

        float sumExp = 0f;
        float[] expValues = new float[logits.Length];
        for (int i = 0; i < logits.Length; i++)
        {
            expValues[i] = Mathf.Exp(logits[i] - maxLogit); // subtract max for numerical stability
            sumExp += expValues[i];
        }

        float[] probs = new float[logits.Length];
        for (int i = 0; i < logits.Length; i++)
            probs[i] = expValues[i] / sumExp;

        return probs;
    }

    void OnDestroy()
    {
        worker.Dispose();
    }
}
