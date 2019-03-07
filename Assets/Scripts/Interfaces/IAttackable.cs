using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    Vector3 GetPosition();

    GameObject GetGameObject();
    bool ReceiveDamage(int damage);
}