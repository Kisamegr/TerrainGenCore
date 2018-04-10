using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTerrainChunk : TerrainChunk {

  VoxelMeshData voxelMeshData;
  int[,] heightLevelMap;

  public override void Regenerate() {
    if (terrainData) {
      // Generate the heightmap
      CreateHeightMap();

      CreateHeightLevelMap();

      // Create the mesh data (vertices, triangles, etc...)
      voxelMeshData = VoxelMeshGenerator.GenerateVoxelMeshData(
        terrainData.size, terrainData.cellSize, heightLevelMap);

      // Apply the mesh data to the mesh itself
      voxelMeshData.ApplyToMesh(mesh);

      // Update the material and position
      UpdateTerrain();
    }
  }

  public void CreateHeightLevelMap() {
    int heightLayers = terrainData.heightLayersNumber - 1;
    heightLevelMap = new int[terrainData.size, terrainData.size];

    for (int z = 0; z < terrainData.size; z++) {
      for (int x = 0; x < terrainData.size; x++) {

        float height = 0;
        if (heightMap != null) {
          height = heightMapData[x, z]; // * terrainData.heightScale;
          if (terrainData.heightCurve != null)
            height *= terrainData.heightCurve.Evaluate(heightMapData[x, z]);
        }

        heightLevelMap[x,z] = Mathf.RoundToInt(height * heightLayers);
      }
    }
  }
}
