using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameRule : NetworkBehaviour
{
  public static GameRule instance = null;
  public GameObject[] playerBase = new GameObject[2];
  public Dictionary<string, Unit> unitMap = new Dictionary<string, Unit>();
  public Dictionary<string, Building> buildingMap = new Dictionary<string, Building>();
  public Dictionary<string, uint> priceMap = new Dictionary<string, uint>();

  public bool gameStarted = false;
  private const uint startingMana = 100;
  public uint incomPersec = 5;
  public uint[] mana = new uint[2] { startingMana, startingMana };
  private uint timer = 0;
  private uint numberOfPlayer = 0;
  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      unitMap.Add("default", Resources.Load<Unit>("Prefabs/DefaultUnit"));//FIXME: sub folder creeps?
      buildingMap.Add("default", Resources.Load<Building>("Prefabs/Buildings/Unit/B_DefaultUnit"));
      priceMap.Add("default", 30);
    }
    else
      Destroy(this);
  }

  [ClientRpc]
  public void RpcGameStarted()
  {
    gameStarted = true;
    Camera.main.GetComponent<GridDisplay>().Init();
    Camera.main.GetComponent<CameraController>().Init();
  }

  private void FixedUpdate()
  {
    if (gameStarted)
    {
      timer++;
      if (timer % 50 == 0)
      {
        mana[0] += incomPersec;
        mana[1] += incomPersec;
      }
    }
  }

}
