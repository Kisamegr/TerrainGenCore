using UnityEngine;
using System;
using System.Linq;

public class LODMesh {
  [SerializeField]
  private int lod;
  public Mesh mesh = null;
  public bool requestedMesh = false;
  public bool hasMesh = false;
  public Action<LODMesh> MeshIsReady;

  public LODMesh(int lod) {
    this.lod = lod;
  }

  public int Lod { get => lod; }

  public void RequestMeshData(Func<object> generateFunction, Action<LODMesh> callback) {
    ThreadedDataRequester.RequestData(generateFunction, OnMeshDataReceived);
    requestedMesh = true;
    MeshIsReady += callback;
  }

  public void OnMeshDataReceived(object meshDataObject) {
    MeshData meshData = (MeshData) meshDataObject;
    Mesh mesh = new Mesh();
    meshData.ApplyToMesh(mesh);
    this.mesh = mesh;
    this.hasMesh = true;

    MeshIsReady(this);
  }

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

  public void SetValues(int lod, int distance, bool useForCollider) {
    this.lod = lod;
    this.distance = distance;
    this.useForCollider = useForCollider;
  }

  public LODInfo(LODInfo other) {
    lod = other.lod;
    distance = other.distance;
    useForCollider = other.useForCollider;
  }

  public override bool Equals(object obj) {
    if (obj == null || GetType() != obj.GetType())
      return false;

    LODInfo other = (LODInfo) obj;
    return lod == other.lod && distance == other.distance && useForCollider == other.useForCollider;
  }
}

public abstract class MeshData {
  public abstract void ApplyToMesh(Mesh mesh);
}
