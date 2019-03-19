using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
  Vector3 GetPosition();

  void ReceiveDamage(int damage);
}