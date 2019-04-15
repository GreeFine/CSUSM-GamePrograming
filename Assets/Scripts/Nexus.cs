using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nexus : Tower
{
  public int pPidOwner;

  protected override void Start()
  {
    MaxHealth = 30;
    health = MaxHealth;
  }

  protected override void onDestroyed()
  {
    if (pPidOwner == PlayerController.pId)
      GameObject.Find("endGameText").GetComponent<Text>().text = "Game Over !";
    else
      GameObject.Find("endGameText").GetComponent<Text>().text = "You Win !";
    Time.timeScale = 0f;
    Destroy(this.gameObject);
  }

}
