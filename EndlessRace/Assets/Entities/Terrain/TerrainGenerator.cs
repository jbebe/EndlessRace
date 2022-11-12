using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EndlessRace
{

  class TerrainInfo {
    public readonly int Index;
    public readonly float EndZ;

    public TerrainInfo(int index, float endZ)
    {
      Index = index;
      EndZ = endZ;
    }
  }

  public class TerrainGenerator : MonoBehaviour
  {
    public GameObject Player;
    private Queue<TerrainInfo> TerrainQueue = new(2);
    private TerrainInfo LatestTerrainInfo;

    // Start is called before the first frame update
    void Start()
    {
      var terrainInfo = new TerrainInfo(0, 0);
      terrainInfo = CreateTerrainEntity(gameObject, terrainInfo);
      TerrainQueue.Enqueue(terrainInfo);
      terrainInfo = CreateTerrainEntity(gameObject, terrainInfo);
      TerrainQueue.Enqueue(terrainInfo);

      LatestTerrainInfo = terrainInfo;
    }

    // Update is called once per frame
    void Update()
    {
      if (Player.transform.position.z > LatestTerrainInfo.EndZ)
      {

      }
    }

    private static TerrainInfo CreateTerrainEntity(GameObject parentObj, TerrainInfo previousTerrainInfo)
    {
      var currentIndex = previousTerrainInfo.Index + 1;

      // Create new 3d object for the terrain
      var terrainObj = new GameObject($"Terrain object #{currentIndex}");
      terrainObj.transform.SetParent(parentObj.transform);

      // Set up terrain component
      var terrain = terrainObj.AddComponent<Terrain>();
      terrain.name = $"Terrain component #{currentIndex}";
      terrainObj.AddComponent<TerrainCollider>();
      
      // Generate heightmap
      GenerateTerrain(terrainObj, terrain, previousTerrainInfo);

      return new TerrainInfo(currentIndex, previousTerrainInfo.EndZ + terrain.terrainData.heightmapResolution);
    }

    //Our Generate Terrain function
    private static void GenerateTerrain(GameObject hostObject, Terrain t, TerrainInfo previousTerrainInfo)
    {
      //The higher the numbers, the more hills/mountains there are
      float tileSize = 5;

      //The lower the numbers in the number range, the higher the hills/mountains will be...
      float divRange = 1;

      //Heights For Our Hills/Mountains
      var terrainSize = 128;
      var terrainWorldSize = terrainSize / 4;
      t.terrainData = new TerrainData
      {
        size = terrainWorldSize * new Vector3(1, 1, 1),
        heightmapResolution = terrainSize,
      };

      float[,] hts = new float[terrainSize, terrainSize];
      var texture = new Texture2D(terrainSize, terrainSize, TextureFormat.RGB24, mipChain: false)
      {
        alphaIsTransparency = true,
      };
      var roadSize = 10;
      for (int i = 0; i < terrainSize; i++)
      {
        for (int j = 0; j < terrainSize; j++)
        {
          float noise;
          if (j > terrainSize / 2 - (roadSize / 2) && j < terrainSize / 2 + (roadSize / 2))
          {
            noise = 0.5f;
          }
          else
          {
            noise = Mathf.PerlinNoise(((float)i / (float)terrainSize) * tileSize, ((float)j / (float)terrainSize) * tileSize) / divRange;
          }
          hts[i, j] = noise;
          var gray = noise;
          texture.SetPixel(j, i, new Color(gray, 0.0f, 0.0f));
        }
      }
      t.terrainData.SetHeights(0, 0, hts);

      // Create texture
      texture.Apply();
      var terrainMaterial = new Material(Shader.Find("Standard"))
      {
        mainTexture = texture,
        name = "Heightmap texture",
        mainTextureScale = new Vector2(1, 1),
      };
      t.materialTemplate = terrainMaterial;
      //SetTerrainSplatMap(t, texture);

      // Reposition Terrain
      hostObject.transform.position += new Vector3(-terrainWorldSize * 2, -terrainWorldSize / 2, previousTerrainInfo.EndZ);

      // Color trigger box
      var redMat = new Material(Shader.Find("Standard"));
      redMat.SetFloat("_Mode", 3); // Transparent mode
      redMat.SetColor("_Color", new Color(1, 1, 1, 0.2f));
      // Create trigger box
      var trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
      trigger.GetComponent<Renderer>().material = redMat;
      trigger.name = $"Trigger #{previousTerrainInfo.Index + 1}";
      trigger.transform.SetParent(hostObject.transform, worldPositionStays: false);
      trigger.transform.localScale = new Vector3(terrainWorldSize * 2, terrainWorldSize, 10);
      trigger.transform.position += new Vector3(terrainWorldSize * 2, terrainWorldSize, terrainSize - 5);
    }

    /*private void SetTerrainSplatMap(Terrain terrain, params Texture2D[] textures)
    {
      var terrainData = terrain.terrainData;

      // The Splat map (Textures)
      TerrainLayer[] splatPrototype = new TerrainLayer[terrainData.terrainLayers.Length];
      for (int i = 0; i < terrainData.terrainLayers.Length; i++)
      {
        splatPrototype[i] = new TerrainLayer
        {
          diffuseTexture = textures[i],    //Sets the texture
          tileSize = new Vector2(terrainData.terrainLayers[i].tileSize.x, terrainData.terrainLayers[i].tileSize.y),    //Sets the size of the texture
          tileOffset = new Vector2(terrainData.terrainLayers[i].tileOffset.x, terrainData.terrainLayers[i].tileOffset.y)    //Sets the size of the texture
        };
      }
      terrainData.terrainLayers = splatPrototype;
    }*/
  }

}