using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(GunController))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Enemy : LivingEntity {

    public static event System.Action OnDeathStatic;

    [SerializeField] private float viewDistance = 20f;
    [SerializeField] private float shootDistance = 15f;
    [SerializeField] private float shootDelay = .5f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private GameObject[] dropDownItems;
    [SerializeField][Range(0, 1)][Tooltip("вероятность выпадения патронов аптечек - (1 - всегда, 0 - никогда)")]
    private float dropDownRate = .2f;

    [SerializeField] private AudioClip detectSound;
    [SerializeField] private bool alwaysPlayDetectSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip deathFullSound;
    [SerializeField] private float dissapearSpeed = .05f;

    private AudioSource audioSource;
    private GunController gunController;
    private NavMeshAgent pathfinder;
    private Transform target;
    private Rigidbody rb;
    private CapsuleCollider capsuleCol;
    private bool targetSpotted;
    private bool targetInView;
    private float delayCounter;
    private EnemyUI enemyUI;
    private bool playerDead;
    private float refreshRate = .2f;
    private float timeCounter = 0;
    private bool inCollision;
    private Animator anim;
    private bool detectSoundPlayed;
    private bool isDead;
    // Use this for initialization
    protected override void Start () {
        base.Start();
        gunController = GetComponent<GunController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        pathfinder = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        capsuleCol = GetComponent<CapsuleCollider>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        enemyUI = GetComponentInChildren<EnemyUI>();
        enemyUI.gameObject.SetActive(false);
        enemyUI.StartingHealth = startingHealth;
        enemyUI.Health = startingHealth;
        gunController.EquipGun();

        target.GetComponent<Player>().OnDeath += OnPlayerDeath;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isDead)
        {
            if (playerDead == false && !GameManager.instance.IsGameInPause)
            {
                LookForPlayer();
            }
            else
            {
                StopMoveToTarget();
            }

            if (targetSpotted && !detectSoundPlayed)
            {
                detectSoundPlayed = true;
                if (alwaysPlayDetectSound)
                {
                    AudioManager.instance.PlayClipWIthVolume(detectSound, audioSource);
                }
                else
                {
                    int random = Random.Range(0, 3);
                    if (random == 1)
                    {
                        AudioManager.instance.PlayClipWIthVolume(detectSound, audioSource);
                    }
                }

            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    private IEnumerator EnquipGunWithDelay()
    {
        yield return new WaitForSeconds(1);
        gunController.EquipGun();
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if(!isDead){
            targetSpotted = true;
            base.TakeDamage(damage);
            delayCounter = 0;
            StartCoroutine(ThrowFromHit(hitDirection));
            if (damage >= health)
            {
                Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
            }
            enemyUI.gameObject.SetActive(true);
            enemyUI.Health = health;
        }
    }

    private IEnumerator ThrowFromHit(Vector3 hitDirection)
    {
        //эффект отбрасывания врага
        float time = 1f;
        float speed = 1 / time * Time.deltaTime;
        float percent = 0;
        Vector3 oldPosition = transform.position;
        Vector3 newPosition = transform.position + hitDirection * 4;
        while (percent < 1) {
            percent += speed;
            if (!inCollision)
            {
                transform.position = Vector3.Lerp(oldPosition, newPosition, percent);
            }
            else
            {
                yield break;
            }
            yield return null;
        }
    }

    
    private void OnCollisionStay(Collision collision)
    {
        inCollision = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        inCollision = false;
    }

    public override void Die()
    {
        if (OnDeathStatic != null)
        {
            OnDeathStatic();
        }
        base.Die();
        isDead = true;
        rb.isKinematic = true;
        capsuleCol.enabled = false;
        pathfinder.enabled = false;
        int random = Random.Range(0, 3);
        if (random == 1)
        {
            AudioManager.instance.PlayClipWIthVolume(deathFullSound, audioSource);
        }
        else
        {
            AudioManager.instance.PlayClipWIthVolume(deathSound, audioSource);
        }
        anim.SetTrigger("IsDead");
        gunController.DestroyGun();

        if (dropDownItems.Length > 0)
        {
            if (dropDownRate != 0)
            {
                int dropDownRandom = Random.Range(0, Mathf.RoundToInt(1 / dropDownRate));
                if (dropDownRandom == 0)
                {
                    int index = Random.Range(0, dropDownItems.Length);
                    Vector3 position = new Vector3(transform.position.x, .3f, transform.position.z);
                    Instantiate(dropDownItems[index], position, Quaternion.identity);
                }
            }

        }

        StartCoroutine(Death());
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(5);

        float speed = dissapearSpeed * Time.deltaTime;
        while (transform.position.y > -.5f)
        {
            transform.Translate(Vector3.down * speed);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnPlayerDeath()
    {
        playerDead = true;
        target.GetComponent<Player>().OnDeath -= OnPlayerDeath;
    }

    private void LookForPlayer()
    {
        if (targetSpotted == true)
        {
            StartMoveToTarget();
        }

        if (Vector3.Distance(transform.position, target.position) < viewDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, direction);

            if (angleBetween < viewAngle / 2)
            {
                RaycastHit hit;

                if (!Physics.Linecast(transform.position, target.position, out hit, obstacleLayerMask, QueryTriggerInteraction.Ignore))
                {
                    targetSpotted = true;
                    targetInView = true;
                }
                else
                {
                    targetInView = false;
                    delayCounter = 0;
                }

                if(targetInView == true)
                {
                    if (Vector3.Distance(transform.position, target.position) < shootDistance)
                    {
                        StopMoveToTarget();
                        delayCounter += Time.deltaTime;
                        if (delayCounter > shootDelay)
                        {
                            gunController.Shoot();
                        }
                    }
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                    if(Mathf.Abs(Quaternion.Angle(transform.rotation, lookRotation)) > 10)
                    {
                        anim.SetBool("IsWalking", true);
                    }
                }

            }
        }

    }

    private void StartMoveToTarget()
    {
        timeCounter += Time.deltaTime;
        if (timeCounter > refreshRate)
        {
            anim.SetBool("IsWalking", true);
            pathfinder.SetDestination(target.position);
            pathfinder.isStopped = false;
            timeCounter = 0;
        }
    }

    private void StopMoveToTarget()
    {
        anim.SetBool("IsWalking", false);
        pathfinder.isStopped = true;
    }

}
