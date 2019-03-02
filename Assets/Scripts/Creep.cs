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
  private Vector3 enemy_nexus;
  private List<IAttackable> targets = new List<IAttackable>();
  private IAttackable currentTarget = null;
  private bool isBuilding = true; //FIXME:
  private uint owner = checked(0);

  private void Awake()
  {
    NavMeshAgent agent = this.gameObject.AddComponent<NavMeshAgent>();
    agent.SetDestination(enemy_nexus);
    ChangeAnimation("Run");
  }

  public void Init(uint p_owner, Vector3 p_start_pos, Vector3 p_enemy_nexus)
  {
    isBuilding = false;
    owner = p_owner;
    this.gameObject.layer = 9 + (int)owner;
    this.transform.position = p_start_pos;
    enemy_nexus = p_enemy_nexus;
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
      GetComponent<NavMeshAgent>().destination = enemy_nexus;
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
    currentTarget.ReceiveDamage(atkDmg); //Should have a warm-up time
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