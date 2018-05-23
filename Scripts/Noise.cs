using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {

  public enum NormalizeMode { Local, Global };

  public static float[,] PerlinNoise(int width, int height, float scale, int seed, float offsetX, float offsetY, int octaves, float persistence, float lacunarity, NormalizeMode normalizeMode = NormalizeMode.Local) {
    float[,] map = new float[width,height];
    float min = float.MaxValue;
    float max = float.MinValue;
    float maxPossibleHeight = 0;
    float frequency = 1;
    float amplitude = 1;

    Random.InitState(seed);

    float halfWidth = (width) / 2f;
    float halfHeight = (height) / 2f;

    Vector2[] octaveOffsets = new Vector2[octaves];
    for (int i = 0; i<octaves; i++) {
      octaveOffsets[i].x = Random.Range(-10000, 10000) + offsetX;
      octaveOffsets[i].y = Random.Range(-10000, 10000) - offsetY;

      maxPossibleHeight += amplitude;
      amplitude *= persistence;
    }


    // Generate the noise
    for (int x = 0; x < width; x++) {
      for (int y = 0; y < height; y++) {
        float sample = 0;
        frequency = 1;
        amplitude = 1;

        for (int o = 0; o < octaves; o++) {
          float xCoord = (x -halfWidth  + octaveOffsets[o].x) / scale;
          float yCoord = (y -halfHeight + octaveOffsets[o].y) / scale;
          float perlinValue = Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * 2 - 1;
          sample += perlinValue * amplitude;

          frequency *= lacunarity;
          //maxPossibleHeight += amplitude;
          amplitude *= persistence;
        }


        if (sample < min) min = sample;
        if (sample > max) max = sample;

        map[x, y] = sample;
      }
    }

    // Normalise the data
    for (int x = 0; x < width; x++) {
      for (int y = 0; y < height; y++) {
        if (normalizeMode == NormalizeMode.Global) {
          float normalizedHeight = (map [x, y] + 1) / ( maxPossibleHeight);
          map[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
        }
        else {
          map[x, y] = Mathf.InverseLerp(min, max, map[x, y]);
        }
      }
    }

    return map;
  }

  public static float[,] Posterize(float[,] noiseMap, int levelNumber) {
    float halfLayerSize = (1 / levelNumber) / 2;
    for (int x = 0; x<noiseMap.GetLength(0); x++) {
      for (int y = 0; y<noiseMap.GetLength(1); y++) {
        for (int i = 0; i<=levelNumber; i++) {
          float levelValue = (float) i/levelNumber;
          if (noiseMap[x, y] < levelValue+halfLayerSize) {
            noiseMap[x, y] = levelValue;
            break;
          }
        }
      }
    }

    return noiseMap;
  }
}
