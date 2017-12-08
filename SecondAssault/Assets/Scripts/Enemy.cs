using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(GunController))]
[RequireComponent(typeof(NavMeshAgent))]
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

    private GunController gunController;
    private NavMeshAgent pathfinder;
    private Transform target;
    private bool targetSpotted;
    private bool targetInView;
    private float delayCounter;
    private EnemyUI enemyUI;
    private bool playerDead;
    private float refreshRate = .2f;
    private float timeCounter = 0;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        gunController = GetComponent<GunController>();
        pathfinder = GetComponent<NavMeshAgent>();
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
        if (playerDead == false && !GameManager.instance.IsGameInPause)
        {
            LookForPlayer();
        }
        else
        {
            StopMoveToTarget();
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
        targetSpotted = true;
        base.TakeDamage(damage);
        if (damage >= health)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
        }
        enemyUI.gameObject.SetActive(true);
        enemyUI.Health = health;
    }

    public override void Die()
    {
        if (OnDeathStatic != null)
        {
            OnDeathStatic();
        }
        base.Die();
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
        Destroy(gameObject);
    }

    private void OnPlayerDeath()
    {
        playerDead = true;
        target.GetComponent<Player>().OnDeath -= OnPlayerDeath;
    }

    private void LookForPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) < viewDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, direction);

            if(targetSpotted == true)
            {
                StartMoveToTarget();
            }

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
                }
            }
        }

    }

    private void StartMoveToTarget()
    {
        timeCounter += Time.deltaTime;
        if (timeCounter > refreshRate)
        {
            pathfinder.SetDestination(target.position);
            pathfinder.isStopped = false;
            timeCounter = 0;
        }
    }

    private void StopMoveToTarget()
    {
        pathfinder.isStopped = true;
    }

}
