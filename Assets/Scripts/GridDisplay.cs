using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDisplay : MonoBehaviour
{
  public float gridSizeX = 10f;
  public float gridSizeZ = 10f;
  public float largeStep = 1f;
  public float smallStep = 0.2f;
  private Color mainColor = new Color(0.26f, 0.44f, 0.6f, 1f);
  public Color subColor = new Color(1f, 1f, 1f, 1f);
  private Color lineColor = new Color(1f, 0f, 0f, 1f);
  public bool subActiv = false;
  public bool activ = false;

  private Material lineMaterial;
  private float sizeX;
  private float startX = 2;
  private float startY = 0.05f;
  private float startZ = 2;
  private float sizeZ;

  private void Start()
  {
    // Create material
    var shader = Shader.Find("Hidden/Internal-Colored");
    lineMaterial = new Material(shader);
    lineMaterial.hideFlags = HideFlags.HideAndDontSave;
    // Turn on alpha blending
    lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
    lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    // Turn backface culling off
    lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
    // Turn off depth writes
    lineMaterial.SetInt("_ZWrite", 0);

    sizeX = gridSizeX * 2;
    sizeZ = gridSizeZ * 2;
  }

  public void Init()
  {
    Transform spawnArea = GameRule.instance.playerBase[PlayerController.pId].transform.Find("BuildingArea");
    startX = spawnArea.position.x + (sizeX / -2f);
    startZ = spawnArea.position.y + (sizeZ / -2f);
  }

  private void OnPostRender()
  {
    if (!activ || startX == -1)
      return;

    lineMaterial.SetPass(0);
    GL.Begin(GL.LINES);
    GL.Color(mainColor);

    for (float i = 0; i <= sizeZ; i += largeStep)
    {
      GL.Vertex3(startX, startY, startZ + i);
      GL.Vertex3(startX + sizeX, startY, startZ + i);
    }
    for (float i = 0; i <= sizeX; i += largeStep)
    {
      GL.Vertex3(startX + i, startY, startZ);
      GL.Vertex3(startX + i, startY, startZ + sizeZ);
    }

    if (subActiv)
    {
      GL.Color(subColor);
      float subSize = (largeStep / smallStep) - 1f;

      for (float i = 0; i <= sizeZ; i += largeStep)
        for (float step = 1; step <= subSize; step += 1)
        {
          GL.Vertex3(startX, startY, startZ + i + (step * smallStep));
          GL.Vertex3(startX + sizeX, startY, startZ + i + (step * smallStep));
        }
      for (float i = 0; i <= sizeX; i += largeStep)
        for (float step = 1; step <= subSize; step += 1)
        {
          GL.Vertex3(startX + i + (step * smallStep), startY, startZ);
          GL.Vertex3(startX + i + (step * smallStep), startY, startZ + sizeZ);
        }
    }

    GL.End();
  }

  public Vector3 GetNearestPointOnGrid(Vector3 position)
  {
    int xCount = Mathf.RoundToInt(position.x / largeStep);
    int zCount = Mathf.RoundToInt(position.z / largeStep);

    Vector3 result = new Vector3(
        (float)xCount * largeStep,
        0,
        (float)zCount * largeStep);

    return result;
  }

  public void DrawLimited(Vector3 start, Vector3 end)
  {
    lineMaterial.SetPass(0);
    GL.Begin(GL.LINES);
    GL.Color(lineColor);

    GL.Vertex3(start.x, 0.1f, start.z);
    GL.Vertex3(end.x, 0.1f, end.z);

    GL.End();
  }
}
