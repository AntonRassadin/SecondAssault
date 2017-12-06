using UnityEngine;

public class AmmoPickUp : PickUpObject {

    [SerializeField] private int ammoAmount = 10;

    public int AmmoAmount { get { return ammoAmount; } set { ammoAmount = value; } }

    protected override void Start () {
        base.Start();
	}

    protected override void Update () {
        base.Update();
	}
}
