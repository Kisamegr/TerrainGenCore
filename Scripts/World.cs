﻿using UnityEngine;
using System.Collections.Generic;

public class World :MonoBehaviour {

  public Transform viewer;
  public TerrainData terrainData;
  public Material terrainMaterial;

  [Header("World Properties")]
  public LODInfo[] lodInfo;

  private int maxViewDistance;
  private int chunkNumber;

  private Dictionary<Vector2Int, TerrainChunk> terrainChunkDict = new Dictionary<Vector2Int, TerrainChunk>();
  private List<TerrainChunk> visibleChunksLastUpdate = new List<TerrainChunk>();

  static World _instance;
  private void Awake() {
    _instance = this;
  }

  public static World GetInstance() {
    return _instance;
  }

  private void OnValidate() {
    if (lodInfo.Length > 0) {
      maxViewDistance = lodInfo[lodInfo.Length-1].distance;
      chunkNumber = maxViewDistance / terrainData.size;
    }
  }

  private void Update() {
    UpdateTerrainChunks();
  }

  private void UpdateTerrainChunks() {
    // Reset the visible chunks from from the last update
    visibleChunksLastUpdate.ForEach(delegate (TerrainChunk chunk) {
      chunk.SetVisible(false);
    });

    // Clear the list
    visibleChunksLastUpdate.Clear();

    // Find the current chunk that the viewer is on
    int currentChunkX = Mathf.RoundToInt(viewer.position.x / terrainData.size);
    int currentChunkY = Mathf.RoundToInt(viewer.position.z / terrainData.size);

    // Parse all the visible chunks and create/update them
    for (int x = -chunkNumber; x <= chunkNumber; x++) {
      for (int y = -chunkNumber; y <= chunkNumber; y++) {
        TerrainChunk chunk = null;

        // Current chunk coordinates
        Vector2Int viewChunkCoords = new Vector2Int(x + currentChunkX, y + currentChunkY);

        // If the chunk exists in the dictionary, try and get it
        if (terrainChunkDict.ContainsKey(viewChunkCoords)) {
          terrainChunkDict.TryGetValue(viewChunkCoords, out chunk);
        }
        // Else, create it and add it to the dictionary
        else {
          chunk = terrainData.useVoxels
            ? new VoxelTerrainChunk(lodInfo, terrainData, terrainData.size, viewChunkCoords, terrainMaterial, transform)
            : new TerrainChunk(lodInfo, terrainData, terrainData.size, viewChunkCoords, terrainMaterial, transform);

          terrainChunkDict.Add(viewChunkCoords, chunk);
        }

        // Then update the chunk and add it to the visible last update list
        if (chunk != null) {
          chunk.UpdateChunk(viewer.position);
          visibleChunksLastUpdate.Add(chunk);
        }
      }
    }
  }

}
