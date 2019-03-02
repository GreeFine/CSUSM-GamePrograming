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
    {
      pid = 0;
      spawner = GameRule.instance.spawners[pid];
      spawner.transform.parent = this.transform;
    }
    else
    {
      pid = 1;
      spawner = GameRule.instance.spawners[pid];
      spawner.transform.parent = this.transform;
    }
    Debug.Log("Player spawned:" + pid); //TODO: fix me
  }


  private void CreateCreep()
  {
    spawner.CmdNewCreep(pid, "default");
  }

  private void Update()
  {
    if (isLocalPlayer)
    {
      if (Input.GetKeyDown(KeyCode.L))
        CreateCreep();
      if (Input.GetKeyDown(KeyCode.Escape))
        Application.Quit();
    }
  }
}