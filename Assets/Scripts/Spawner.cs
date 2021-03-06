﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{
  public List<(int, string, Vector3)> placedUnits = new List<(int, string, Vector3)>();
  public Nexus enemyNexus = null;
  private float WaveTime = 10.0f;
  private List<Unit> unitsActive { get; } = new List<Unit>();
  private float Timer = 0.0f;

  public void SpawnUnits()
  {
    foreach (var unit in placedUnits)
    {
      Quaternion quaternion = new Quaternion(0, 0, 0, 0);
      Unit unitObj = GameRule.instance.unitMap[unit.Item2];

      Unit tmp = Instantiate(unitObj, unit.Item3, unitObj.transform.rotation);
      tmp.transform.Rotate(0f, unit.Item1 * 180f, 0f, Space.Self);
      NetworkServer.Spawn(tmp.gameObject);
      tmp.RpcInit(unit.Item1, enemyNexus.transform.position);
    }
  }
  private void Update()
  {
    if (!isServer) return;
    Timer -= Time.deltaTime;
    if (Timer < 0.0f)
    {
      SpawnUnits();
      Timer = WaveTime;
    }
  }
}