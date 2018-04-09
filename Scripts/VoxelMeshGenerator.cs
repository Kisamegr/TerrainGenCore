using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMeshGenerator {
  public static VoxelMeshData GenerateVoxelMeshData() {
    VoxelMeshData data = new VoxelMeshData();


    data.AddFace(Voxel.FaceDirection.Top);

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

  public void AddFace(Voxel.FaceDirection dir) {
    int vertexCount = vertices.Count;

    // Add the 4 new vertices
    vertices.AddRange(Voxel.FaceVertices(dir));

    // Add the 2 triangles
    triangles.Add(vertexCount);
    triangles.Add(vertexCount + 1);
    triangles.Add(vertexCount + 2);
    triangles.Add(vertexCount);
    triangles.Add(vertexCount + 2);
    triangles.Add(vertexCount + 3);
  }

  public void ApplyToMesh(Mesh mesh) {
    mesh.Clear(true);
    mesh.vertices  = vertices.ToArray();
    //mesh.uv        = uvs;
    mesh.triangles = triangles.ToArray();
    mesh.RecalculateNormals();
  }

}

public class Voxel {
  static Vector3[] originVertices = {
      new Vector3(-0.5f,  0.5f,  0.5f),
      new Vector3(0.5f,   0.5f,  0.5f),
      new Vector3(0.5f,   0.5f, -0.5f),
      new Vector3(-0.5f,  0.5f, -0.5f),
      new Vector3(-0.5f, -0.5f,  0.5f),
      new Vector3(0.5f,  -0.5f,  0.5f),
      new Vector3(0.5f,  -0.5f, -0.5f),
      new Vector3(-0.5f, -0.5f, -0.5f)
    };

  static int[][] faceVertexIndex = {
      new int[] {0, 1, 2, 3}, // Top
      new int[] {0, 1, 5, 4}, // Front
      new int[] {4, 5, 6, 7},
      new int[] {3, 2, 6, 7},
      new int[] {0, 3, 7, 4},
      new int[] {1, 5, 6, 2},


    };

  public static Vector3[] FaceVertices(FaceDirection dir) {
    Vector3[] vertices = new Vector3[4];
    for (int i = 0; i<4; i++) {
      vertices[i] = originVertices[faceVertexIndex[(int) dir][i]];
    }
    return vertices;
  }

  public enum FaceDirection {
    Top, Front, Bottom, Back, Left, Right
  };
}
