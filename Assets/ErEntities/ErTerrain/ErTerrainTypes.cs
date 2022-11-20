using UnityEngine;

namespace Assets.ErEntities.ErTerrain
{
  public class ErTerrainConfig
  {
    public int Width;

    public int Height;

    public int TerrainBoxHeight;

    public int Resolution;

    //The higher the numbers, the more hills/mountains there are
    public float Frequency;

    //The lower the numbers in the number range, the higher the hills/mountains will be...
    public float Amplitude;

    public ErTerrainTextureConfig Texture;
  }

  public class ErTerrainTextureConfig
  {
    public int Width;

    public int Height;
  }

  class ErTerrainContext
  {
    public int Index { get; set; }

    public GameObject Parent { get; set; }

    public GameObject Previous { get; set; }

    public GameObject Current { get; set; }

    public float[,] CurrentHeightMap { get; set; }

    public float[,] PreviousHeightMap { get; set; }
  }
}
