using UnityEngine;

public abstract class Chunk {

  public GameObject   meshGameObject;
  protected MeshFilter   meshFilter;
  protected MeshRenderer meshRenderer;
  protected MeshCollider meshCollider;

  protected int lodIndex = -1;
  protected int oldLodIndex = -1;
  protected int lodLastUpdate = 1;

  protected LODMesh[] lodMeshes;
  protected LODInfo[] lodInfos;

  protected int colliderIndex = -1;

  protected float distanceFromViewerLastUpdate;
  protected Vector3 lastViewerPosition;
  protected Bounds chunkBounds;
  protected Vector3 chunkPosition;
  protected Vector2 chunkCoords;

  public Chunk(LODInfo[] lodInfo, int size, Vector2Int chunkCoords, bool useCollider, Transform parent = null) {
    this.lodInfos = lodInfo;

    this.chunkCoords = chunkCoords;
    chunkPosition = new Vector3(chunkCoords.x * size, 0, chunkCoords.y * size);
    chunkBounds = new Bounds(chunkPosition, Vector3.one * size);

    meshGameObject = new GameObject();
    meshGameObject.transform.parent = parent;
    meshGameObject.transform.position = chunkPosition;
    meshFilter = meshGameObject.AddComponent<MeshFilter>();
    meshRenderer = meshGameObject.AddComponent<MeshRenderer>();
    if (useCollider)
      meshCollider = meshGameObject.AddComponent<MeshCollider>();

    lodMeshes = new LODMesh[lodInfo.Length];
    for (int i = 0; i<lodInfo.Length; i++) {
      lodMeshes[i] = new LODMesh(lodInfo[i].Lod);
      if (useCollider && lodInfo[i].UseForCollider)
        colliderIndex = i;
    }
  }

  public void SetVisible(bool visible) {
    if (meshGameObject)
      meshGameObject.SetActive(visible);
  }

  public bool IsVisible() {
    if (meshGameObject)
      return meshGameObject.activeSelf;
    return false;
  }

  public virtual void UpdateChunk(Vector3 viewerPosition) {
    lastViewerPosition = viewerPosition;
    oldLodIndex = lodIndex;
    distanceFromViewerLastUpdate = Mathf.Sqrt(chunkBounds.SqrDistance(viewerPosition));
    bool visible = distanceFromViewerLastUpdate <= lodInfos[lodInfos.Length-1].Distance;

    if (visible) {
      // Find the current lod
      for (int i = 0; i<lodInfos.Length; i++) {
        if (distanceFromViewerLastUpdate < lodInfos[i].Distance) {
          lodIndex = i;
          break;
        }
      }

      lodLastUpdate = lodInfos[lodIndex].Lod;
    }

    SetVisible(visible);
  }

  public LODInfo InfoFromLod(int lod) {
    for (int i = 0; i<lodInfos.Length; i++)
      if (lodInfos[i].Lod == lod)
        return lodInfos[i];
    return null;
  }
}


