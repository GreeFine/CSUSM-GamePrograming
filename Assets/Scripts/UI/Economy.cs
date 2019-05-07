using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Economy : MonoBehaviour
{
  private void FixedUpdate()
  {
    if (GameRule.instance.gameStarted)
      GetComponent<Text>().text = GameRule.instance.mana[PlayerController.pId] + "";
  }
}
