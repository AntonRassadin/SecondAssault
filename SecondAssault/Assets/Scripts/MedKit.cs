using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKit : PickUpObject {

    [SerializeField] private int healthAmount = 30;

    public int HealthAmount
    {
        get
        {
            return healthAmount;
        }

        set
        {
            healthAmount = value;
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
