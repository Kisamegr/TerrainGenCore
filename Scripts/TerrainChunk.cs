using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System.Threading;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainChunk : Chunk {

  [Range(1,4)]
  public int lod = 1;
  public TerrainData terrainData;


  protected bool hasHeightMap;
  protected float[,] heightMapData;
  public Texture2D heightMap;

  protected int lodStep;
  protected int lodSize;

  public TerrainChunk(LODInfo[] lodInfo, TerrainData terrainData, Vector2Int chunkCoords, Material terrainMaterial, bool useCollider, Transform parent = null)
      : base(lodInfo, terrainData.size, chunkCoords, useCollider, parent) {

    this.terrainData = terrainData;
    meshRenderer.sharedMaterial = terrainMaterial;
    meshGameObject.name = "TerrainChunk " + chunkCoords.ToString();
    hasHeightMap = false;

    terrainData.OnValuesUpdated += Regenerate;
  }


  public override void UpdateChunk(Vector3 viewerPosition) {
    base.UpdateChunk(viewerPosition);

    // If the chunk is visible
    if (visible) {
      if (oldLodIndex != lodIndex) {
        // Set the lod variables based on the current lod info
        lod = lodInfo[lodIndex].Lod;
        lodStep = lodInfo[lodIndex].LodStep;
        lodSize = lodInfo[lodIndex].LodSize(terrainData.size);
      }

      // If the current lod does not have a mesh requested
      if (!lodMeshes[lodIndex].requestedMesh) {
        GenerateMeshData();
        lodMeshes[lodIndex].requestedMesh = true;
      }
      else if (lodMeshes[lodIndex].hasMesh) {
        meshFilter.mesh = lodMeshes[lodIndex].mesh;
        if (meshCollider) {
          if(distanceFromViewerLastUpdate < World.GetInstance().colliderDistanceThreshold)
            meshCollider.sharedMesh = lodMeshes[lodIndex].mesh;
        }
      }
    }
  }

  protected virtual void GenerateMeshData() {
    if (World.GetInstance().renderSystem == World.RenderSystem.Threaded) {
      ThreadedDataRequester.RequestData(() => GenerateMeshDataThreaded(), OnMeshDataGenerated);
    }
    else {
      Debug.Log("JOB SYSTEM NOT IMPLEMENTED YET !!!");
    }
  }

  protected virtual void Regenerate() {
    hasHeightMap = false;
    foreach (LODMesh info in lodMeshes) {
      info.requestedMesh = false;
      info.hasMesh = false;
    }

    if (visible)
      GenerateMeshData();

  }

  protected virtual object GenerateMeshDataThreaded() {
    // Generate the height map
    if (!hasHeightMap) {
      heightMapData = CreateHeightMap();
      hasHeightMap = true;
    }

    // Generate the mesh data
    return MeshGenerator.GenerateMeshData(
        terrainData.size + 1,
        lodInfo[lodIndex],
        heightMapData,
        terrainData.heightScale,
        terrainData.heightCurve);
  }




  protected virtual void OnMeshDataGenerated(object meshDataObject) {
    MeshData meshData = (MeshData) meshDataObject;

    Mesh mesh = new Mesh();
    meshData.ApplyToMesh(mesh);
    // Generate the terrain mesh and set it to the current lod
    lodMeshes[lodIndex].mesh = mesh;
    lodMeshes[lodIndex].hasMesh = true;

    meshFilter.mesh = lodMeshes[lodIndex].mesh;
    if (meshCollider)
      if (distanceFromViewerLastUpdate < World.GetInstance().colliderDistanceThreshold)
        meshCollider.sharedMesh = lodMeshes[lodIndex].mesh;
  }

  protected float[,] CreateHeightMap() {
    return Noise.PerlinNoise(terrainData.size+1,
        terrainData.scale, terrainData.seed,
        terrainData.offsetX + chunkPosition.x,
        terrainData.offsetY - chunkPosition.z,
        terrainData.octaves, terrainData.persistense, terrainData.lacunarity,
        terrainData.normalizeMode);
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
