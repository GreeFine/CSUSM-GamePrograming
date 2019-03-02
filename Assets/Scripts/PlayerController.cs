using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
  uint id = 0;
  private Spawner spawner = null;

  private void Awake()
  {
    id = GetComponent<NetworkIdentity>().netId.Value;
    Spawner[] spawners = GameObject.FindObjectsOfType<Spawner>();
    spawner = spawners[id];
    Debug.Log("Player spawned:" + id);
  }

  private void AddCreep()
  {
    spawner.CmdNewCreep(id, "test");
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.L))
      AddCreep();
    if (Input.GetKeyDown(KeyCode.Escape))
      Application.Quit();
  }
}