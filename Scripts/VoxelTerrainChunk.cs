using System;
using UnityEngine;

public class VoxelTerrainChunk :TerrainChunk {

  VoxelMeshData voxelMeshData;
  int[,] heightLevelMap;

  protected int lodCellSize;
  protected int lodHeightLayers;

  public VoxelTerrainChunk(LODInfo[] lodInfo, TerrainData terrainData, int size, Vector2Int chunkCoords, Material terrainMaterial, Transform parent = null)
    : base(lodInfo, terrainData, size, chunkCoords, terrainMaterial, parent) {
  }

  protected override Mesh GenerateTerrainMesh() {
    // Set the other lod variables
    lodCellSize = terrainData.cellSize * lodStep;
    lodHeightLayers = terrainData.heightLayersNumber / lodStep;

    // Create the height map
    CreateHeightMap();

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

  // Rounds the real height values to integer height-levels 
  public void CreateHeightLevelMap() {
    // The size of the map depends on the lod because the less detailed chunks
    // have bigger voxels, hence the available space is filled with less voxels
    heightLevelMap = new int[lodSize, lodSize];

    for (int z = 0; z < lodSize; z++) {
      for (int x = 0; x < lodSize; x++) {
        float height = 0;
        // Get the original height values, and pass it through the height curve
        if (heightMap != null) {
          height = heightMapData[x*lodStep, z*lodStep]; // * terrainData.heightScale;
          if (terrainData.heightCurve != null)
            height *= terrainData.heightCurve.Evaluate(height);
        }
        // Then round the value to the nearest height level
        heightLevelMap[x, z] = Mathf.RoundToInt(height * lodHeightLayers);
      }
    }
  }
}
