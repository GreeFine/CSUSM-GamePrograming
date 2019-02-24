using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creep : MonoBehaviour, IAttackable
{
    public bool hasAnimator = false;
    public int maxLife;
    public int atkDmg;
    public float atkRange;
    public float atkSpeed;

    private int currentLife;
    private float atkReload = 0f;
    private Vector3 nexus;
    private List<IAttackable> targets;
    private IAttackable currentTarget = null;
    private bool isBuilding = true;

    public void Init(Vector3 newPos, Vector3 dest, int playerId)
    {
        isBuilding = false;
        this.transform.position = newPos;
        this.gameObject.layer =  8 + playerId;

        NavMeshAgent agent;
        agent = this.gameObject.AddComponent<NavMeshAgent>();
        agent.SetDestination(dest);
        nexus = dest;

        currentLife = maxLife;
        targets = new List<IAttackable>();
        ChangeAnimation("Run");
    }

    public void SetDestination(Vector3 dest)
    {
        GetComponent<NavMeshAgent>().SetDestination(dest);
    }

    private void ChooseTarget()
    {
        int count = targets.Count;
        int i = 0;
        while (i < count && targets[i] == null)
        {
            targets.RemoveAt(i);
            i++;
        }
        targets.Sort(SortByDistance);
        currentTarget = targets[0];
    }

    private void Attack()
    {
        if (currentTarget == null)
        {
            ChangeAnimation("Run");
            GetComponent<NavMeshAgent>().destination = nexus;
            GetComponent<NavMeshAgent>().isStopped = false;
            return;
        }
        if (Vector3.Distance(this.transform.position, currentTarget.GetPosition()) > atkRange)
        {
            GetComponent<NavMeshAgent>().destination = currentTarget.GetPosition();
            return;
        }
        GetComponent<NavMeshAgent>().isStopped = true;
        ChangeAnimation("Attack");
        atkReload = atkSpeed;
        currentTarget.ReceiveDamage(atkDmg);//Should have a warm-up time
    }

    public void Update()
    {
        if (isBuilding)
            return;
        atkReload -= Time.deltaTime;
        if (atkReload <= 0)
        {
            if (currentTarget == null && targets.Count > 0)
                ChooseTarget();
            Attack();
        }
    }

    private void ChangeAnimation(string name)
    {
        if (hasAnimator)
            GetComponent<Animator>().SetBool(name, true);
        else
            GetComponent<Animation>().Play(name);
    }

    private void OnTriggerEnter(Collider other)
    {
        IAttackable tmp = other.GetComponent<IAttackable>();
        if (tmp != null)
            targets.Add(tmp);
    }

    private void OnTriggerExit(Collider other)
    {
        IAttackable tmp = other.GetComponent<IAttackable>();
        if (tmp != null)
            targets.Remove(tmp);
    }

    private int SortByDistance(IAttackable a, IAttackable b)
    {
        return ((this.transform.position - a.GetPosition()).sqrMagnitude.CompareTo((this.transform.position - b.GetPosition()).sqrMagnitude));
    }


    //Interface : IAttackable
    public Vector3 GetPosition() { return this.transform.position; }
    public void ReceiveDamage(int damage)
    {
        currentLife -= damage;
        if (currentLife <= 0)
            Destroy(this.gameObject);
    }
}