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
  private string currrentUnitName = "";

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

  private void CreateGhost(string name)
  {
    currrentUnitName = name;
    if (ghost != null)
      Destroy(ghost.gameObject);
    positioning = true;
    Quaternion quaternion = new Quaternion(0, PlayerController.pId * 180, 0, 0);
    ghost = Instantiate(GameRule.instance.buildingMap[currrentUnitName], this.transform.position, quaternion);
  }

  private void Update()
  {
    if (!GameRule.instance.gameStarted)
      return;
    if (Input.GetKeyDown(KeyCode.Alpha1))
      CreateGhost(GameRule.instance.unitNames[0]);
    if (Input.GetKeyDown(KeyCode.Alpha2))
      CreateGhost(GameRule.instance.unitNames[1]);
    if (Input.GetKeyDown(KeyCode.Alpha3))
      CreateGhost(GameRule.instance.unitNames[2]);
    if (Input.GetKeyDown(KeyCode.Alpha4))
      CreateGhost(GameRule.instance.unitNames[3]);
    if (Input.GetKeyDown(KeyCode.Alpha5))
      CreateGhost(GameRule.instance.unitNames[4]);
    if (Input.GetKeyDown(KeyCode.Alpha6))
      CreateGhost(GameRule.instance.unitNames[5]);
    if (Input.GetKeyDown(KeyCode.Alpha7))
      CreateGhost(GameRule.instance.unitNames[6]);
    if (Input.GetKeyDown(KeyCode.Alpha8))
      CreateGhost(GameRule.instance.unitNames[7]);
    if (Input.GetKeyDown(KeyCode.Alpha9))
      CreateGhost(GameRule.instance.unitNames[8]);

    if (positioning)
    {
      Vector3 pos;
      if (MouseRayCast(out pos))
        ghost.transform.position = pos;

      if (Input.GetMouseButtonDown(0))
      {
        positioning = false;
        Destroy(ghost.gameObject);
        ghost = null;
        if (isServer)
          CmdPlaceNewBuilding(PlayerController.pId, currrentUnitName, pos);
        else if (buyBuilding(PlayerController.pId, currrentUnitName))
          CmdPlaceNewBuilding(PlayerController.pId, currrentUnitName, pos);
      }
    }
  }

  private bool buyBuilding(int pId, string buildingName)
  {
    uint cost = GameRule.instance.priceMap[buildingName];
    if (GameRule.instance.mana[pId] < cost)
      return false;

    GameRule.instance.mana[pId] -= cost;
    return true;
  }

  [Command]
  private void CmdPlaceNewBuilding(int pId, string buildingName, Vector3 pos)
  {
    if (buyBuilding(pId, buildingName))
    {
      Spawner spawner = GameRule.instance.playerBase[pId].GetComponentInChildren<Spawner>();
      Quaternion quaternion = new Quaternion(0, pId * 180, 0, 0);
      Building tmp = Instantiate(GameRule.instance.buildingMap[buildingName], pos, quaternion);
      pos.x += spawner.transform.localPosition.x - this.transform.localPosition.x;
      spawner.placedUnits.Add((pId, buildingName, pos));
      NetworkServer.Spawn(tmp.gameObject);
      tmp.Init();
    }
  }
}
