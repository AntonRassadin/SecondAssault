using UnityEngine;

public class PickUpObject : MonoBehaviour {

    [SerializeField] private float hoverSpeed = 1;
    [SerializeField] private float highDelta = .2f;
    [SerializeField] private float rotationAngleOverOneSecond = 25;
    [SerializeField] private AudioClip pickUpSound;

    private Vector3 originalPos;

    public AudioClip PickUpSound
    {
        get
        {
            return pickUpSound;
        }

        set
        {
            pickUpSound = value;
        }
    }

    protected virtual void Start () {
        originalPos = transform.position;
	}

    protected virtual void Update () {
        transform.position = originalPos + Vector3.up * highDelta * Mathf.Sin(Time.time * hoverSpeed);
        transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up * rotationAngleOverOneSecond * Time.deltaTime);
	}

}
