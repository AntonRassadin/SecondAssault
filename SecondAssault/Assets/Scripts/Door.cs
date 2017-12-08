using UnityEngine;

public class Door : MonoBehaviour {

    [SerializeField]
    private bool isDoorBlocked;
    private Animator anim;

	private void Start () {
        anim = GetComponent<Animator>();
	}
	
	private void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!isDoorBlocked)
        {
            anim.SetTrigger("OpenDoors");
        }
    }

    public void OpenDoor()
    {
        isDoorBlocked = false;
    }
}
