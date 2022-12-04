using Assets.ErCommon;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ErEntities.ErTerrain
{

  public class ErTerrainGenerator : MonoBehaviour
  {
    [SerializeField]
    public GameObject Player;

    public static ErTerrainConfig Config = new()
    {
      // Basic dimensions
      Size = 128 + 1,
      Height = 16,

      // Terrain shape
      Frequency = 5,
      Amplitude = 4,

      // Misc
      TriggerCheckSec = 1,

      // Texture
      Texture = new ErTerrainTextureConfig
      {
        Width = 128,
        Height = 128
      },

      // How many vertices should be interpolated with next terrain
      StitchSize = 20,

      RoadSize = 10,
    };

    // [0]: previous
    // [1]: active
    // [2]: next
    public List<ErTerrainContext> Terrains;

    private ErTerrainContext NextTerrain => Terrains[2];

    private float Timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
      Terrains = InitTerrainQueue();

    }

    // Update is called once per frame
    void Update()
    {
      UpdateTerrainGeneration();
    }

    void UpdateTerrainGeneration()
    {
      Timer += Time.deltaTime;
      if (Timer > Config.TriggerCheckSec)
      {
        var playerReachedNext = NextTerrain.Bounds.Contains(Player.transform.position);
        if (playerReachedNext) GenerateNewTerrain();
        Timer = 0.0f;
      }
    }

    void OnDrawGizmos()
    {
      var grey = new Color(1, 0, 0, 0.3f);
      var green = new Color(0, 1, 0, 0.3f);
      var yellow = new Color(1, 1, 0, 0.3f);

      /*
      // DEBUG terrain bounds
      var i = 0.0f;
      foreach (var ctx in Terrains)
      {
        Gizmos.color = new Color(i, 1 - i, 0, 0.3f);
        i += 0.3f;
        Gizmos.DrawCube(ctx.Bounds.center, ctx.Bounds.size);
      }
      */

      GizmoHelper.DrawPlane(
        gameObject.transform.position + new Vector3(0, 0, Config.Size),
        Config.Size, Config.Size, Config.Height,
        yellow);
      GizmoHelper.DrawPlane(
        gameObject.transform.position,
        Config.Size, Config.Size, Config.Height,
        green);
      GizmoHelper.DrawPlane(
        gameObject.transform.position - new Vector3(0, 0, Config.Size),
        Config.Size, Config.Size, Config.Height,
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

    private void GenerateNewTerrain()
    {
      var deletableObject = Terrains[0];
      Terrains[0] = Terrains[1];
      Terrains[1] = Terrains[2];
      Terrains[2] = AppendTerrainObject(Terrains[1]);
      Destroy(deletableObject.Current);
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

      context.Bounds = new Bounds(
        new Vector3(0, Config.Height / 2, (context.Index - 2) * (Config.Size)),
        new Vector3(Config.Size, Config.Height, Config.Size));

      return ErTerrainHelper.CreateTerrainObject(context);
    }
  }

}