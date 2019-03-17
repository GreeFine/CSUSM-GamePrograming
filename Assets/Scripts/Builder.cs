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

  private void Start()
  {
    if (!isLocalPlayer)
      this.enabled = false;
  }

  public bool MouseRayCast(out Vector3 pos)
  {
    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    float rayDistance;

    if (groundPlane.Raycast(ray, out rayDistance)
       && Physics.Raycast(ray, out hit, rayDistance) && hit.collider.tag == "ConstructionArea")
    {
      pos = Camera.main.GetComponent<GridDisplay>().GetNearestPointOnGrid(ray.GetPoint(rayDistance));
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
      Quaternion quaternion = new Quaternion(0, PlayerController.pId * 180, 0, 0);
      ghost = Instantiate(GameRule.instance.buildingMap["default"], this.transform.position, quaternion);
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
        CmdPlaceNewBuilding(PlayerController.pId, "default", pos);
      }
    }
  }

  [Command]
  private void CmdPlaceNewBuilding(int pId, string buildingName, Vector3 pos)
  {
    Spawner spawner = GameRule.instance.playerBase[pId].GetComponentInChildren<Spawner>();
    Quaternion quaternion = new Quaternion(0, pId * 180, 0, 0); //FIXME: depend on player side
    Building tmp = Instantiate(GameRule.instance.buildingMap[buildingName], pos, quaternion);
    pos.x += spawner.transform.localPosition.x - this.transform.localPosition.x;
    spawner.placedUnits.Add((pId, buildingName, pos));
    NetworkServer.Spawn(tmp.gameObject);
    tmp.Init();
  }
}
