using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Builder : NetworkBehaviour
{

  private bool positioning = false;
  private Building ghost = null;
  private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
  private Vector3 lastValidPos = Vector3.zero;
  private Camera camera;
  private Spawner spawner = null;
  private int pId = -7;

  private void Start()
  {
    if (isServer)
      pId = 1;//FIXME: temporary
    else
      pId = 0;
    camera = GameRule.instance.gameObject.GetComponent<Camera>();
    spawner = GameRule.instance.spawners[pId];
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
      ghost = Instantiate(GameRule.instance.buildingMap["default"], this.transform);
    }

    if (positioning)
    {
      Vector3 pos;
      if (MouseRayCast(out pos))
        ghost.transform.position = pos;

      if (Input.GetMouseButtonDown(0))
      {
        positioning = false;
        Destroy(ghost);
        ghost = null;
        CmdPlaceNewBuilding(pId, "default", pos);
      }
    }
  }

  [Command]
  private void CmdPlaceNewBuilding(int pId, string buildingName, Vector3 pos)
  {
    Quaternion quaternion = new Quaternion(0, 0, 0, 0); //FIXME: depend on player side
    Building tmp = Instantiate(GameRule.instance.buildingMap[buildingName], pos, quaternion);
    pos.x += Mathf.Abs(spawner.transform.position.x - this.GetComponentInParent<Transform>().position.x);
    spawner.placedUnits.Add((pId, buildingName, pos));
    NetworkServer.Spawn(tmp.gameObject);
    tmp.Init();
  }
}
