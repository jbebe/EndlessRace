using System.Collections.Generic;
using UnityEngine;

namespace Assets.ErEntities.ErTerrain
{

  public class ErTerrainGenerator : MonoBehaviour
  {
    public static ErTerrainConfig Config = new()
    {
      // Basic dimensions
      Width = 33,
      Height = 33,
      TerrainBoxHeight = 8,

      // Terrain shape
      Resolution = 16,
      Frequency = 5,
      Amplitude = 4,

      // Texture
      Texture = new ErTerrainTextureConfig
      {
        Width = 128,
        Height = 128
      }
    };

    // [0]: behind
    // [1]: active
    // [2]: forward
    private List<ErTerrainContext> Terrains;

    // Start is called before the first frame update
    void Start()
    {
      Terrains = InitTerrainQueue();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
      var grey = new Color(1, 0, 0, 0.3f);
      var green = new Color(0, 1, 0, 0.3f);
      var yellow = new Color(1, 1, 0, 0.3f);

      GizmoHelper.DrawPlane(
        gameObject.transform.position + new Vector3(0, 0, Config.Height),
        Config.Width, Config.Height, Config.TerrainBoxHeight,
        yellow);
      GizmoHelper.DrawPlane(
        gameObject.transform.position,
        Config.Width, Config.Height, Config.TerrainBoxHeight,
        green);
      GizmoHelper.DrawPlane(
        gameObject.transform.position - new Vector3(0, 0, Config.Height),
        Config.Width, Config.Height, Config.TerrainBoxHeight,
        grey);
    }

    private List<ErTerrainContext> InitTerrainQueue()
    {
      var terrains = new List<ErTerrainContext>();
      for (var i = 0; i < 3; ++i)
      {
        var terrainObj = AppendTerrainObject(i == 0 ? null : terrains[i - 1]);
        terrains.Add(terrainObj);
      }

      return terrains;
    }

    private ErTerrainContext AppendTerrainObject(ErTerrainContext previousContext)
    {
      var context = new ErTerrainContext { Parent = gameObject };
      if (previousContext == null)
      {
        context.Index = 1;
        context.Previous = null;
        context.PreviousHeightMap = null;
      }
      else
      {
        context.Index = previousContext.Index + 1;
        context.Previous = previousContext.Current;
        context.PreviousHeightMap = previousContext.CurrentHeightMap;
      }

      return ErTerrainHelper.CreateTerrainObject(context);
    }
  }

}