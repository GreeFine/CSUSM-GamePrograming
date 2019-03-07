using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : AAttacker, IAttackable
{
    public GameObject lowerBody;
    public Projectile projectile;
    public GameObject spawnPosition;

    private Quaternion defaultRotation;

    protected virtual void Start()
    {
        defaultRotation = this.transform.rotation;
    }

    //Specifics Implementations
    protected override void Cancel() { }

    protected override void NoTargets()
    {
        lowerBody.transform.rotation = defaultRotation;//TODO : animation
    }

    protected override void TargetNotInRange()
    {
        lowerBody.transform.rotation.SetLookRotation(currentTarget.transform.position);
    }

    protected override void LaunchAttack()
    {
        Projectile tmp;

        if (currentTarget == null)
            return;
        lowerBody.transform.rotation.SetLookRotation(currentTarget.transform.position);
        atkReload = atkSpeed * modifierAS;
        tmp = Instantiate(projectile, this.transform.parent, true);
        tmp.transform.position = spawnPosition.transform.position;
        tmp.Init(currentTarget);
    }

    //Interface : IAttackable
    public Vector3 GetPosition() { return this.transform.position; }

    public GameObject GetGameObject() { return this.gameObject; }

    public bool ReceiveDamage(int dmg)
    {
        //TODO
        Debug.Log("Receive " + dmg.ToString() + " damage");
        return false;
    }
}
