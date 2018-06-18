using UnityEngine;

public class TerrainMeshData : MeshData {
  public Vector3[] vertices;
  public Vector2[] uvs;
  public int[]     triangles;

  public TerrainMeshData(int size) {
    vertices  = new Vector3[size * size];
    uvs       = new Vector2[size * size];
    triangles = new int[(size - 1) * (size - 1) * 2 * 3];
  }

  public override void ApplyToMesh(Mesh mesh) {
    if (mesh) {
      mesh.Clear(true);
      mesh.vertices  = vertices;
      mesh.uv        = uvs;
      mesh.triangles = triangles;
      mesh.RecalculateNormals();
    }
  }
}
