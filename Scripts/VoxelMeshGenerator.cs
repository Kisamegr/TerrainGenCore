using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMeshGenerator {
  public static VoxelMeshData GenerateVoxelMeshData(int mapSize, int cellSize, int[,] heightLevelMap = null) {
    VoxelMeshData data = new VoxelMeshData();
    float halfSizeScaled = cellSize * mapSize / 2;
    Vector3 centerOffset = new Vector3(-halfSizeScaled, 0, -halfSizeScaled);
  
    for (int z = 0; z < mapSize; z++) 
      for (int x = 0; x < mapSize; x++) 
        data.AddVoxel(x, z, mapSize, cellSize, heightLevelMap, centerOffset);
      
    return data;
  }
}

public class VoxelMeshData {
  public List<Vector3> vertices;
  public List<int> triangles;

  public VoxelMeshData() {
    vertices = new List<Vector3>();
    triangles = new List<int>();
  }

  public void AddVoxel(int x, int z, int mapSize, int cellSize, int[,] heightLevelMap, Vector3 offset) {
    // Get the current voxel height and position
    int currentVoxelHeight = heightLevelMap[x, z] * cellSize;
    Vector3 currentVoxelPosition = new Vector3(x*cellSize, currentVoxelHeight, z*cellSize) + offset;

    // Draw the top face, as it will always be visible
    AddFace(Voxel.FaceSide.Top, currentVoxelPosition, cellSize);

    // For each of the 4 side faces (Front, Right, Back, Left)
    for (int i = 0; i<4; i++) {
      // Get the correspondent neighbor voxel
      Vector2Int neighbourCoords = new Vector2Int(x, z) + Voxel.voxelNeighbours[i];
      // If the neighbor exists (it's not outside the chunk bounds)
      if (neighbourCoords.x >= 0 && neighbourCoords.y >= 0 && neighbourCoords.x < heightLevelMap.GetLength(0) && neighbourCoords.y < heightLevelMap.GetLength(1)) {
        // Get the neighbor's height
        int neighbourVoxelHeight = heightLevelMap[neighbourCoords.x, neighbourCoords.y] * cellSize;
        // If the current voxel's height is greater than the neighbor's height
        if (currentVoxelHeight > neighbourVoxelHeight) {
          Vector3 facePosition = currentVoxelPosition;
          // For every height level between them add a face
          while(facePosition.y > neighbourVoxelHeight) {
            AddFace((Voxel.FaceSide) i, facePosition, cellSize);
            facePosition.y -= cellSize;
          }
        }
      }
    }
  }

  void AddFace(Voxel.FaceSide side, Vector3 offset, float scale) {
    int vertexCount = vertices.Count;

    // Add the 4 new vertices
    vertices.AddRange(Voxel.FaceVertices(side, offset, scale));

    // Add the 2 triangles
    triangles.Add(vertexCount);
    triangles.Add(vertexCount + 1);
    triangles.Add(vertexCount + 2);
    triangles.Add(vertexCount);
    triangles.Add(vertexCount + 2);
    triangles.Add(vertexCount + 3);
  }

  public void ApplyToMesh(Mesh mesh) {
    if (mesh) {
      mesh.Clear(true);
      mesh.vertices  = vertices.ToArray();
      //mesh.uv        = uvs;
      mesh.triangles = triangles.ToArray();
      mesh.RecalculateNormals();
    }
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
