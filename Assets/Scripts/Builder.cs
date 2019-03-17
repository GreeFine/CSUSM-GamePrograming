using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
  public Camera camera;
  public int pId = 1;//TODO : SETME
  public Building[] availableBuildings = new Building[1];

  private bool positioning = false;
  private Building ghost = null;
  private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
  private Vector3 lastValidPos = Vector3.zero;
  private int layer;

  private void Start()
  {
    camera = GameRule.instance.gameObject.GetComponent<Camera>();
    layer = 1 << (9 + pId);
  }

  public bool MouseRayCast(out Vector3 pos)
  {
    RaycastHit hit;
    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
    float rayDistance;

    if (groundPlane.Raycast(ray, out rayDistance)
       && Physics.Raycast(ray, out hit, rayDistance) && hit.collider.tag == "ConstructionArea")
    {
      pos = camera.GetComponent<GridDisplay>().GetNearestPointOnGrid(ray.GetPoint(rayDistance));
      lastValidPos = pos;
      return true;
    }
    pos = lastValidPos;
    return false;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.N))
    {
      if (ghost != null)
        Destroy(ghost);
      positioning = true;
      ghost = Instantiate(availableBuildings[0], this.transform);
    }

    if (positioning)
    {
      Vector3 pos;
      if (MouseRayCast(out pos))
      {
        Debug.Log(pos);
        ghost.transform.position = pos;
      }
    }

    if (Input.GetMouseButtonDown(0) && positioning)
    {
      positioning = false;
      ghost.Init();
      ghost = null;
    }

  }
}
