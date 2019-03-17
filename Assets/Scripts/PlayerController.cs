using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
  private void Update()
  {
    if (isLocalPlayer)
    {
      if (Input.GetKeyDown(KeyCode.Escape))
        Application.Quit();
    }
  }
}