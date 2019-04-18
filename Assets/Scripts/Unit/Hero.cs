using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hero : Unit
{
    public Spell[] spells;
    
    private float[] spellsCD;
    private UnityEvent OnEnemiesNearby;

    void Start()
    {
        spellsCD = new float[spells.Length];
        foreach (Spell spell in spells)
            spell.Init(this, OnEnemiesNearby);
    }

    void LateUpdate()
    {
        for(int i = 0; i < spellsCD.Length; i++)
            spellsCD[i] -= Time.deltaTime;
    }
    
    public List<GameObject> GetTargets() { return targets; }

    protected override void AddTarget(GameObject target)
    {
        base.AddTarget(target);
        OnEnemiesNearby.Invoke();
    }
}
