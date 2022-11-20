using UnityEngine;

public static class GizmoHelper
{
  // Start is called before the first frame update
  public static void DrawPlane(Vector3 position, float width, float height, float ceiling, Color color)
  {
    Gizmos.color = color;
    var planeSize = new Vector3(width, 0, height);
    Gizmos.DrawCube(position, planeSize);
    Gizmos.DrawWireCube(position + new Vector3(0, ceiling / 2, 0), planeSize + new Vector3(0, ceiling, 0));
  }
}
