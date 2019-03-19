using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : MonoBehaviour, IAttackable
{
    public Vector3 GetPosition() { return this.transform.position; }

    public void ReceiveDamage(int dmg)
    {
        //TODO
        Debug.Log("Receive " + dmg.ToString() + " damage");
    }
}
