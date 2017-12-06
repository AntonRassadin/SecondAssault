using UnityEngine;

public class Door : MonoBehaviour {

    private Animator anim;

	private void Start () {
        anim = GetComponent<Animator>();
	}
	
	private void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        anim.SetTrigger("OpenDoors");
    }
}
