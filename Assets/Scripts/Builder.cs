﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Builder : NetworkBehaviour
{
  public static Builder instance = null;
  private bool positioning = false;
  private Building ghost = null;
  private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
  private Vector3 lastValidPos = Vector3.zero;
  private string currrentUnitName = "";

  private void Start()
  {
    if (isLocalPlayer)
      instance = this;
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

  public void CreateGhost(string name)
  {
    Camera.main.GetComponent<GridDisplay>().activ = true;
    if (ghost != null)
      Destroy(ghost.gameObject);
    positioning = true;

    currrentUnitName = name;
    Building building = GameRule.instance.buildingMap[currrentUnitName];
    ghost = Instantiate(building, this.transform.position, building.transform.rotation);
    ghost.transform.Rotate(0f, PlayerController.pId * 180f, 0f, Space.Self);
  }

  private void DestroyGhost()
  {
    Camera.main.GetComponent<GridDisplay>().activ = false;
    if (ghost == null)
      return;
    positioning = false;
    Destroy(ghost.gameObject);
    ghost = null;
  }

  private void Update()
  {
    if (!GameRule.instance.gameStarted)
      return;
    else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1))
      DestroyGhost();

    if (positioning)
    {
      Vector3 pos;
      if (MouseRayCast(out pos))
        ghost.transform.position = pos;

      if (Input.GetMouseButtonDown(0))
      {
        DestroyGhost();
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
      Building building = GameRule.instance.buildingMap[buildingName];

      Building tmp = Instantiate(building, pos, building.transform.rotation);
      tmp.transform.Rotate(0f, pId * 180f, 0f, Space.Self);

      pos.x += spawner.transform.localPosition.x - this.transform.localPosition.x;
      spawner.placedUnits.Add((pId, buildingName, pos));
      NetworkServer.Spawn(tmp.gameObject);
      tmp.Init();
    }
  }
}
