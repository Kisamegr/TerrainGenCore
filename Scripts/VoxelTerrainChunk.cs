using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTerrainChunk :TerrainChunk {

  VoxelMeshData voxelMeshData;


  public override void Regenerate() {
    if (terrainData) {
      // Generate the heightmap
      CreateHeightMap();

      // Create the mesh data (vertices, triangles, etc...)
      voxelMeshData = VoxelMeshGenerator.GenerateVoxelMeshData();

      // Apply the mesh data to the mesh itself
      voxelMeshData.ApplyToMesh(mesh);

      // Update the material and position
      //UpdateTerrain();
    }
  }
}
