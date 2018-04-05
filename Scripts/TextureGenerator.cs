using UnityEngine;

public class TextureGenerator {

  public static void GenerateTexture(float[,] pixelColors, ref Texture2D texture) {
    int width  = pixelColors.GetLength(0);
    int height = pixelColors.GetLength(1);

    Color[] colors = new Color[width*height];

    for (int y = 0; y < height; y++) {
      for (int x = 0; x < width; x++) {
        float sample =  pixelColors[x,y];
        colors[y*width + x] = new Color(sample, sample, sample);
      }
    }

    texture.SetPixels(colors);
    texture.Apply();
  }

  public static Texture2DArray GenerateTextureArray(int textureSize, Texture2D[] textures) {
    Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, TextureFormat.RGB565, true);

    for (int i = 0; i<textures.Length; i++)
      textureArray.SetPixels(textures[i].GetPixels(), i);

    textureArray.Apply();
    return textureArray;
  }
}
