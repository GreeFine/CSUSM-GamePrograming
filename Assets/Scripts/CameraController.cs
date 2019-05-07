using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  private float maxX = 65.0f;
  private float maxZ = 25.0f;
  public float cameraSpeed = 50f;
  private bool focused = true;
  private Vector3 startPos;
  private Vector3 startPosP2;

  private void Start()
  {
    startPos = this.transform.position;
  }

  public void Init()
  {
    this.transform.position = GameRule.instance.playerBase[PlayerController.pId].transform.position;
    this.transform.position += new Vector3(0, 15, 0);
    if (PlayerController.pId == 1)
      this.transform.eulerAngles = new Vector3(60, 270, 0);

    startPos = this.transform.position;
    gameObject.transform.Find("Canvas").gameObject.SetActive(true);
  }

  private void Update()
  {
    var pos = this.transform.position;
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

      var newPos = this.transform.position;
      if (PlayerController.pId == 0)
        newPos += updatedPosition;
      else
        newPos -= updatedPosition;
      if (newPos.x < maxX && newPos.x > -maxX
      && newPos.z < maxZ && newPos.z > -maxZ)
        this.transform.position = newPos;
    }
  }
  void OnApplicationFocus(bool hasFocus)
  {
    focused = hasFocus;
  }
}
