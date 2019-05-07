using UnityEngine;
using UnityEngine.UI;

public class UnitIcone : MonoBehaviour
{
  public Sprite icone;
  public string manaCost;
  public string unit;

  private void Start()
  {
    GetComponent<Image>().sprite = icone;
    GetComponentInChildren<Text>().text = manaCost;
  }

  public void OnIconeClick()
  {
    Builder.instance.CreateGhost(unit);
  }
}
