using UnityEngine;

public static class MeshGenerator {

  public static MeshData GenerateMeshData(int size, float[,] heightMap = null, float heightScale = 0, AnimationCurve heightCurve = null) {
    int triangleVertexCounter = 0;
    float halfSize = (size-1) / 2f;
    MeshData data = new MeshData(size, size);

    for (int z = 0; z < size; z++) {
      for (int x = 0; x < size; x++) {
        int vertexIndex = z*size + x;

        float height = 0;
        if (heightMap != null) {
          height = heightMap[x, z] * heightScale;
          if (heightCurve != null)
            height *= heightCurve.Evaluate(heightMap[x, z]);
        }

        data.vertices[vertexIndex] = new Vector3(x - halfSize,
                                                 height,
                                                 z - halfSize);

        data.uvs[vertexIndex]      = new Vector2(x / (float) size, z / (float) size);

        if ((x < size-1) && (z < size-1)) {
          int triangleIndex = triangleVertexCounter * 6;
          data.triangles[triangleIndex]     = vertexIndex;
          data.triangles[triangleIndex + 1] = vertexIndex + size;
          data.triangles[triangleIndex + 2] = vertexIndex + size + 1;
          data.triangles[triangleIndex + 3] = vertexIndex + size + 1;
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
  public Vector3[] vertices;
  public Vector2[] uvs;
  public int[]     triangles;

  public MeshData(int width, int length) {
    vertices  = new Vector3[width * length];
    uvs       = new Vector2[width * length];
    triangles = new int[(width - 1) * (length - 1) * 2 * 3];
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
