using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(SphereCollider))]
public abstract class AAttacker : NetworkBehaviour
{
  public int atkDmg;
  public float atkRange;
  public float atkSpeed;
  protected float atkReload;
  protected float modifierAS = 1f;
  protected bool isAttacking = false;
  protected GameObject currentTarget;
  protected List<GameObject> targets { get; } = new List<GameObject>();

  protected virtual void Update()
  {
    atkReload -= Time.deltaTime;
    if (atkReload <= 0)
    {
      if (isAttacking)
        Cancel();
      if (currentTarget == null && targets.Count > 0)
        ChooseTarget();
      StartAttack();
    }
  }

  //Attack
  private void ChooseTarget()
  {
    targets.RemoveAll(elem => elem == null);
    if (targets.Count <= 0)
    {
      currentTarget = null;
      return;
    }
    targets.Sort(SortByDistance);
    currentTarget = targets[0];
  }

  private int SortByDistance(GameObject a, GameObject b)
  {
    return ((this.transform.position - a.transform.position).sqrMagnitude.CompareTo((this.transform.position - b.transform.position).sqrMagnitude));
  }

  private void StartAttack()
  {
    if (currentTarget == null)
    {
      isAttacking = false;
      NoTargets();
      return;
    }
    if (Vector3.Distance(this.transform.position, currentTarget.GetComponent<IAttackable>().GetPosition()) > atkRange)
    {
      isAttacking = false;
      TargetNotInRange();
      return;
    }
    isAttacking = true;
    LaunchAttack();
  }

  //Specifics Implementations
  protected virtual void Cancel() { }

  protected virtual void NoTargets() { }

  protected virtual void TargetNotInRange() { }

  protected virtual void LaunchAttack() { }

  //Triggers
  private void OnTriggerEnter(Collider other)
  {
    if (other.GetComponent<IAttackable>() != null)
      AddTarget(other.gameObject);
  }

  protected virtual void AddTarget(GameObject target) { targets.Add(target); }

  private void OnTriggerExit(Collider other)
  {
    if (other.GetComponent<IAttackable>() != null)
      RemoveTarget(other.gameObject);
  }

  protected virtual void RemoveTarget(GameObject target) { targets.Remove(target); }
}
