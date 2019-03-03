using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using UnityEngine.UI;

public class Unit : NetworkBehaviour, IAttackable
{
    //TODO : struct to organise variables
    public bool hasAnimator = false;
    public Image healthBar;
    public int maxLife;
    public int atkDmg;
    public float atkRange;
    public float atkSpeed;
    public float animDmgTime;

    private int pidOwner = -9;
    private int currentLife;
    private float atkReload = 0f;
    private float modifierAS = 1f; //TODO : implem AS buff/debuff setter
    private float modifierMS = 1f; //TODO : implem unit speed + animation
    private bool isAttacking = false;
    private Vector3 enemyNexus;
    private List<GameObject> targets = new List<GameObject>();
    private GameObject currentTarget = null;
    private bool isDead = false;
    

    private void Start()
    {
        NavMeshAgent agent = this.gameObject.AddComponent<NavMeshAgent>();
        agent.SetDestination(enemyNexus);
        ChangeAnimation("Run");
    }

    [ClientRpc]
    public void RpcInit(int pPidOwner, Vector3 pStartPos, Vector3 pEnemyNexus)
    {
        pidOwner = pPidOwner;
        this.gameObject.layer = 9 + pidOwner;
        this.transform.position = pStartPos;
        enemyNexus = pEnemyNexus;
        currentLife = maxLife;
        if (pPidOwner == 1)
        {
            healthBar.GetComponentInParent<Canvas>().GetComponent<RectTransform>().Rotate(new Vector3(1, 0, 0), -90);
            healthBar.fillOrigin = 0;
        }
    }

    public void SetDestination(Vector3 dest)
    {
        GetComponent<NavMeshAgent>().SetDestination(dest);
    }

    private void Update()
    {
        atkReload -= Time.deltaTime;
        if (atkReload <= 0)
        {
            if (isAttacking)
                ChangeAnimation("Idle");
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
            isAttacking = false;
            ChangeAnimation("Run");
            GetComponent<NavMeshAgent>().destination = enemyNexus;
            GetComponent<NavMeshAgent>().isStopped = false;
            return;
        }
        if (Vector3.Distance(this.transform.position, currentTarget.GetComponent<IAttackable>().GetPosition()) > atkRange)
        {
            isAttacking = false;
            GetComponent<NavMeshAgent>().destination = currentTarget.GetComponent<IAttackable>().GetPosition();
            return;
        }
        isAttacking = true;
        GetComponent<NavMeshAgent>().isStopped = true;
        atkReload = atkSpeed * modifierAS;
        ChangeAnimation("Attack", atkSpeed * modifierAS);
        StartCoroutine(AtkWindUpComplete(animDmgTime * modifierAS, currentTarget));
    }

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
        else
        healthBar.fillAmount = (float)currentLife / (float)maxLife;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}