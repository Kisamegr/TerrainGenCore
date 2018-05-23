﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "New TerrainData", menuName = "Terrain Data")]
public class TerrainData :UpdatableData {

  [Header("Terrain Properties")]
  [Range(1, 300)]
  public int size = 200;
  [Range(-10,10)]
  public float heightOffset = 0.12f;
  [Range(1, 40)]
  public float heightScale = 10;
  public AnimationCurve heightCurve;
  public Noise.NormalizeMode normalizeMode;
  public int textureResolution = 512;

  [Header("Voxel Properties")]
  public bool useVoxels = false;
  public int cellSize = 1;
  [Range(2,100)]
  public int heightLayersNumber = 20;

  [Header("Generation Properties")]
  public int seed          = 0;
  public float offsetX     = 0;
  public float offsetY     = 0;
  [Range(0.01f, 15)]
  public float scale       = 1;
  [Range(0, 10)]
  public int octaves       = 4;
  [Range(0, 2)]
  public float persistense = 0.5f;
  [Range(0, 10)]
  public float lacunarity  = 2;

  public bool usePosterization = false;
  [Range(2, 10)]
  public int posterizeLevel = 4;

  [HideInInspector]
  public List<TerrainLayer> layers;

  public float MinHeight {
    get {
      return heightScale * heightCurve.Evaluate(0);
    }
  }

  public float MaxHeight {
    get {
      return heightScale * heightCurve.Evaluate(1);
    }
  }

  public float HeightOffsetScaled {
    get {
      return heightOffset * heightScale;
    }
  }

  public void ApplyToMaterial(Material material) {
    material.SetFloat("minHeight", MinHeight - HeightOffsetScaled);
    material.SetFloat("maxHeight", MaxHeight - HeightOffsetScaled);

    material.SetInt("layers", layers.Count);

    if (layers.Count > 0) {
      material.SetFloatArray("startHeights", layers.Select(x => x.startHeight).ToArray());
      material.SetFloatArray("blendHeights", layers.Select(x => x.blendHeight).ToArray());
      material.SetFloatArray("textureScales", layers.Select(x => x.textureScale).ToArray());
      material.SetColorArray("tintColors", layers.Select(x => x.tintColor).ToArray());
      material.SetFloatArray("tintColorBlends", layers.Select(x => x.tintBlend).ToArray());

      Texture2DArray albedoTextures = TextureGenerator.GenerateTextureArray(textureResolution,
      layers.Select(x => x.albedo).ToArray());
      if (albedoTextures.depth > 0)
        material.SetTexture("albedoTextures", albedoTextures);

      Texture2DArray normalTextures = TextureGenerator.GenerateTextureArray(textureResolution,
      layers.Select(x => x.normal).ToArray());
      if (normalTextures.depth > 0)
        material.SetTexture("normalTextures", albedoTextures);
    }

  }

  protected virtual void OnValidate() {
    if (autoUpdate) {
      if(useVoxels) {
        heightScale = heightLayersNumber;
      }
    }
    base.OnValidate();
  }

}
