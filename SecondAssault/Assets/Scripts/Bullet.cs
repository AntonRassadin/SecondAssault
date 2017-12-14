using UnityEngine;

public class Bullet : MonoBehaviour {

    // Use this for initialization
    [SerializeField] LayerMask collideWithLayerMask;
    private float speed = 20f;
    public float Speed { get {return speed; } set { speed = value; } }
    private void Start () {
        Destroy(gameObject, 3f);
	}
	
	// Update is called once per frame
	private void Update () {
        CheckCollisions();
        MoveBullet();
	}
    private void FixedUpdate()
    {
        
    }

    private void MoveBullet()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

    }

    private void CheckCollisions()
    {
        Ray ray = new Ray(transform.position, transform.up);
        if (Physics.Raycast(ray, speed * Time.deltaTime, collideWithLayerMask, QueryTriggerInteraction.Ignore))
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }
}
