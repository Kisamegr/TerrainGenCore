using UnityEngine;

public static class MeshGenerator {

  public static MeshData GenerateMeshData(int width, int length, float[,] heightMap = null, float heightScale = 0, AnimationCurve heightCurve = null) {
    int triangleVertexCounter = 0;
    float halfWidth = width / 2f;
    float halfLength = length / 2f;

    MeshData data = new MeshData(width, length);

    for (int z = 0; z < length; z++) {
      for (int x = 0; x < width; x++) {
        int vertexIndex = z*width + x;

        float height = 0;
        if(heightMap != null) {
          height = heightMap[x, z] * heightScale;
          if (heightCurve != null)
            height *= heightCurve.Evaluate(heightMap[x, z]);
        }

        data.vertices[vertexIndex] = new Vector3(x - halfWidth,
                                                 height,
                                                 z - halfLength);

        data.uvs[vertexIndex]      = new Vector2(x / (float) width, z / (float) length);

        if ((x < width-1) && (z < length-1)) {
          int triangleIndex = triangleVertexCounter * 6;
          data.triangles[triangleIndex]     = vertexIndex;
          data.triangles[triangleIndex + 1] = vertexIndex + width;
          data.triangles[triangleIndex + 2] = vertexIndex + width + 1;
          data.triangles[triangleIndex + 3] = vertexIndex + width + 1;
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
    mesh.Clear(true);
    mesh.vertices  = vertices;
    mesh.uv        = uvs;
    mesh.triangles = triangles;
    mesh.RecalculateNormals();
  }

}
