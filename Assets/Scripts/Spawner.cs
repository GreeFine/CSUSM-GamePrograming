using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{
  private static Dictionary<string, Creep> creeps_list = new Dictionary<string, Creep>();
  private List<Creep> creeps_created = new List<Creep>();
  private Nexus enemy_nexus = null;

  private void Awake()
  {
    Nexus[] nexus = GameObject.FindObjectsOfType<Nexus>();
    enemy_nexus = nexus[0];
  }

  // Update is called once per frame

  [Command]
  public void CmdNewCreep(uint playerId, string creepName)
  {
    // Creep creep_ = Instantiate (creepsList[creepName], transform.position, transform.rotation);
    Creep tmp = Resources.Load<Creep>("Prefabs/DefaultCreep");
    tmp.Init(playerId, transform.position, enemy_nexus.transform.position);
    Creep creep_ = Instantiate(tmp, transform.position, transform.rotation);
    NetworkServer.Spawn(creep_.gameObject);
    creeps_created.Add(creep_);
    Debug.Log("Creep added" + creeps_created.Count.ToString());
  }

  private void FixedUpdate()
  {
    //SPAWN THE UNITS
  }
}