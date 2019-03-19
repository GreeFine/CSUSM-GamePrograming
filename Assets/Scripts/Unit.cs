using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IAttackable
{
  public bool hasAnimator = false;
  public int maxLife;
  public int atkDmg;
  public float atkRange;
  public float atkSpeed;

  private int currentLife;
  private float atkReload = 0f;
  private Vector3 enemyNexus;
  private List<IAttackable> targets = new List<IAttackable>();
  private IAttackable currentTarget = null;

  private int pidOwner = -1;

  private void Awake()
  {
    NavMeshAgent agent = this.gameObject.AddComponent<NavMeshAgent>();
    agent.SetDestination(enemyNexus);
    ChangeAnimation("Run");
  }

  public void Init(int pPidOwner, Vector3 pStartPos, Vector3 pEnemyNexus)
  {
    pidOwner = pPidOwner;
    this.gameObject.layer = 9 + pidOwner;
    this.transform.position = pStartPos;
    enemyNexus = pEnemyNexus;
    currentLife = maxLife;
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
      GetComponent<NavMeshAgent>().destination = enemyNexus;
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
    currentTarget.ReceiveDamage(atkDmg); //TODO: Should have a warm-up time
  }

  public void Update()
  {
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