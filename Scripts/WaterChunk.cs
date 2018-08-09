using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterChunk : Chunk {

  public WaterData waterData;
  private MeshData waterMeshData;
  private WaterRenderer waterRenderer;

  public WaterChunk(LODInfo[] lodInfo, WaterData waterData, Vector2Int chunkCoords, Material waterMaterial, Transform parent = null)
    : base(lodInfo, waterData.size, chunkCoords,false, parent) {

    meshGameObject.layer = LayerMask.NameToLayer("Water");
    this.waterData = waterData;

    meshRenderer.sharedMaterial = waterMaterial;
    meshGameObject.name = "WaterChunk " + chunkCoords.ToString();

    WaterRenderer waterRenderer = meshGameObject.AddComponent<WaterRenderer>();
    waterRenderer.CreateCameras(meshRenderer, waterData.resolution);
  }


  public override void UpdateChunk(Vector3 viewerPosition) {
    base.UpdateChunk(viewerPosition);

    if (IsVisible()) {
      // Check if the current lod does not have a mesh requested, and request it
      if (!lodMeshes[lodIndex].requestedMesh) {
        RequestLodMeshData(lodMeshes[lodIndex], lodInfos[lodIndex]);
      }
      // If it has a mesh...
      else if (lodMeshes[lodIndex].hasMesh) {
        meshFilter.mesh = lodMeshes[lodIndex].mesh;
      }
    }

  }


  protected override void RequestLodMeshData(LODMesh lodMesh, LODInfo lodInfo) {
    lodMesh.RequestMeshData(
      () => MeshGenerator.GenerateMeshData(
        waterData.size + 1,
        lodInfo),
      OnLodMeshReady);
  }

  protected override void OnLodMeshReady(LODMesh lodMesh) {
    if (meshGameObject.activeSelf && lodLastUpdate == lodMesh.Lod) {
      meshFilter.mesh = lodMesh.mesh;
    }

    if (colliderIndex >= 0 && lodMeshes[colliderIndex].Lod == lodMesh.Lod) {
      if (distanceFromViewerLastUpdate < World.GetInstance().colliderDistanceThreshold) {
        meshCollider.sharedMesh = lodMesh.mesh;
      }
    }
    //if (!subscribed) {
    //  terrainData.OnValuesUpdated += Invalidate;
    //  subscribed = true;
    //}
  }
}
