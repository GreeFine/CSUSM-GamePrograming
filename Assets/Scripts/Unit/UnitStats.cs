using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UnitStats
{
    public int maxHealth;
    public int atkDmg;
    public float atkRange;
    public float atkSpeed;
    public float animDmgTime;

    [HideInInspector]
    public int currentHealth;
    [HideInInspector]
    public float atkReload;
    [HideInInspector]
    public float modifierAS; //TODO : implem AS buff/debuff setter
    [HideInInspector]
    public float modifierMS; //TODO : implem unit speed + animation
}
