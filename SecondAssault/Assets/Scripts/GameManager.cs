using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Color healthBarLowColor;
    [SerializeField] private float healthBarBlinkTime = .1f;
    [SerializeField] private Text ammo;
    

    [Header("Guns")][SerializeField] public Gun[] guns;
    private Dictionary<string, Gun> allGuns;

    public static GameManager instance = null;
    private AudioSource playerAudioSource;

    private Color healthBarOriginalColor;
    private float healthbarWidth;
    private Player player;
    private int score;
    private bool gameOver;
    private bool isHalfHealth;
    private bool isHealthBlinking;

    public Dictionary<string, Gun> AllGuns {get {return allGuns;} set {allGuns = value;} }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Enemy.OnDeathStatic += OnEnemyDeath;
        player = FindObjectOfType<Player>();
        player.OnDeath += OnPlayerDeath;
        playerAudioSource = player.GetComponent<AudioSource>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        healthbarWidth = healthBar.rectTransform.sizeDelta.x;
        healthBarOriginalColor = healthBar.color;
        allGuns = new Dictionary<string, Gun>();
        foreach (Gun gun in guns)
        {
            allGuns.Add(gun.Name, gun);
        }
    }
    private void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
    private void OnPlayerDeath()
    {
        gameOver = true;
        inGameUI.SetActive(false);
        Invoke("SetGameOverUIActive", 1f);
        //player.OnDeath -= OnPlayerDeath;
    }

    public void PlaySounAtPlayer(AudioClip audioCLip)
    {
        playerAudioSource.PlayOneShot(audioCLip);
    }

    private void SetGameOverUIActive()
    {
        gameOverUI.SetActive(true);
    }

    private void OnEnemyDeath()
    {
        score++;
        print(score);
    }

    private void OnDestroy()
    {
        Enemy.OnDeathStatic -= OnEnemyDeath;
    }

    public void UpdateHealthBar(float startingHealth, float health)
    {
        float healthPecrent = health / startingHealth;
        if(health < startingHealth / 2)
        {
            isHalfHealth = true;
            if (!isHealthBlinking)
            {
                StartCoroutine(HealthBarColorBlink());
            }
        }
        else
        {
            isHalfHealth = false;
        }
        healthBar.rectTransform.sizeDelta = new Vector2(healthbarWidth * healthPecrent, healthBar.rectTransform.sizeDelta.y);
        healthText.text = (Mathf.RoundToInt(100 * healthPecrent)).ToString();
    }

    public void UpdateAmmo(int ammoInGun, int totalAmmo)
    {
        if(totalAmmo < 0)
        {
            totalAmmo = 999;
        }
        ammo.text = ammoInGun + "/" + totalAmmo;
    }

    private IEnumerator HealthBarColorBlink()
    {
        isHealthBlinking = true;
        float percent = 0;
        float speed = 1 / healthBarBlinkTime;
        while(isHalfHealth)
        {
            while(percent < 1)
            {
                percent += Time.deltaTime * speed;
                healthBar.color = Color.Lerp(healthBarOriginalColor, healthBarLowColor, percent);
                yield return null;
            }
            while(percent > 0)
            {
                percent -= Time.deltaTime * speed;
                healthBar.color = Color.Lerp(healthBarOriginalColor, healthBarLowColor, percent);
                yield return null;
            }
        }
        isHealthBlinking = false;
    }
}
