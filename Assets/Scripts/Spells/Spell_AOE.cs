using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spell_AOE : Spell
{
    public int dmg;
    public int radius;

    public override void Init(Hero hero, UnityEvent OnEnemies)
    {
        owner = hero;
        OnEnemies.AddListener(OnSpellLaunch);
    }

    private void OnSpellLaunch()
    {
        if (owner.GetTargets().Count > 3)
        {

        }
    }
}
