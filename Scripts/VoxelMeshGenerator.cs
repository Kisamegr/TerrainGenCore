using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMeshGenerator {
  public static VoxelMeshData GenerateVoxelMeshData(int size, float cellSize, int[,] heightLevelMap = null) {
    float halfSizeScaled = cellSize * size / 2;

    VoxelMeshData data = new VoxelMeshData();


    for (int z = 0; z < size; z++) {
      for (int x = 0; x < size; x++) {

        Vector3 pos = new Vector3(x*cellSize - halfSizeScaled,
                                  heightLevelMap[x,z] * cellSize,
                                  z*cellSize - halfSizeScaled);

        data.AddVoxel(new Vector2Int(x, z), pos, cellSize, size, heightLevelMap);
      }
    }

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

  public void AddVoxel(Vector2Int coords, Vector3 offset, float scale, int mapSize, int[,] heightLevelMap) {

    // Draw the top face
    AddFace(Voxel.FaceSide.Top, offset, scale);

    int currentHeight = heightLevelMap[coords.x, coords.y];

    for(int i=0; i<4; i++) {
      Vector2Int neighbourCoords = Voxel.voxelNeighbours[i] + coords;

      if(neighbourCoords.x >= 0 && neighbourCoords.y >= 0 && neighbourCoords.x < heightLevelMap.GetLength(0) && neighbourCoords.y < heightLevelMap.GetLength(1)) {
        int neighbourHeight = heightLevelMap[neighbourCoords.x, neighbourCoords.y];

        if(currentHeight > neighbourHeight)
          AddFace((Voxel.FaceSide)i, offset, scale);
      }
    }



    //foreach (Voxel.FaceSide dir in Enum.GetValues(typeof(Voxel.FaceSide))) {
    //  AddFace(dir, Vector3.zero, scale);
    //}
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
      new Vector3( 0.5f,  0.5f,  0.5f),
      new Vector3(-0.5f,  0.5f,  0.5f),
      new Vector3(-0.5f, -0.5f,  0.5f),
      new Vector3( 0.5f, -0.5f,  0.5f),
      new Vector3( 0.5f,  0.5f, -0.5f),
      new Vector3( 0.5f, -0.5f, -0.5f),
      new Vector3(-0.5f, -0.5f, -0.5f),
      new Vector3(-0.5f,  0.5f, -0.5f)
    };

  // The vertex indeces in the originVertices array fpr each cube side 
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
