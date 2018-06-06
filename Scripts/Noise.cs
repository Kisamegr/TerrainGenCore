using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Noise {

  public enum NormalizeMode { Local, Global };

  public static float[,] PerlinNoise(int size, float scale, int seed, float offsetX, float offsetY, int octaves, float persistence, float lacunarity, NormalizeMode normalizeMode) {
    float[,] map = new float[size,size];
    float min = float.MaxValue;
    float max = float.MinValue;
    float maxPossibleHeight = 0;
    float frequency = 1;
    float amplitude = 1;

    System.Random rand = new System.Random(seed);

    float halfSize = size / 2f;

    Vector2[] octaveOffsets = new Vector2[octaves];
    int randOffset = 100000;
    float randC = (randOffset * 2.0f) / int.MaxValue;
    for (int i = 0; i<octaves; i++) {
      octaveOffsets[i].x = rand.Next()*randC - randOffset + offsetX;
      octaveOffsets[i].y = rand.Next()*randC - randOffset - offsetY;

      maxPossibleHeight += amplitude;
      amplitude *= persistence;
    }


    // Generate the noise
    for (int x = 0; x < size; x++) {
      for (int y = 0; y < size; y++) {
        float sample = 0;
        frequency = 1;
        amplitude = 1;

        for (int o = 0; o < octaves; o++) {
          float xCoord = ((x-halfSize) + octaveOffsets[o].x) / scale;
          float yCoord = ((y-halfSize) + octaveOffsets[o].y) / scale;
          float perlinValue = Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * 2 - 1;
          sample += perlinValue * amplitude;

          frequency *= lacunarity;
          amplitude *= persistence;
        }


        if (sample < min) min = sample;
        if (sample > max) max = sample;

        map[x, y] = sample;
      }
    }

    // Normalize the data
    for (int x = 0; x < size; x++) {
      for (int y = 0; y < size; y++) {
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


  public struct GeneratePerlinJob : IJob {
    public int size;
    public int lodStep;
    public float scale;
    public int seed;
    public float offsetX;
    public float offsetY;
    public int octaves;
    public float persistense;
    public float lacunarity;
    public NormalizeMode normalizeMode;

    public NativeArray<float> nativeHeightMap;

    public void Execute() {
      //float[,] heightMap = PerlinNoise(size, lodStep, scale, seed,
      //  offsetX, offsetY, octaves, persistense, lacunarity, normalizeMode);

      //for(int i=0; i<size*size; i++) {
      //  nativeHeightMap[i] = heightMap[i/size, i%size];
      //}
    }
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
