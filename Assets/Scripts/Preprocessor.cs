using UnityEngine;
using Unity.Barracuda;

public class Preprocessor
{
    public static Tensor Preprocess(Texture2D image, int targetWidth, int targetHeight)
    {
        // Resize image using RenderTexture for better quality
        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
        Graphics.Blit(image, rt);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D resized = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
        resized.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        resized.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        // Convert to float array (NHWC: 1xH x W x 3)
        float[] floatValues = new float[targetHeight * targetWidth * 3];
        Color[] pixels = resized.GetPixels();

        // Normalize each channel using standard ImageNet mean/std
        float meanR = 0.485f, meanG = 0.456f, meanB = 0.406f;
        float stdR = 0.229f, stdG = 0.224f, stdB = 0.225f;

        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];

            // Convert 0–1 range to normalized input
            floatValues[i * 3 + 0] = (pixel.r - meanR) / stdR; // R
            floatValues[i * 3 + 1] = (pixel.g - meanG) / stdG; // G
            floatValues[i * 3 + 2] = (pixel.b - meanB) / stdB; // B
        }

        return new Tensor(1, targetHeight, targetWidth, 3, floatValues); // NHWC
    }
}
