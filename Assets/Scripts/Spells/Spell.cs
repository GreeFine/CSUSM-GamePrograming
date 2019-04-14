using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spell : MonoBehaviour
{
    public float cooldown;

    protected Hero owner;

    public virtual void Init(Hero hero, UnityEvent OnEnemies)
    {
        owner = hero;
    }
}
