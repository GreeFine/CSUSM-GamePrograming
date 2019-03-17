using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 50f;
  private bool focused = true;

    void Update()
    {
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

        this.transform.position += updatedPosition;
    }
}
  void OnApplicationFocus(bool hasFocus)
  {
    focused = hasFocus;
  }
}
