using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Player : LivingEntity {

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    [SerializeField]
    private float timeToAutoRegenHealth = 2f;

    [Header("Blood Screen Effects")]
    [SerializeField]
    private Image bloodScreen;
    [SerializeField]
    private float bloodScreenTime = .5f;
    [SerializeField][Range(0, 1)]
    private float bloodScreenAlphaPercent = .5f;

    [Header("Death Effect")]
    [SerializeField]
    private float deathAnimationTime = 1f;
    [SerializeField]
    private float deathRotationAngle = 30f;
    [SerializeField]
    private float deathHightDrop = .9f;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            if(value > StartingHealth)
            {
                health = StartingHealth;
            }
            else
            {
                health = value;
            }
            GameManager.instance.UpdateHealthBar(startingHealth, health);
        }
    }


    public float StartingHealth
    {
        get
        {
            return startingHealth;
        }

        set
        {
            startingHealth = value;
        }
    }

    private Rigidbody playerRb;
    private Vector3 moveVelocity;
    private Vector3 rotation;
    private float cameraRotationX;
    private float currentCameraRotation;
    private Color hitEffectOriginalColor;
    public bool dead;
    private Quaternion orinalRotation;
    private float originalHight;
    private float timeToAutoRegenHealthCount;
    private bool isOnFire;
    private float timeSinceLastHit;

    protected override void Start()
    {
        base.Start();
        hitEffectOriginalColor = bloodScreen.color;
        playerRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            PerformMovement();
            PerformRotation();
        }
    }

    private void Update()
    {
        if (isOnFire)
        {
            timeSinceLastHit += Time.deltaTime;
            if(timeSinceLastHit > 5)
            {
                isOnFire = false;
                timeSinceLastHit = 0;
            }
        }

        if(health < startingHealth / 2 && !isOnFire)
        {
            timeToAutoRegenHealthCount += Time.deltaTime;
            if(timeToAutoRegenHealthCount >= timeToAutoRegenHealth)
            {
                Health += .3f;
            }
        }
        else
        {
            timeToAutoRegenHealthCount = 0;
        }
    }

    public override void Die()
    {
        dead = true;
        StartCoroutine(DeathEffect());
        base.Die();
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            isOnFire = true;
            timeSinceLastHit = 0;
            StartCoroutine(BloodScreen());
            base.TakeDamage(damage);
            GameManager.instance.UpdateHealthBar(startingHealth, health);
        }
    }

    private IEnumerator DeathEffect()
    {
        playerRb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        float percent = 0;
        float speed = 1 / deathAnimationTime;

        orinalRotation = transform.rotation;
        Quaternion targetRotation = orinalRotation * Quaternion.Euler(Vector3.forward * deathRotationAngle);
        originalHight = transform.position.y;
        float targetHight = originalHight - deathHightDrop;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            transform.rotation = Quaternion.Lerp(orinalRotation, targetRotation, percent);
            float high = Mathf.Lerp(originalHight, targetHight, percent);
            transform.position = new Vector3(transform.position.x, high, transform.position.z);
            yield return null;
        }
    }

    private IEnumerator BloodScreen()
    {
        Color hitEffectColor = hitEffectOriginalColor;
        hitEffectColor.a = bloodScreenAlphaPercent;
        float percent = 0;
        while(percent < 1)
        {
            float speed = 1 / bloodScreenTime;
            percent += Time.deltaTime * speed;
            Color screenColor = Color.Lerp(hitEffectColor, hitEffectOriginalColor, percent);
            bloodScreen.color = screenColor;
            yield return null;
        }
        
    }

    public void Move(Vector3 _moveVelocity)
    {
        moveVelocity = _moveVelocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }

    public void PerformMovement()
    {
        playerRb.MovePosition(playerRb.position + moveVelocity * Time.fixedDeltaTime);
    }

    public void PerformRotation()
    {
        

        playerRb.MoveRotation(playerRb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            currentCameraRotation -= cameraRotationX;
            currentCameraRotation = Mathf.Clamp(currentCameraRotation, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotation, 0f, 0f);
        }


    }
}
