using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nexus : Tower
{
    public int pPidOwner;
    public Image healthBar;
    public GameObject healthBarGO;

    protected override void Start()
    {
        base.Start();
        if (pPidOwner == 1)
            healthBarGO.transform.Rotate(new Vector3(90f, -180f, 0f));
    }
}
