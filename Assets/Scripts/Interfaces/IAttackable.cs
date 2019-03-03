using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
  Vector3 GetPosition();

  GameObject GetGameObject();
  void ReceiveDamage(int damage);
}