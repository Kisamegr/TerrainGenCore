using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshChunk : MonoBehaviour {

  protected MeshFilter   meshFilter;
  protected MeshRenderer meshRenderer;

  protected Mesh mesh;

  public void Awake() {
    meshFilter = GetComponent<MeshFilter>();
    meshRenderer = GetComponent<MeshRenderer>();

    mesh = new Mesh();
    meshFilter.mesh = mesh;
  }

  public abstract void Regenerate();
}
