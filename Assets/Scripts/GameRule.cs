using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRule : MonoBehaviour
{
  public static GameRule instance = null;
  public GameObject[] playerBase = new GameObject[2];
  public Dictionary<string, Unit> unitMap = new Dictionary<string, Unit>();
  public Dictionary<string, Building> buildingMap = new Dictionary<string, Building>();
  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      unitMap.Add("default", Resources.Load<Unit>("Prefabs/DefaultUnit"));//FIXME: sub folder creeps?
      buildingMap.Add("default", Resources.Load<Building>("Prefabs/Buildings/Unit/B_DefaultUnit"));
    }
    else
      Destroy(this);
  }

}
