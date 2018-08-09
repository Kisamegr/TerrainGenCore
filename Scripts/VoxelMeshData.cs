using System.Collections.Generic;
using UnityEngine;

public class VoxelMeshData : MeshData {
  public List<Vector3> vertices;
  public List<int> triangles;

  public VoxelMeshData() {
    vertices = new List<Vector3>();
    triangles = new List<int>();
  }

  public void AddVoxel(int x, int z, int lodStep, int mapSize, int cellSize, int[,] heightLevelMap, Vector3 offset) {
    // Get the current voxel height and position
    int currentVoxelHeight = heightLevelMap[x*lodStep, z*lodStep] * cellSize / lodStep;
    Vector3 currentVoxelPosition = new Vector3(x*cellSize, currentVoxelHeight, z*cellSize) + offset;

    // Draw the top face, as it will always be visible
    AddFace(Voxel.FaceSide.Top, currentVoxelPosition, cellSize);

    // For each of the 4 side faces (Front, Right, Back, Left)
    for (int i = 0; i<4; i++) {
      // Get the correspondent neighbor voxel
      Vector2Int neighbourCoords = (new Vector2Int(x, z) + Voxel.voxelNeighbours[i]) * lodStep;
      // If the neighbor exists (it's not outside the chunk bounds)
      if (neighbourCoords.x >= 0 && neighbourCoords.y >= 0 && neighbourCoords.x < heightLevelMap.GetLength(0) && neighbourCoords.y < heightLevelMap.GetLength(1)) {
        // Get the neighbor's height
        int neighbourVoxelHeight = heightLevelMap[neighbourCoords.x, neighbourCoords.y] * cellSize / lodStep;
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

  public override void ApplyToMesh(Mesh mesh) {
    if (mesh) {
      mesh.Clear(true);
      mesh.vertices  = vertices.ToArray();
      //mesh.uv        = uvs;
      mesh.triangles = triangles.ToArray();
      mesh.RecalculateNormals();
    }
  }

}
