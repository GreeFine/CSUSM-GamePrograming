using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRule : MonoBehaviour
{
  public static GameRule instance = null;
  public Spawner[] spawners = new Spawner[2];
  public Dictionary<string, Unit> unitList = new Dictionary<string, Unit>();

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      unitList.Add("default", Resources.Load<Unit>("Prefabs/DefaultUnit"));
    }
    else
      Destroy(this);
  }

}
