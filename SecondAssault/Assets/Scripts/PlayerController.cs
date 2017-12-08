using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(GunController))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float movementSpeed = 5;
    [SerializeField]
    private float lookSensitivity = 5;

    private Player player;
    private GunController gunController;
    private int shotgunAmmo;

    private float gunRotationCurrentVelosityX;
    private float gunRotationCurrentVelosityY;

    public int ShotgunAmmo
    {
        get
        {
            int shotgunAmmoTemp = shotgunAmmo;
            shotgunAmmo = 0;
            return shotgunAmmoTemp;
        }

        set
        {
            shotgunAmmo = value;
        }
    }

    private void Start () {
        player = GetComponent<Player>();
        gunController = GetComponent<GunController>();
        gunController.EquipGun();
        SetMouseSensivity();
        //StartCoroutine(EnquipGunWithDelay());
    }

    private void Update () {
        if (!player.dead && !GameManager.instance.IsGameInPause)
        {
            Move();
            Rotate();

            if (Input.GetMouseButtonDown(0))
            {
                gunController.Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                gunController.Reload();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Shotgun")
        {
            print(GameManager.instance.AllGuns["Shotgun"].Name);
            gunController.EquipGun(GameManager.instance.AllGuns["Shotgun"]);
            gunController.AddSHotgunAmmo(ShotgunAmmo);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "ShotgunAmmo")
        {
            int ammo = other.GetComponent<AmmoPickUp>().AmmoAmount;
            ShotgunAmmo += ammo;
            if (gunController.GetGunName() == "Shotgun")
            {
                gunController.AddSHotgunAmmo(ShotgunAmmo);
            }

            AudioClip audio = other.GetComponent<AmmoPickUp>().PickUpSound;
            if(audio != null)
            {
                AudioManager.instance.PlayCLipAtPlayer(audio);
            }
            Destroy(other.gameObject);
        }else if(other.gameObject.tag == "MedKit")
        {
            if(player.Health < player.StartingHealth)
            {
                int health = other.GetComponent<MedKit>().HealthAmount;
                player.Health += health;
                AudioClip audio = other.GetComponent<MedKit>().PickUpSound;
                if (audio != null)
                {
                    AudioManager.instance.PlayCLipAtPlayer(audio);
                }
                Destroy(other.gameObject);
            }
        }else if(other.gameObject.tag == "EndGame")
        {
            GameManager.instance.EndGame();
        }
    }

    private IEnumerator EnquipGunWithDelay()
    {
        yield return new WaitForSeconds(1);
        gunController.EquipGun();
    }

    private void SetMouseSensivity()
    {
        lookSensitivity = PlayerPrefs.GetFloat("mouse sensivity", 5);
    }

    private void Move()
    {
        float moveInputX = Input.GetAxisRaw("Horizontal");
        float moveInputY = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveInputX;
        Vector3 moveVertical = transform.forward * moveInputY;

        Vector3 moveVelocity = (moveHorizontal + moveVertical).normalized * movementSpeed;
        player.Move(moveVelocity);
    }

    private void Rotate()
    {
        float rotationX = Input.GetAxisRaw("Mouse X") * lookSensitivity;
        Vector3 rotationInput = new Vector3(0, rotationX, 0);
        player.Rotate(rotationInput);
        
        float rotationY = Input.GetAxisRaw("Mouse Y") * lookSensitivity;
        player.RotateCamera(rotationY);

        //поворот оружия при повороте игрока 
        float gunRotationX = Mathf.Clamp(Mathf.SmoothDamp(0, rotationY, ref gunRotationCurrentVelosityY, Time.deltaTime * 10), -10, 10);
        float gunRotationY = Mathf.Clamp(Mathf.SmoothDamp(0, rotationX, ref gunRotationCurrentVelosityX, Time.deltaTime * 10), -10, 10);

        gunController.MoveAnimation(new Vector3(gunRotationX, gunRotationY, 0));
    }
}
