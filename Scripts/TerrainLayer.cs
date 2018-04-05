using UnityEngine;
using System;

[Serializable]
public struct TerrainLayer {
  public string layerName;
  public Texture2D albedo;
  public Texture2D normal;
  public Color tintColor;
  [Range(0,1)]
  public float tintBlend;
  [Range(1f,10)]
  public float textureScale;
  [Range(0,1)]
  public float startHeight;
  [Range(0,1)]
  public float blendHeight;


}
