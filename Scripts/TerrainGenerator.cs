﻿using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator :MonoBehaviour {

  public TerrainData terrainData;

  private MeshFilter   terrainMeshFilter;
  private MeshRenderer terrainMeshRenderer;

  private MeshData meshData;
  private Mesh terrainMesh;
  private Texture2D heightMap;
  private float[,] heightMapData;

  public void Awake() {
    terrainMeshFilter = GetComponent<MeshFilter>();
    terrainMeshRenderer = GetComponent<MeshRenderer>();

    terrainMesh = new Mesh();
    terrainMeshFilter.mesh = terrainMesh;
  }

  public void OnValidate() {
    if (Application.isPlaying) {
      if (terrainData != null) 
        terrainData.OnValuesUpdated += Regenerate;
    }
  }

  public void Regenerate() {
    if (terrainData) {
      // Generate the heightmap
      CreateHeightMap();

      // Create the mesh data (vertices, triangles, etc...)
      meshData = MeshGenerator.GenerateMeshData(terrainData.size, terrainData.size,
        heightMapData, terrainData.heightScale, terrainData.heightCurve);

      // Create the mesh from the mesh data
      meshData.ApplyToMesh(terrainMesh);

      // Update the material and position
      UpdateTerrain();
    }
    else
      Debug.Log("No terrain data specified!!!");
  }

  public void UpdateTerrain() {
    if (terrainMeshRenderer && terrainData) {
      terrainData.ApplyToMaterial(terrainMeshRenderer.sharedMaterial);
      transform.position = new Vector3(0, -terrainData.HeightOffsetScaled, 0);
    }
  }

  void CreateHeightMap() {
    // Generate the perlin noise height map
    heightMapData = Noise.PerlinNoise(terrainData.size, terrainData.size, terrainData.scale,
      terrainData.seed, terrainData.offsetX, terrainData.offsetY,
      terrainData.octaves, terrainData.persistense, terrainData.lacunarity);

    if (terrainData.usePosterization)
      heightMapData = Noise.Posterize(heightMapData, terrainData.posterizeLevel);

    // Create the heightmap if it does not exist
    if (!heightMap) {
      heightMap = new Texture2D(terrainData.size, terrainData.size);
      heightMap.filterMode = FilterMode.Point;
    }
    // Or check if the size has changed to update it
    else if (heightMap.width != terrainData.size || heightMap.height != terrainData.size)
      heightMap.Resize(terrainData.size, terrainData.size);

    // Generate the actual texture from the height map and set the material properties
    //TextureGenerator.GenerateTexture(heightMapData, ref heightMap);
    //terrainMeshRenderer.sharedMaterial.SetFloat("_HeightScale", terrainData.heightScale);
    //terrainMeshRenderer.sharedMaterial.mainTexture = heightMap;
  }

}
