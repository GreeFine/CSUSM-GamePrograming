using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed;
    public int damage;

    private GameObject target;
    private bool isActiv = false;

    public void Init(GameObject enemy)
    {
        target = enemy;
        isActiv = true;
    }

    protected void Update()
    {
        if (isActiv)
        {
            if (!target)
            {
                Destroy(this.gameObject);
                return;
            }
            this.transform.LookAt(target.transform.position);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            if ((this.transform.position - target.transform.position).sqrMagnitude < 0.1f)
            {
                target.GetComponent<IAttackable>().ReceiveDamage(damage);
                Destroy(this.gameObject);
            }
        }
    }
}
