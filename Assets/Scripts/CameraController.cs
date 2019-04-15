using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public float cameraSpeed = 50f;
  private bool focused = true;
  private Vector3 startPos;

  private Vector3 startPosP2;

  public void Init()
  {
    if (PlayerController.pId == 1)
    {
      this.transform.position += new Vector3(128, 0, 0);
      this.transform.eulerAngles = new Vector3(60, 270, 0);
    }
    startPos = this.transform.position;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
      this.transform.position = startPos;
    if (focused)
    {
      Vector3 updatedPosition = Vector3.zero;

      if (Input.mousePosition.y < 10f)
        updatedPosition -= Vector3.right * cameraSpeed * Time.deltaTime;
      else if (Input.mousePosition.y > Screen.height - 10f)
        updatedPosition += Vector3.right * cameraSpeed * Time.deltaTime;
      if (Input.mousePosition.x < 10f)
        updatedPosition += Vector3.forward * cameraSpeed * Time.deltaTime;
      else if (Input.mousePosition.x > Screen.width - 10f)
        updatedPosition -= Vector3.forward * cameraSpeed * Time.deltaTime;

      if (PlayerController.pId == 0)
        this.transform.position += updatedPosition;
      else
        this.transform.position -= updatedPosition;
    }
  }
  void OnApplicationFocus(bool hasFocus)
  {
    focused = hasFocus;
  }
}
