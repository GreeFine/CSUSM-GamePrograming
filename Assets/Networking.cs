using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Networking : MonoBehaviour
{
  private void Start()
  {
    if (MainMenu.isJoin)
      NetworkManager.singleton.StartClient();
    else
      NetworkManager.singleton.StartHost();
  }
}
