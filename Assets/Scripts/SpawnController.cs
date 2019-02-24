using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public static SpawnController instance = null;
    public GameObject enemyNexus;
    public GameObject spawnPoint;
    public GameObject spawnPlateform;
    public int playerId;

    public List<Creep> creeps = new List<Creep>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void CreateCreep(Creep creep)
    {
        creeps.Add(creep);
    }

    private void SpawnCreeps()
    {
        Creep tmp;
        foreach(Creep creep in creeps)
        {
            tmp = Instantiate(creep, spawnPoint.transform);
            tmp.Init(spawnPoint.transform.position + (creep.transform.position - spawnPlateform.transform.position), enemyNexus.transform.position, playerId);
            tmp.SetDestination(enemyNexus.transform.position);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            SpawnCreeps();
    }
}
