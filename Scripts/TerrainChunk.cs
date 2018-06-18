using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System.Threading;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainChunk : Chunk {

  [Range(1,4)]
  public TerrainData terrainData;

  protected bool hasHeightMap;
  protected bool requestedHeightMap;
  protected float[,] heightMapData;

  protected bool subscribed = false;

  public TerrainChunk(LODInfo[] lodInfo, TerrainData terrainData, Vector2Int chunkCoords, Material terrainMaterial, bool useCollider, Transform parent = null)
      : base(lodInfo, terrainData.size, chunkCoords, useCollider, parent) {

    this.terrainData = terrainData;
    meshRenderer.sharedMaterial = terrainMaterial;
    meshGameObject.name = "TerrainChunk " + chunkCoords.ToString();
    hasHeightMap = false;
  }


  public override void UpdateChunk(Vector3 viewerPosition) {
    base.UpdateChunk(viewerPosition);

    // If the chunk is visible
    if (IsVisible()) {
      // If the chunk hasn't generated a height map yet, request it
      if (!requestedHeightMap) {
        ThreadedDataRequester.RequestData(() => GenerateHeightMap(), OnHeightMapReceived);
        requestedHeightMap = true;
      }
      // If it has a height map...
      else if (hasHeightMap) {
        // Check if the current lod does not have a mesh requested, and request it
        if (!lodMeshes[lodIndex].requestedMesh) {
          RequestLodMeshData(lodMeshes[lodIndex], lodInfos[lodIndex]);
        }
        // If it has a mesh...
        else if (lodMeshes[lodIndex].hasMesh) {
          meshFilter.mesh = lodMeshes[lodIndex].mesh;
        }

        // Check if we need to create a collider
        if (meshCollider != null && colliderIndex != -1) {
          // If the the distance is near the threshold...
          if (distanceFromViewerLastUpdate < World.GetInstance().colliderDistanceThreshold * 1.5f) {
            // Request the collider mesh if it doesn't exist
            if (!lodMeshes[colliderIndex].requestedMesh) {
              RequestLodMeshData(lodMeshes[colliderIndex], lodInfos[colliderIndex]);
            }
            // If the player reached the collider distance threshold, set the mesh
            if (distanceFromViewerLastUpdate < World.GetInstance().colliderDistanceThreshold) {
              if (lodMeshes[colliderIndex].hasMesh) {
                meshCollider.sharedMesh = lodMeshes[colliderIndex].mesh;
              }
            }
          }
        }
      }
    }
  }

  protected virtual void Invalidate() {
    requestedHeightMap = false;
    hasHeightMap = false;
    foreach (LODMesh info in lodMeshes) {
      info.requestedMesh = false;
      info.hasMesh = false;
    }

    if (IsVisible())
      UpdateChunk(lastViewerPosition);
  }

  protected virtual float[,] GenerateHeightMap() {
    return Noise.PerlinNoise(terrainData.size+1,
        terrainData.scale, terrainData.seed,
        terrainData.offsetX + chunkPosition.x,
        terrainData.offsetY - chunkPosition.z,
        terrainData.octaves, terrainData.persistense, terrainData.lacunarity,
        terrainData.normalizeMode);
  }

  protected virtual void OnHeightMapReceived(object heightMapObject) {
    heightMapData = (float[,]) heightMapObject;
    hasHeightMap = true;
    if (IsVisible())
      UpdateChunk(lastViewerPosition);
  }

  
  protected virtual void RequestLodMeshData(LODMesh lodMesh, LODInfo lodInfo) {
    lodMesh.RequestMeshData(
      () => MeshGenerator.GenerateMeshData(
        terrainData.size + 1,
        lodInfo,
        heightMapData,
        terrainData.heightScale,
        terrainData.heightCurve),
      OnLodMeshReady);
  }

  protected virtual void OnLodMeshReady(LODMesh lodMesh) {
    if (meshGameObject.activeSelf && lodLastUpdate == lodMesh.Lod) {
      meshFilter.mesh = lodMesh.mesh;
    }

    if (colliderIndex >= 0 && lodMeshes[colliderIndex].Lod == lodMesh.Lod) {
      if (distanceFromViewerLastUpdate < World.GetInstance().colliderDistanceThreshold) {
        meshCollider.sharedMesh = lodMesh.mesh;
      }
    }

    if (!subscribed) {
      terrainData.OnValuesUpdated += Invalidate;
      subscribed = true;
    }
  }


  /*protected void CreateHeightMapJob() {
    int size = lodSize + 1;

    Noise.GeneratePerlinJob noiseJob = new Noise.GeneratePerlinJob();
    noiseJob.size = size;
    noiseJob.lodStep = lodStep;
    noiseJob.scale = terrainData.scale;
    noiseJob.seed = terrainData.seed;
    noiseJob.offsetX = terrainData.offsetX + chunkPosition.x;
    noiseJob.offsetY = terrainData.offsetY - chunkPosition.z;
    noiseJob.octaves = terrainData.octaves;
    noiseJob.persistense = terrainData.persistense;
    noiseJob.lacunarity = terrainData.lacunarity;
    noiseJob.normalizeMode = terrainData.normalizeMode;

    NativeArray<float> nativeHeightMap = new NativeArray<float>(size*size, Allocator.Temp);
    noiseJob.nativeHeightMap = nativeHeightMap;

    JobHandle noiseJobHandle = noiseJob.Schedule();

    noiseJobHandle.Complete();

    heightMapData = new float[size, size];
    for (int i = 0; i<size*size; i++) {
      heightMapData[i/size, i%size] = nativeHeightMap[i];
    }
    //heightMapData = noiseJob.heightMap;

    nativeHeightMap.Dispose();
  }
  */

  //if (terrainData.usePosterization)
  //heightMapData = Noise.Posterize(heightMapData, terrainData.posterizeLevel);

  // Create the height map if it does not exist
  //if (!heightMap) {
  //  heightMap = new Texture2D(terrainData.size+1, terrainData.size+1) {
  //    filterMode = FilterMode.Point
  //  };
  //}
  //// Or check if the size has changed to update it
  //else if (heightMap.width != terrainData.size+1 || heightMap.height != terrainData.size+1)
  //  heightMap.Resize(terrainData.size+1, terrainData.size+1);

  // Generate the actual texture from the height map and set the material properties
  //TextureGenerator.GenerateTexture(heightMapData, ref heightMap);
  //meshRenderer.sharedMaterial.SetFloat("_HeightScale", terrainData.heightScale);
  //meshRenderer.material.mainTexture = heightMap;

  //}

  //}



}
