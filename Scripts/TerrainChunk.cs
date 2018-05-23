using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainChunk :Chunk {

  [Range(1,4)]
  public int lod = 1;
  public TerrainData terrainData;

  protected BoxCollider collider;

  protected MeshData meshData;
  public Texture2D heightMap;
  protected float[,] heightMapData;
  protected bool hasHeightMap;

  protected int lodStep;
  protected int lodSize;

  public TerrainChunk(LODInfo[] lodInfo, TerrainData terrainData, int size, Vector2Int chunkCoords, Material terrainMaterial, Transform parent = null)
      : base(lodInfo, size, chunkCoords, parent) {

    this.terrainData = terrainData;
    meshRenderer.sharedMaterial = terrainMaterial;
    hasHeightMap = false;
    meshGameObject.name = "TerrainChunk " + chunkCoords.ToString();

    collider = meshGameObject.AddComponent<BoxCollider>();
    collider.size = new Vector3(size, 0, size);

  }

  //public void OnValidate() {
  //  if (Application.isPlaying) {
  //    if (terrainData != null) 
  //      terrainData.OnValuesUpdated += Regenerate;
  //    Regenerate();
  //  }
  //}

  public override void UpdateChunk(Vector3 viewerPosition) {
    base.UpdateChunk(viewerPosition);

    // Set the lod variables based on the current lod info
    lod = lodInfo[lodIndex].lod;
    lodStep = (int) Math.Pow(2, lod-1);
    lodSize = terrainData.size / lodStep;

    // If the chunk is visible
    if (visible) {
      // If the current lod does not have a mesh generated
      if (!lodMeshes[lodIndex].hasMesh) {
        // Generate the terrain mesh and set it to the current lod
        Mesh mesh = GenerateTerrainMesh();
        lodMeshes[lodIndex].mesh = mesh;
        lodMeshes[lodIndex].hasMesh = true;

        // Update the material
        UpdateMaterial();
      }

      // Set the mesh filter's mesh to the current lod mesh
      meshFilter.mesh = lodMeshes[lodIndex].mesh;
    }
  }

  public void UpdateMaterial() {
    if (meshRenderer && terrainData) {
      terrainData.ApplyToMaterial(meshRenderer.sharedMaterial);
      //transform.position = new Vector3(0, -terrainData.HeightOffsetScaled, 0);
    }
  }

  protected virtual Mesh GenerateTerrainMesh() {
    // Create the height map
    CreateHeightMap();

    // Create the mesh data (vertices, triangles, etc...)
    meshData = MeshGenerator.GenerateMeshData(
      lodSize+1, 
      lodStep,
      heightMapData, 
      terrainData.heightScale, 
      terrainData.heightCurve);

    // Apply the mesh data to the mesh itself
    Mesh mesh = new Mesh();
    meshData.ApplyToMesh(mesh);

    return mesh;
  }

  // Generates a 2D array of height values with Perlin Noise
  protected void CreateHeightMap() {
      // Generate the perlin noise height map
      heightMapData = Noise.PerlinNoise(lodSize+1, lodStep, 
        terrainData.scale, terrainData.seed, 
        terrainData.offsetX + chunkPosition.x ,
        terrainData.offsetY - chunkPosition.z,
        terrainData.octaves, terrainData.persistense, terrainData.lacunarity,
        terrainData.normalizeMode);

      //if (terrainData.usePosterization)
        //heightMapData = Noise.Posterize(heightMapData, terrainData.posterizeLevel);

      // Create the height map if it does not exist
      //if (!heightMap) {
      //  heightMap = new Texture2D(terrainData.size+1, terrainData.size+1) {
      //    filterMode = FilterMode.Point
      //  };
      //}
      //// Or check if the size has changed to update it
      //else if (heightMap.width != terrainData.size+1 || heightMap.height != terrainData.size+1)
      //  heightMap.Resize(terrainData.size+1, terrainData.size+1);

      // Generate the actual texture from the height map and set the material properties
      //TextureGenerator.GenerateTexture(heightMapData, ref heightMap);
      //meshRenderer.sharedMaterial.SetFloat("_HeightScale", terrainData.heightScale);
      //meshRenderer.material.mainTexture = heightMap;

    //}

  }



}
