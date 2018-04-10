using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainChunk : MeshChunk {

  public TerrainData terrainData;

  protected MeshData meshData;
  protected Texture2D heightMap;
  protected float[,] heightMapData;

  public void OnValidate() {
    if (Application.isPlaying) {
      if (terrainData != null) 
        terrainData.OnValuesUpdated += Regenerate;
    }
  }

  public override void Regenerate() {
    if (terrainData) {
      // Generate the heightmap
      CreateHeightMap();

      // Create the mesh data (vertices, triangles, etc...)
      meshData = MeshGenerator.GenerateMeshData(
        terrainData.size, heightMapData, terrainData.heightScale, terrainData.heightCurve);

      // Apply the mesh data to the mesh itself
      meshData.ApplyToMesh(mesh);

      // Update the material and position
      UpdateTerrain();
    }
    else
      Debug.Log("No terrain data specified!!!");
  }

  public void UpdateTerrain() {
    if (meshRenderer && terrainData) {
      terrainData.ApplyToMaterial(meshRenderer.sharedMaterial);
      transform.position = new Vector3(0, -terrainData.HeightOffsetScaled, 0);
    }
  }

  protected void CreateHeightMap() {
    // Generate the perlin noise height map
    heightMapData = Noise.PerlinNoise(terrainData.size, terrainData.size, terrainData.scale,
      terrainData.seed, terrainData.offsetX, terrainData.offsetY,
      terrainData.octaves, terrainData.persistense, terrainData.lacunarity);

    if (terrainData.usePosterization)
      heightMapData = Noise.Posterize(heightMapData, terrainData.posterizeLevel);

    // Create the heightmap if it does not exist
    if (!heightMap) {
      heightMap = new Texture2D(terrainData.size, terrainData.size);
      heightMap.filterMode = FilterMode.Point;
    }
    // Or check if the size has changed to update it
    else if (heightMap.width != terrainData.size || heightMap.height != terrainData.size)
      heightMap.Resize(terrainData.size, terrainData.size);

    // Generate the actual texture from the height map and set the material properties
    //TextureGenerator.GenerateTexture(heightMapData, ref heightMap);
    //meshRenderer.sharedMaterial.SetFloat("_HeightScale", terrainData.heightScale);
    //meshRenderer.sharedMaterial.mainTexture = heightMap;
  }

}
