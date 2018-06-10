using System;
using UnityEngine;

public abstract class Chunk {

  protected GameObject   meshGameObject;
  protected MeshFilter   meshFilter;
  protected MeshRenderer meshRenderer;
  protected MeshCollider meshCollider;

  protected int lodIndex = -1;
  protected int oldLodIndex = -1;
  protected LODMesh[] lodMeshes;
  protected LODInfo[] lodInfo;

  protected float distanceFromViewerLastUpdate;
  protected Vector3 lastViewerPosition;
  protected Bounds chunkBounds;
  protected bool visible = false;
  protected Vector3 chunkPosition;
  protected Vector2 chunkCoords;

  public Chunk(LODInfo[] lodInfo, int size, Vector2Int chunkCoords, bool useCollider, Transform parent = null) {
    this.lodInfo = lodInfo;

    this.chunkCoords = chunkCoords;
    chunkPosition = new Vector3(chunkCoords.x * size, 0, chunkCoords.y * size);
    chunkBounds = new Bounds(chunkPosition, Vector3.one * size);

    meshGameObject = new GameObject();
    meshGameObject.transform.parent = parent;
    meshGameObject.transform.position = chunkPosition;
    meshFilter = meshGameObject.AddComponent<MeshFilter>();
    meshRenderer = meshGameObject.AddComponent<MeshRenderer>();
    if(useCollider)
      meshCollider = meshGameObject.AddComponent<MeshCollider>();

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
    lastViewerPosition = viewerPosition;
    oldLodIndex = lodIndex; 
    distanceFromViewerLastUpdate = Mathf.Sqrt(chunkBounds.SqrDistance(viewerPosition));
    bool visible = distanceFromViewerLastUpdate <= lodInfo[lodInfo.Length-1].Distance;

    if (visible) {
      // Find the lod
      for (int i = 0; i<lodInfo.Length; i++) {
        if (distanceFromViewerLastUpdate < lodInfo[i].Distance) {
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
  [SerializeField]
  private int lod;
  [SerializeField]
  private int distance;
  [SerializeField]
  private bool useForCollider;

  public int Lod { get => lod; }
  public int Distance { get => distance; }
  public bool UseForCollider { get => useForCollider; }

  public int LodStep {
    get {
      return (int) Math.Pow(2, Lod-1);
    }
  }

  public int LodSize(int size) {
    return (int) Math.Ceiling((double) size / LodStep);
  }
}
