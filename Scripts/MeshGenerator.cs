using System;
using UnityEngine;

public static class MeshGenerator {

  public static MeshData GenerateMeshData(int size, LODInfo lodInfo, float[,] heightMap = null, float heightScale = 0, AnimationCurve heightCurve = null) {
    int lodStep = lodInfo.LodStep;
    int lodSize = lodInfo.LodSize(size);

    int triangleVertexCounter = 0;
    float halfSize = (lodSize-1) / 2f;
    MeshData data = new MeshData(lodSize);

    AnimationCurve copyHeightCurve = null;
    if (heightCurve != null)
      copyHeightCurve = new AnimationCurve(heightCurve.keys);

    for (int z = 0; z < lodSize; z++) {
      for (int x = 0; x < lodSize; x++) {
        int vertexIndex = z*lodSize + x;

        float height = 0;
        if (heightMap != null) {
          float heightSample = heightMap[x*lodStep, z*lodStep];
          height = heightSample * heightScale;
          if (copyHeightCurve != null)
            height *= copyHeightCurve.Evaluate(heightSample);
        }

        data.vertices[vertexIndex] = new Vector3((x - halfSize) * lodStep,
                                                 height,
                                                 (z - halfSize) * lodStep);

        data.uvs[vertexIndex]      = new Vector2(x / (float) lodSize, z / (float) lodSize);

        if ((x < lodSize-1) && (z < lodSize-1)) {
          int triangleIndex = triangleVertexCounter * 6;
          data.triangles[triangleIndex]     = vertexIndex;
          data.triangles[triangleIndex + 1] = vertexIndex + lodSize;
          data.triangles[triangleIndex + 2] = vertexIndex + lodSize + 1;
          data.triangles[triangleIndex + 3] = vertexIndex + lodSize + 1;
          data.triangles[triangleIndex + 4] = vertexIndex + 1;
          data.triangles[triangleIndex + 5] = vertexIndex;
          triangleVertexCounter++;
        }
      }
    }

    return data;
  }



}

public class MeshData {
  public int       size;
  public Vector3[] vertices;
  public Vector2[] uvs;
  public int[]     triangles;

  public MeshData(int size) {
    this.size = size;
    vertices  = new Vector3[size * size];
    uvs       = new Vector2[size * size];
    triangles = new int[(size - 1) * (size - 1) * 2 * 3];
  }

  public void ApplyToMesh(Mesh mesh) {
    if (mesh) {
      mesh.Clear(true);
      mesh.vertices  = vertices;
      mesh.uv        = uvs;
      mesh.triangles = triangles;
      mesh.RecalculateNormals();
    }
  }

}
