using UnityEngine;

public abstract class Chunk {

  protected GameObject   meshGameObject;
  protected MeshFilter   meshFilter;
  protected MeshRenderer meshRenderer;

  protected int lodIndex = -1;
  protected int oldLodIndex = -1;
  protected LODMesh[] lodMeshes;
  protected LODInfo[] lodInfo;

  protected Bounds chunkBounds;
  protected bool visible = false;
  protected Vector3 chunkPosition;
  protected Vector2 chunkCoords;

  public Chunk(LODInfo[] lodInfo, int size, Vector2Int chunkCoords, Transform parent = null) {
    this.lodInfo = lodInfo;

    this.chunkCoords = chunkCoords;
    chunkPosition = new Vector3(chunkCoords.x * size, 0, chunkCoords.y * size);
    chunkBounds = new Bounds(chunkPosition, Vector3.one * size);

    meshGameObject = new GameObject();
    meshGameObject.transform.parent = parent;
    meshGameObject.transform.position = chunkPosition;
    meshFilter = meshGameObject.AddComponent<MeshFilter>();
    meshRenderer = meshGameObject.AddComponent<MeshRenderer>();

    lodMeshes = new LODMesh[lodInfo.Length];
    for (int i = 0; i<lodInfo.Length; i++) {
      lodMeshes[i] = new LODMesh();
    }
  }

  public void SetVisible(bool visible) {
    this.visible = visible;
    if (meshGameObject)
      meshGameObject.SetActive(visible);
  }

  public virtual void UpdateChunk(Vector3 viewerPosition) {
    oldLodIndex = lodIndex; 
    float distanceFromViewer = Mathf.Sqrt(chunkBounds.SqrDistance(viewerPosition));
    bool visible = distanceFromViewer <= lodInfo[lodInfo.Length-1].distance;

    if (visible) {
      // Find the lod
      for (int i = 0; i<lodInfo.Length; i++) {
        if (distanceFromViewer < lodInfo[i].distance) {
          lodIndex = i;
          break;
        }
      }
    }

    SetVisible(visible);
  }
}

public class LODMesh {
  public Mesh mesh = null;
  public bool requestedMesh = false;
  public bool hasMesh = false;
}

[System.Serializable]
public class LODInfo {
  public int lod;
  public int distance;
}
