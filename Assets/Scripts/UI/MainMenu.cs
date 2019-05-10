using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  public static bool isJoin = false;
  public void Join()
  {
    isJoin = true;
    SceneManager.LoadScene("DefaultScene");
  }

  public void Host()
  {
    SceneManager.LoadScene("DefaultScene");
  }

  public void Quit()
  {
    Application.Quit();
  }
}
