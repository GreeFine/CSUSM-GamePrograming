using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{
  private List<Unit> unitsCreated = new List<Unit>();
  private List<Unit> unitsActive { get; } = new List<Unit>();
  public Nexus enemyNexus = null;


  [Command]
  public void CmdNewCreep(int playerId, string creepName)
  {
    Debug.Log(GameRule.instance.unitList[creepName]);
    Unit tmp = Instantiate(GameRule.instance.unitList[creepName], transform.position, transform.rotation);
    tmp.Init(playerId, transform.position, enemyNexus.transform.position);
    NetworkServer.Spawn(tmp.gameObject);
    unitsActive.Add(tmp);
    Debug.Log("Creep added" + unitsCreated.Count.ToString());
  }

  private void FixedUpdate()
  {
    //SPAWN THE UNITS
  }
}