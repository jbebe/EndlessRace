using UnityEngine;

namespace Assets.ErEntities.ErTerrain
{
  public static class ErTerrainHelper
  {
    internal static ErTerrainContext CreateTerrainObject(ErTerrainContext context)
    {
      // Create new 3d object for the terrain
      context.Current = new GameObject($"Terrain #{context.Index}");
      context.Current.transform.SetParent(context.Parent.transform);
      context.Current.transform.position = context.Previous == null
        // Default: align first (behind) terrain behind the active one
        ? new Vector3(-ErTerrainGenerator.Config.Size / 2, 0, -(3 * ErTerrainGenerator.Config.Size) / 2)
        // Rest of the terrains will be just padded by height to their respective places
        : context.Previous.transform.position + new Vector3(0, 0, ErTerrainGenerator.Config.Size);

      // Set up terrain component
      var terrain = context.Current.AddComponent<Terrain>();
      var collider = context.Current.AddComponent<TerrainCollider>();

      // Generate heightmap
      GenerateHeightmap(context);

      // Assign terrainData to collider
      collider.terrainData = terrain.terrainData;

      return context;
    }

    private static void GenerateHeightmap(ErTerrainContext context)
    {
      // shorthands
      var current = context.Current;
      var size = ErTerrainGenerator.Config.Size;
      var textureSize = ErTerrainGenerator.Config.Texture.Width;
      var frequency = ErTerrainGenerator.Config.Frequency;
      var amplitude = ErTerrainGenerator.Config.Amplitude;
      var previousHeightMap = context.PreviousHeightMap;

      //
      // Terrain
      //

      var terrainComp = current.GetComponent<Terrain>();
      terrainComp.terrainData = new TerrainData();
      terrainComp.terrainData.heightmapResolution = ErTerrainGenerator.Config.Resolution;
      terrainComp.terrainData.size = new Vector3(size, ErTerrainGenerator.Config.Height, size);

      float[,] heightMap = new float[size, size];
      var texture = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, mipChain: false);
      var roadSize = 10;
      var hasPrevious = context.PreviousHeightMap != null;
      for (int h = 0; h < size; h++)
      {
        for (int w = 0; w < size; w++)
        {
          float noise;
          if (h == 0 && hasPrevious) // stitching
          {
            noise = previousHeightMap[size - 1, w];
          }
          else if (w > size / 2 - (roadSize / 2) && w < size / 2 + (roadSize / 2)) // road
          {
            noise = 0.2f;
          }
          else // terrain
          {
            noise = Mathf.PerlinNoise((h / (float)size) * frequency, (w / (float)size) * frequency) / amplitude;
          }
          heightMap[h, w] = noise;
          var gray = Mathf.Min(1.0f, noise * amplitude);
          texture.SetPixel(w, h, new Color(gray, gray, gray));
        }
      }
      context.CurrentHeightMap = heightMap;
      terrainComp.terrainData.SetHeights(0, 0, heightMap);

      //
      // Texture
      //

      texture.Apply();
      var terrainMaterial = new Material(Shader.Find("HDRP/TerrainLit"))
      {
        mainTexture = texture,
        name = "Heightmap texture",
        mainTextureScale = new Vector2((float)size / textureSize, (float)size / textureSize),
      };
      terrainMaterial.SetTextureOffset("_MainTex", new Vector2(-10, -10));
      terrainComp.materialTemplate = terrainMaterial;
    }
  }
}
