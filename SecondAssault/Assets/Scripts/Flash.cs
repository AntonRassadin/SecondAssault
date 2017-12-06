using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour {

    [SerializeField] private float flashTime;
	// Use this for initialization
	void Start () {
        Deactivate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        gameObject.SetActive(true);
        Invoke("Deactivate", flashTime);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
