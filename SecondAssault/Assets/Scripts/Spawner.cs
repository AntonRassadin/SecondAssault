using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [SerializeField] private Enemy enemy;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private float timeBetweenEnemySpawn = 3;
    [SerializeField] private float timeBetweenWawes = 3;
    [SerializeField] private int numberOfWawes = 2;
    [SerializeField] private AudioClip spawnAudio;

    private int enemyCount;
    private int enemyInWawe;
    private Player player;
    private bool isPlayerDead;
    private bool routineAlreadyStart;
    private bool allEnemiesInWawesDead;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.OnDeath += OnPlayerDeath;
        enemyInWawe = transform.childCount;
        enemyCount = enemyInWawe * numberOfWawes;
    }

    private void OnPlayerDeath()
    {
        isPlayerDead = true;
    }

    private void Update()
    {
        if (routineAlreadyStart && !allEnemiesInWawesDead)
        {
            if(enemyInWawe == 0)
            {
                numberOfWawes--;
                {
                    if(numberOfWawes > 0)
                    {
                        StartCoroutine(StartSecondWave());
                        enemyInWawe = transform.childCount;
                    }
                    else
                    {
                        allEnemiesInWawesDead = true;
                        GameManager.instance.OpenDoorA();
                    }
                }
            }
        }
    }

    private void OnEnemyDeath()
    {
        enemyInWawe--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Instantiate(enemy, transform.position + Vector3.up, transform.rotation);
            if (!routineAlreadyStart)
            {
                StartCoroutine(SpawnEnemies());
            }
        }
    }

    private IEnumerator StartSecondWave()
    {
        yield return new WaitForSeconds(timeBetweenWawes);
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        routineAlreadyStart = true;
        foreach (Transform spawnPoint in transform)
        {
            if (!isPlayerDead)
            {
                Enemy newEnemy = Instantiate(enemy, spawnPoint.position + Vector3.up, spawnPoint.rotation, spawnPoint) as Enemy;
                newEnemy.OnDeath += OnEnemyDeath;
                Destroy(Instantiate(spawnEffect, spawnPoint.position, Quaternion.identity), .7f);
                AudioManager.instance.PlayClipWIthVolume(spawnAudio, spawnPoint.GetComponent<AudioSource>());
                yield return new WaitForSeconds(timeBetweenEnemySpawn);
            }
        }
    }
}
