using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
  private int pid = -1;
  private Spawner spawner = null;


  public override void OnStartLocalPlayer()
  {
    if (isServer)
      pid = 0;
    else
      pid = 1;
    Debug.Log("Player spawned:" + pid);
    spawner = GameRule.instance.spawners[pid];
    CmdInit(pid);
    // spawner.transform.parent = this.transform;
    // CmdReParent();
  }

  [Command]
  public void CmdInit(int pPid)
  {
    pid = pPid;
    Debug.Log(pid);
    spawner = GameRule.instance.spawners[pid];
    Debug.Log(spawner);
  }

  [Command]
  public void CmdSpawn()
  {
    spawner.SpawnUnit(pid, "default");
  }

  private void Update()
  {
    if (isLocalPlayer)
    {
      if (Input.GetKeyDown(KeyCode.L))
        CmdSpawn();
      if (Input.GetKeyDown(KeyCode.Escape))
        Application.Quit();
    }
  }
}