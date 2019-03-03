using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;



public class Unit : NetworkBehaviour, IAttackable
{
  public bool hasAnimator = false;
  public int maxLife;
  public int atkDmg;
  public float atkRange;
  public float atkSpeed;

  private int currentLife;
  private float atkReload = 0f;
  private Vector3 enemyNexus;
  private List<GameObject> targets = new List<GameObject>();
  private GameObject currentTarget = null;
  private bool isDead = false;
  private int pidOwner = -9;


  private void Start()
  {
    NavMeshAgent agent = this.gameObject.AddComponent<NavMeshAgent>();
    agent.SetDestination(enemyNexus);
    ChangeAnimation("Run");
  }

  [ClientRpc]
  public void RpcInit(int pPidOwner, Vector3 pStartPos, Vector3 pEnemyNexus)
  {
    Debug.Log(pPidOwner);
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
    if (targets.Count <= 0)
    {
      currentTarget = null;
      return;
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
    if (Vector3.Distance(this.transform.position, currentTarget.GetComponent<IAttackable>().GetPosition()) > atkRange)
    {
      GetComponent<NavMeshAgent>().destination = currentTarget.GetComponent<IAttackable>().GetPosition();
      return;
    }
    GetComponent<NavMeshAgent>().isStopped = true;
    ChangeAnimation("Attack");
    atkReload = atkSpeed;
    currentTarget.GetComponent<IAttackable>().ReceiveDamage(atkDmg); //TODO: Should have a warm-up time
  }

  private void Update()
  {
    Debug.Log(pidOwner);

    atkReload -= Time.deltaTime;
    if (atkReload <= 0)
    {
      if (currentTarget == null && targets.Count > 0)
        ChooseTarget();
      Attack();
    }
  }

  private void LateUpdate()
  {
    if (isDead)
      Destroy(this.gameObject);
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
      targets.Add(other.gameObject);
  }

  private void OnTriggerExit(Collider other)
  {
    IAttackable tmp = other.GetComponent<IAttackable>();
    if (tmp != null)
      targets.Remove(other.gameObject);
  }

  private int SortByDistance(GameObject a, GameObject b)
  {
    return ((this.transform.position - a.transform.position).sqrMagnitude.CompareTo((this.transform.position - b.transform.position).sqrMagnitude));
  }

  //Interface : IAttackable
  public Vector3 GetPosition() { return this.transform.position; }
  public void ReceiveDamage(int damage)
  {
    currentLife -= damage;
    if (currentLife <= 0)
      isDead = true;
  }

  public GameObject GetGameObject()
  {
    return this.gameObject;
  }
}