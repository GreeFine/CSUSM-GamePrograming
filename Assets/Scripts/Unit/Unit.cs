﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Unit : AAttacker, IAttackable
{
    public bool hasAnimator;
    public Image healthBar;
    public int maxHealth;
    public float moveSpeed;
    public float animDmgTime;

    protected int pidOwner = -9;
    protected Vector3 enemyNexus;
    protected bool isDead = false;
    protected int currentHealth;
    protected float modifierMS = 1f;

    private void Start()
    {
        NavMeshAgent agent = this.gameObject.AddComponent<NavMeshAgent>();
        agent.SetDestination(enemyNexus);
        agent.speed = moveSpeed * modifierMS;
        ChangeAnimation("Run", GetComponent<Animation>()["Run"].length * modifierMS);
    }

    [ClientRpc]
    public void RpcInit(int pPidOwner, Vector3 pStartPos, Vector3 pEnemyNexus)
    {
        pidOwner = pPidOwner;
        this.gameObject.layer = 9 + pidOwner;
        this.transform.position = pStartPos;
        enemyNexus = pEnemyNexus;
        currentHealth = maxHealth;
        if (pPidOwner == 1)
        {
            healthBar.GetComponentInParent<Canvas>().GetComponent<RectTransform>().Rotate(new Vector3(1, 0, 0), -90);
            healthBar.fillOrigin = 0;
        }
        modifierMS = 1f;
        modifierAS = 1f;
    }

    private void LateUpdate()
    {
        if (isDead)
            Destroy(this.gameObject);
    }

    protected override void Cancel()
    {
        ChangeAnimation("Idle");
    }

    protected override void NoTargets()
    {
        ChangeAnimation("Run", GetComponent<Animation>()["Run"].length * modifierMS);
        GetComponent<NavMeshAgent>().destination = enemyNexus;
        GetComponent<NavMeshAgent>().isStopped = false;
    }

    protected override void TargetNotInRange()
    {
        ChangeAnimation("Run", GetComponent<Animation>()["Run"].length * modifierMS);
        GetComponent<NavMeshAgent>().destination = currentTarget.GetComponent<IAttackable>().GetPosition();
    }

    protected override void LaunchAttack()
    {
        GetComponent<NavMeshAgent>().isStopped = true;
        atkReload = atkSpeed * modifierAS;
        ChangeAnimation("Attack", atkSpeed * modifierAS);
        StartCoroutine(AtkWindUpComplete(animDmgTime * modifierAS, currentTarget));
    }

    //Animation
    private IEnumerator AtkWindUpComplete(float time, GameObject target)
    {
        yield return new WaitForSeconds(time);
        if (target != null)
            target.GetComponent<IAttackable>().ReceiveDamage(atkDmg);
        yield return null;
    }

    private void ChangeAnimation(string name)
    {
        if (hasAnimator)
            GetComponent<Animator>().SetBool(name, true);
        else
            GetComponent<Animation>().Play(name);
    }

    private void ChangeAnimation(string name, float animeSpeed = 1.0f)
    {
        if (hasAnimator)
            GetComponent<Animator>().SetBool(name, true);
        else
        {
            GetComponent<Animation>()[name].speed = GetComponent<Animation>()[name].length / animeSpeed;
            GetComponent<Animation>().Play(name, PlayMode.StopAll);
        }
    }

    //Interface : IAttackable
    public Vector3 GetPosition() { return this.transform.position; }
    public bool ReceiveDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true;
            return true;
        }
        healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
        return false;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}