using UnityEngine;
using Unity.Barracuda;

public class Preprocessor
{
    public static Tensor Preprocess(Texture2D image, int width, int height)
    {
        Texture2D resized = new Texture2D(width, height);
        Graphics.ConvertTexture(image, resized);

        float[] floatValues = new float[width * height * 3];
        Color[] pixels = resized.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            // Normalize 0–1
            floatValues[i * 3 + 0] = pixels[i].r;
            floatValues[i * 3 + 1] = pixels[i].g;
            floatValues[i * 3 + 2] = pixels[i].b;
        }

        return new Tensor(1, height, width, 3, floatValues);
    }
}
