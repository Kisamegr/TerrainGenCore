using System;
using System.Collections;
using UnityEngine;

public class VoxelMeshGenerator {
  public static VoxelMeshData GenerateVoxelMeshData(int size, int cellSize, LODInfo lodInfo, int[,] heightLevelMap = null) {
    int lodStep = lodInfo.LodStep;
    int lodSize = lodInfo.LodSize(size);
    int lodCellSize = cellSize * lodInfo.LodStep;

    float halfSizeScaled = lodCellSize * lodSize / 2;

    VoxelMeshData data = new VoxelMeshData();
    Vector3 centerOffset = new Vector3(-halfSizeScaled, 0, -halfSizeScaled);
  
    for (int z = 0; z < lodSize; z++) 
      for (int x = 0; x < lodSize; x++) 
        data.AddVoxel(x, z, lodStep, lodSize, lodCellSize, heightLevelMap, centerOffset);
        //data.AddVoxel(x, z, size, cellSize, heightLevelMap, centerOffset);
            
    return data;
  }
}

public class Voxel {
  // The sides of a voxel/cube
  public enum FaceSide {
    Front, Right, Back, Left, Top, Bottom
  };

  // The vertices of an 1x1x1 cube
  static Vector3[] originVertices = {
      new Vector3( 0.5f,   0,  0.5f),
      new Vector3(-0.5f,   0,  0.5f),
      new Vector3(-0.5f,  -1,  0.5f),
      new Vector3( 0.5f,  -1,  0.5f),
      new Vector3( 0.5f,   0, -0.5f),
      new Vector3( 0.5f,  -1, -0.5f),
      new Vector3(-0.5f,  -1, -0.5f),
      new Vector3(-0.5f,   0, -0.5f)
    };

  // The vertex indexes in the originVertices array for each cube side 
  static int[][] faceVertexIndex = {
      new int[] {0, 1, 2, 3}, // Front
      new int[] {0, 3, 5, 4}, // Right
      new int[] {4, 5, 6, 7}, // Back
      new int[] {7, 6, 2, 1}, // Left
      new int[] {0, 4, 7, 1}, // Top
      new int[] {6, 5, 3, 2}, // Bottom
    };

  public static Vector2Int[] voxelNeighbours = {
    new Vector2Int( 0,  1),
    new Vector2Int( 1,  0),
    new Vector2Int( 0, -1),
    new Vector2Int(-1,  0)
  };

  // Returns the face vertices of the given side
  public static Vector3[] FaceVertices(FaceSide side, Vector3 offset, float scale) {
    Vector3[] vertices = new Vector3[4];
    for (int i = 0; i<4; i++) {
      Vector3 originVertex =  originVertices[faceVertexIndex[(int) side][i]];
      vertices[i] = originVertex * scale + offset;
    }
    return vertices;
  }

}
