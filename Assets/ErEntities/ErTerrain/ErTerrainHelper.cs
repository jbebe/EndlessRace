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
        ? new Vector3(-ErTerrainGenerator.Config.Width / 2, 0, -(3 * ErTerrainGenerator.Config.Height) / 2)
        // Rest of the terrains will be just padded by height to their respective places
        : context.Previous.transform.position + new Vector3(0, 0, ErTerrainGenerator.Config.Height);

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
      var width = ErTerrainGenerator.Config.Width;
      var height = ErTerrainGenerator.Config.Height;
      var textureWidth = ErTerrainGenerator.Config.Texture.Width;
      var textureHeight = ErTerrainGenerator.Config.Texture.Height;
      var frequency = ErTerrainGenerator.Config.Frequency;
      var amplitude = ErTerrainGenerator.Config.Amplitude;
      var previousHeightMap = context.PreviousHeightMap;

      //
      // Terrain
      //

      var terrainComp = current.GetComponent<Terrain>();
      terrainComp.terrainData = new TerrainData
      {
        size = new Vector3(width, ErTerrainGenerator.Config.TerrainBoxHeight, height),
        heightmapResolution = ErTerrainGenerator.Config.Resolution,
      };

      float[,] heightMap = new float[height, width];
      var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, mipChain: false)
      {
        alphaIsTransparency = true,
      };
      var roadSize = 10;
      var hasPrevious = context.PreviousHeightMap != null;
      for (int h = 0; h < height; h++)
      {
        for (int w = 0; w < width; w++)
        {
          float noise;
          if (h == 0 && hasPrevious) // stitching
          {
            noise = previousHeightMap[height - 1, w];
          }
          else if (w > width / 2 - (roadSize / 2) && w < width / 2 + (roadSize / 2)) // road
          {
            noise = 0.2f;
          }
          else // terrain
          {
            noise = Mathf.PerlinNoise((h / (float)height) * frequency, (w / (float)width) * frequency) / amplitude;
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
      var terrainMaterial = new Material(Shader.Find("Standard"))
      {
        mainTexture = texture,
        name = "Heightmap texture",
        mainTextureScale = new Vector2(33.0f / 128.0f, 33.0f / 128.0f),
      };
      terrainComp.materialTemplate = terrainMaterial;
    }
  }
}
