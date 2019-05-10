using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameRule : NetworkBehaviour
{
  public static GameRule instance = null;
  public NetworkManager netmanager = null;
  public GameObject[] playerBase = new GameObject[2];
  public List<string> unitNames = new List<string>();

  public Dictionary<string, Unit> unitMap = new Dictionary<string, Unit>();
  public Dictionary<string, Building> buildingMap = new Dictionary<string, Building>();
  public Dictionary<string, uint> priceMap = new Dictionary<string, uint>();

  public bool gameStarted = false;
  private const uint startingMana = 20;
  private const uint incomPersec = 6;
  public uint[] mana = new uint[2];
  private uint timer = 0;
  private uint numberOfPlayer = 0;

  private void AddNewUnit(string name, uint price)
  {
    unitNames.Add(name);
    unitMap.Add(name, Resources.Load<Unit>("Prefabs/Units/" + name));
    buildingMap.Add(name, Resources.Load<Building>("Prefabs/Buildings/Units/" + name + "_B"));
    priceMap.Add(name, price);
  }
  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      AddNewUnit("Orc/Orc_worker", 10);
      AddNewUnit("Orc/Orc_light_infantry", 30);
      AddNewUnit("Orc/Orc_light_cavalry", 50);
      AddNewUnit("Orc/Orc_spearman", 50);
      AddNewUnit("Orc/Orc_archer", 70);
      AddNewUnit("Orc/Orc_shaman", 100);
      AddNewUnit("Orc/Orc_heavy_infantry", 100);
      AddNewUnit("Orc/Orc_heavy_cavalry", 150);
      AddNewUnit("Orc/Orc_mounted_shaman", 200);

      AddNewUnit("Undead/UD_worker", 10);
      AddNewUnit("Undead/UD_light_infantry", 30);
      AddNewUnit("Undead/UD_light_cavalry", 50);
      AddNewUnit("Undead/UD_spearman", 50);
      AddNewUnit("Undead/UD_archer", 70);
      AddNewUnit("Undead/UD_mage", 100);
      AddNewUnit("Undead/UD_heavy_infantry", 100);
      AddNewUnit("Undead/UD_heavy_cavalry", 150);
      AddNewUnit("Undead/UD_mage_mounted", 200);
    }
    else
      Destroy(this);
  }

  [ClientRpc]
  public void RpcGameStarted()
  {
    print("???GS");
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
