using System;
using UnityEngine;

public class VoxelTerrainChunk : TerrainChunk {
  private int[,] heightLevelMapData;

  public VoxelTerrainChunk(LODInfo[] lodInfo, TerrainData terrainData, Vector2Int chunkCoords, Material terrainMaterial, bool useCollider, Transform parent = null)
    : base(lodInfo, terrainData, chunkCoords, terrainMaterial, useCollider, parent) {
  }

  protected override void OnHeightMapReceived(object heightMapObject) {
    heightMapData = (float[,]) heightMapObject;
    ThreadedDataRequester.RequestData(() => CreateHeightLevelMap(heightMapData), OnHeightLevelMapDataReceived);
  }

  // Rounds the real height values to integer height-levels 
  protected int[,] CreateHeightLevelMap(float[,] heightMapData) {
    AnimationCurve heightCurve = new AnimationCurve(terrainData.heightCurve.keys);
    int size = terrainData.size;
    // The size of the map depends on the lod because the less detailed chunks
    // have bigger voxels, hence the available space is filled with less voxels
    int [,] heightLevelMap = new int[size, size];

    for (int z = 0; z < size; z++) {
      for (int x = 0; x < size; x++) {
        float height = 0;
        // Get the original height values, and pass it through the height curve
        if (heightMapData != null) {
          height = heightMapData[x, z]; // * terrainData.heightScale;
          if (heightCurve != null)
            height *= heightCurve.Evaluate(height);
        }
        // Then round the value to the nearest height level
        heightLevelMap[x, z] = Mathf.RoundToInt(height * terrainData.HeightLayersNumber);
      }
    }

    return heightLevelMap;
  }

  protected void OnHeightLevelMapDataReceived(object heightLevelMapObject) {
    heightLevelMapData = (int[,]) heightLevelMapObject;
    hasHeightMap = true;
    if (IsVisible())
      UpdateChunk(lastViewerPosition);
  }

  protected override void RequestLodMeshData(LODMesh lodMesh, LODInfo lodInfo) {
    lodMesh.RequestMeshData(
      () => VoxelMeshGenerator.GenerateVoxelMeshData(
        terrainData.size,
        terrainData.cellSize,
        lodInfo,
        heightLevelMapData),
      OnLodMeshReady);
  }

  /*
protected override void GenerateMeshData() {
  // Set the other lod variables
  lodCellSize = terrainData.cellSize * lodStep;
  lodHeightLayers = terrainData.heightLayersNumber / lodStep;
  base.GenerateMeshData();
}

protected override object GenerateMeshDataThreaded() {
  float[,] heightMapData = CreateHeightMap();

  int[,] heightLevelMap = CreateHeightLevelMap(heightMapData);

  return VoxelMeshGenerator.GenerateVoxelMeshData(
    lodSize, lodCellSize, heightLevelMap);

}

protected override void OnMeshDataGenerated(object meshDataObject) {
  VoxelMeshData meshData = (VoxelMeshData) meshDataObject;

  Mesh mesh = new Mesh();
  meshData.ApplyToMesh(mesh);
  // Generate the terrain mesh and set it to the current lod
  lodMeshes[lodIndex].mesh = mesh;
  lodMeshes[lodIndex].hasMesh = true;

  meshFilter.mesh = lodMeshes[lodIndex].mesh;
}
*/

  /*
  protected override Mesh GenerateTerrainMesh() {
    // Set the other lod variables
    lodCellSize = terrainData.cellSize * lodStep;
    lodHeightLayers = terrainData.heightLayersNumber / lodStep;

    // Create the height map
    RequestHeightMap();

    // Create the height level map
    CreateHeightLevelMap();

    // Create the mesh data (vertices, triangles, etc...)
    voxelMeshData = VoxelMeshGenerator.GenerateVoxelMeshData(
      lodSize, lodCellSize, heightLevelMap);

    // Apply the mesh data to the mesh itself
    Mesh mesh = new Mesh();
    voxelMeshData.ApplyToMesh(mesh);
    return mesh;
  }
  */

}
