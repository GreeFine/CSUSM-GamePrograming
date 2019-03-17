using System.Collections;
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
    if (!isServer) return;
    foreach (var unit in placedUnits)
    {
      Debug.Log("Units: " + unit.Item2 + ": " + unit.Item3);
      Quaternion quaternion = new Quaternion(0, 0, 0, 0);//FIXME: depend on pid
      Unit tmp = Instantiate(GameRule.instance.unitMap[unit.Item2], unit.Item3, quaternion);
      NetworkServer.Spawn(tmp.gameObject);
      tmp.RpcInit(unit.Item1, enemyNexus.transform.position);
      // unitsActive.Add(tmp);
    }
  }
  private void Update()
  {
    Timer -= Time.deltaTime;
    if (Timer < 0.0f)
    {
      SpawnUnits();
      Timer = WaveTime;
    }
  }
}