using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {

    public event System.Action OnAmmoEmpty;

    [SerializeField] private string gunName;

    [SerializeField] private Bullet bullet;
    [SerializeField] public Transform barrel;

    [SerializeField] private GameObject shell;
    [SerializeField] private Transform shellEjector;

    [Header("Effects")]
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private ParticleSystem bloodEffect;
    [SerializeField] private ParticleSystem playerBloodEffect;

    [SerializeField] private Effects gunEffects;
    [Header("Audio")]
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip reloadAudioClip;
    [SerializeField] private AudioClip pickUpSound;

    [SerializeField] private AudioSource barrelAudioSource;
    

    [Header("Parameters")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float bulletSpeed = 50f;
    [SerializeField] private float msBetweenShots = 200;
    [SerializeField] private int magazineBulletAmount = 10;
    [SerializeField][Tooltip("отрицательное значение - бесконечный боезапас")]
    private int totalBulletAmount = 100;
    [SerializeField]
    [Tooltip("количесвто добавочных патронов при поднятии")]
    private int pickUpBulletAmount = 10;
    [SerializeField] private float reloadTime = 1f;

    [Header("Recoil")]
    [SerializeField] private float recoilTime = .2f;
    [SerializeField] private Vector2 recoilAngleMinMax = new Vector2(2f, 5f);
    [SerializeField] private Vector2 recoilKickMinMax = new Vector2(.05f, .2f);

    private LayerMask layerMask;
    private float nextShotTime;
    private bool playerGun;
    private bool inReload;
    private int magazineBulletsCount;

    private float recoilAngle;

    private float recoilRotSmoothDump;
    private Vector3 recoilKickSmoothDump;

    public bool PlayerGun { get { return playerGun; } set { playerGun = value; } }
    public LayerMask GunLayerMask { get { return layerMask; } set {layerMask = value; } }
    public float RecoilAngle { get { return recoilAngle; } }

    public string Name { get { return gunName; } set { gunName = value; }}

    public int TotalBulletAmount
    {
        get
        {
            return totalBulletAmount;
        }

        set
        {
            totalBulletAmount = value;
            UpdateAmmoUI();
        }
    }

    public int PickUpBulletAmount
    {
        get
        {
            return pickUpBulletAmount;
        }

        set
        {
            pickUpBulletAmount = value;
        }
    }

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

    private void Start()
    {
        StartCoroutine(Reload());
        //magazineBulletsCount = magazineBulletAmount;
        UpdateAmmoUI();
    }

    private void LateUpdate()
    {
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDump, recoilTime);
        if(recoilAngle < 0.1)
        {
            recoilAngle = 0;
        }

        if (!inReload)
        {
            transform.localEulerAngles = Vector3.left * recoilAngle;
        }
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilKickSmoothDump, recoilTime);
    }

    public void Shoot()
    {
        if (nextShotTime < Time.time && !inReload)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            gunEffects.Activate();

            Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .7f, layerMask, QueryTriggerInteraction.Ignore);

            if (initialCollisions.Length == 0)
            {
                // оружие не в обьекте 
                Bullet newBullet;

                if (barrel.transform.childCount > 0)
                {
                    foreach (Transform child in barrel.transform)
                    {
                        newBullet = Instantiate(bullet, child.position, child.rotation * Quaternion.Euler(90f, 0f, 0f)) as Bullet;
                        newBullet.SetSpeed(bulletSpeed);
                        TakeShot(child.position, child.forward);
                    }
                }
                else
                {
                    TakeShot(barrel.position, barrel.forward);
                    newBullet = Instantiate(bullet, barrel.position, barrel.rotation * Quaternion.Euler(90f, 0f, 0f)) as Bullet;
                    newBullet.SetSpeed(bulletSpeed);
                }
            }
            else
            {
                Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                if (playerGun)
                {
                    if (barrel.transform.childCount > 0)
                    {
                        foreach (Transform child in barrel.transform)
                        {
                            TakeShot(cameraRay.origin, cameraRay.direction);
                        }
                    }
                    else
                    {
                        TakeShot(cameraRay.origin, cameraRay.direction);
                    }
                }
                else
                {
                    if (barrel.transform.childCount > 0)
                    {
                        foreach (Transform child in barrel.transform)
                        {
                            TakeShot(transform.position, transform.forward);
                        }
                    }
                    else
                    {
                        TakeShot(transform.position, transform.forward);
                    }
                }
                //    HitObject(initialCollisions[0], transform.position);
                //    for (int i = 1; i < barrel.transform.childCount; i++)
                //    {
                //        HitObject(initialCollisions[0], transform.position);
                //    }
                }
                Instantiate(shell, shellEjector.position, shellEjector.rotation * Quaternion.Euler(90f, 0f, 0f));

            Recoil();

            barrelAudioSource.PlayOneShot(shotSound);

            magazineBulletsCount--;
            UpdateAmmoUI();
            if (magazineBulletsCount <= 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    private void TakeShot(Vector3 barrelPosition, Vector3 barrelForward)
    {
        RaycastHit hit;
        Ray ray = new Ray(barrelPosition, barrelForward);
        if (Physics.Raycast(ray, out hit, 500f, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                //кровь игрока
                //Destroy((Instantiate(playerBloodEffect.gameObject, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject), playerBloodEffect.main.duration);
            }
            else if (hit.collider.gameObject.tag == "Enemy")
            {
                Destroy((Instantiate(bloodEffect.gameObject, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject), bloodEffect.main.duration);
            }
            else
            {
                Destroy((Instantiate(impactEffect.gameObject, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject), impactEffect.main.duration);
            }
            HitObject(hit.collider, hit.point);
        }
    }

    internal void ReloadGun()
    {
        if(magazineBulletsCount < magazineBulletAmount && !inReload)
        {
            StartCoroutine(Reload());
        }
    }

    public void UpdateAmmoUI()
    {
        if (playerGun)
        {
            GameManager.instance.UpdateAmmo(magazineBulletsCount, totalBulletAmount);
        }
    }

    private IEnumerator Reload()
    {
        if(totalBulletAmount != 0)
        {
            inReload = true;
            barrelAudioSource.PlayOneShot(reloadAudioClip);
            StartCoroutine(AnimateReload());
            yield return new WaitForSeconds(reloadTime);
            inReload = false;
            if (totalBulletAmount < 0)
            {
                magazineBulletsCount = magazineBulletAmount;
            }
            else { 
                if ((totalBulletAmount - (magazineBulletAmount - magazineBulletsCount) >= 0))
                {
                    totalBulletAmount -= (magazineBulletAmount - magazineBulletsCount);
                    magazineBulletsCount = magazineBulletAmount;
                }
                else if ((totalBulletAmount - (magazineBulletAmount - magazineBulletsCount) < 0))
                {
                    magazineBulletsCount += totalBulletAmount;
                    totalBulletAmount = 0;
                }
            }
        }
        else
        {
            if (magazineBulletsCount == 0 && totalBulletAmount == 0)
            {
                yield return new WaitForSeconds(.5f);
                if (OnAmmoEmpty != null)
                {
                    OnAmmoEmpty();
                }
            }
        }

        UpdateAmmoUI();
    }

    private IEnumerator AnimateReload()
    {
        inReload = true;
        yield return new WaitForSeconds(.2f);
        float reloadSpeed = 1 / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30f;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        inReload = false;
    }

    private void Recoil()
    {
        recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
        recoilAngle = Mathf.Clamp(recoilAngle, 0, 45);

        transform.localPosition += Vector3.back * Random.Range(recoilKickMinMax.x, recoilKickMinMax.y);
    }

    private void HitObject(Collider col, Vector3 hitPoint)
    {
        IDamageable damageableObject = col.gameObject.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            if (playerGun)
            {
                damageableObject.TakeHit(damage, col.gameObject.transform.position, transform.forward);
            }
            else
            {
                damageableObject.TakeHit(damage, col.transform.position, transform.forward);
            }
        }
    }

    public void AddSHotgunAmmo(int ammo)
    {
        TotalBulletAmount += ammo;
    }

    private void OnDrawGizmos()
    {
        if (barrel.transform.childCount > 0)
        {
            foreach (Transform child in barrel.transform)
            {
                //Gizmos.DrawLine(child.position, child.position + child.forward  * 10);
                Gizmos.DrawRay(child.position, child.forward * 20);
                
                //Gizmos.DrawSphere(transform.position, .7f);
                //Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                //Gizmos.DrawRay(cameraRay.origin, child.forward * 20);
                //Debug.DrawRay(cameraRay.origin, cameraRay.direction * 500, Color.red);
            }
        }
    }
}

