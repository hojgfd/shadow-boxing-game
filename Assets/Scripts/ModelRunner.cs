using UnityEngine;
using Unity.Barracuda;

public class ModelRunner : MonoBehaviour
{
    public NNModel modelAsset;           // Drag your ONNX/Barracuda model here
    private IWorker worker;

    public CameraScript cameraInput;      // Reference to the script that handles the webcam

    void Start()
    {
        // Load the model and create a worker for inference
        var model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, model);
    }

    void Update()
    {
        if (cameraInput.webcam == null || !cameraInput.webcam.isPlaying) return;

        // Capture the current webcam frame
        Texture2D frame = new Texture2D(cameraInput.webcam.width, cameraInput.webcam.height);
        frame.SetPixels(cameraInput.webcam.GetPixels());
        frame.Apply();

        // Run inference
        int prediction = Predict(frame);

        // Print prediction in real-time
        Debug.Log("Predicted Class Index: " + prediction);

        //// Optional: Map prediction to actions here
        //switch (prediction)
        //{
        //    case 0: MoveLeft(); break;
        //    case 1: MoveRight(); break;
        //    case 2: Jump(); break;
        //    default: Idle(); break;
        //}
    }

    public int Predict(Texture2D frame)
    {
        // Preprocess frame to match model input (resize & normalize)
        Tensor input = Preprocessor.Preprocess(frame, 224, 224);

        // Run the model
        worker.Execute(input);
        Tensor output = worker.PeekOutput();

        // Find the index with the highest confidence
        int bestIndex = ArgMax(output.ToReadOnlyArray());

        // Dispose tensors to free memory
        input.Dispose();
        output.Dispose();

        return bestIndex;
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

    void OnDestroy()
    {
        worker.Dispose();
    }

    //// Example placeholder actions
    //void MoveLeft() { /* implement movement */ }
    //void MoveRight() { /* implement movement */ }
    //void Jump() { /* implement jump */ }
    //void Idle() { /* implement idle */ }
}
