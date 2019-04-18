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
  private const uint startingMana = 200;
  private const uint incomPersec = 10;
  public uint[] mana = new uint[2];
  private uint timer = 0;
  private uint numberOfPlayer = 0;

  private void AddNewUnit(string name, uint price)
  {
    unitMap.Add(name, Resources.Load<Unit>("Prefabs/Units/" + name));
    buildingMap.Add(name, Resources.Load<Building>("Prefabs/Buildings/Units/" + name + "_B"));
    priceMap.Add(name, price);
  }

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      AddNewUnit("Nature/Spider", 60);
      AddNewUnit("Orc/Orc_archer", 100);
      AddNewUnit("Orc/Orc_light_infantry", 150);
    }
    else
      Destroy(this);
  }

  [ClientRpc]
  public void RpcGameStarted()
  {
    instance.mana[0] = startingMana;
    instance.mana[1] = startingMana;
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
