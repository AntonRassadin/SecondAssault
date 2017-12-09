using UnityEngine;

public enum GunOwner { Enemy, Player }

public class GunController : MonoBehaviour {

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GunOwner gunOwner;
    [SerializeField] private Gun defaultGun;
    [SerializeField] Transform gunHolder;

    //private int ammo;
    private Gun equippedGun;
    // Use this for initialization
	
	// Update is called once per frame
	void LateUpdate () {
        if (equippedGun != null)
        {
            LookOnTarget();
        }

    }

    private void LookOnTarget()
    {
        if (gunOwner == GunOwner.Player)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            //Debug.DrawRay(cameraRay.origin, cameraRay.direction * 500, Color.red);

            Ray barrelRay = new Ray(equippedGun.barrel.position, equippedGun.barrel.forward);
            Ray gunRay = new Ray(equippedGun.transform.position, equippedGun.transform.forward);

            Debug.DrawRay(barrelRay.origin, barrelRay.direction * 500);

            RaycastHit hit;
            Vector3 lookPoint;

            if (Physics.Raycast(cameraRay, out hit, 500f, layerMask, QueryTriggerInteraction.Ignore))
            {
                //поворот ствола в сторону обьекта в зависимости от расстояния

                if (equippedGun.RecoilAngle != 0)
                {
                    //корректировка выстрела с учетом отдачи
                    lookPoint = new Vector3(hit.point.x, gunRay.GetPoint(hit.distance).y, hit.point.z);
                }
                else
                {
                    lookPoint = hit.point;
                }
                equippedGun.barrel.LookAt(lookPoint);

                //поворот оружия в зависимости от удаленности обьекта 
                //Vector3 lookPoint = Vector3.Lerp(gunRay.GetPoint(500f), hit.point, Time.deltaTime * 50);
                //equippedGun.transform.LookAt(lookPoint);
            }
            else
            {
                if (equippedGun.RecoilAngle != 0)
                {
                    lookPoint = new Vector3(cameraRay.GetPoint(500f).x, gunRay.GetPoint(500f).y, cameraRay.GetPoint(500f).z);
                }
                else
                {
                    lookPoint = cameraRay.GetPoint(500f);
                }
                equippedGun.barrel.LookAt(lookPoint);

                //Vector3 lookPoint = Vector3.Lerp(gunRay.GetPoint(500f), cameraRay.GetPoint(500f), Time.deltaTime * 5);
                //equippedGun.transform.LookAt(lookPoint);
            }
        }
        else if (gunOwner == GunOwner.Enemy)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            Debug.DrawRay(ray.origin, ray.direction);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 500f, layerMask))
            {
                if (hit.distance < 2f)
                {
                    gunHolder.LookAt(ray.GetPoint(2f));
                }
                else
                {
                    gunHolder.LookAt(hit.point);
                }
            }
            else
            {
                gunHolder.LookAt(ray.GetPoint(500f));
            } 
        }
    }

    public void MoveAnimation(Vector3 rotation)
    {
        gunHolder.localEulerAngles = rotation;
    }

    public void AddSHotgunAmmo(int ammo)
    {
        if(equippedGun.Name == "Shotgun")
        {
            equippedGun.AddSHotgunAmmo(ammo);
        }
    }

    internal void Reload()
    {
        equippedGun.ReloadGun();
    }

    public void Shoot()
    {
        if(equippedGun != null)
        {
            equippedGun.Shoot();
        }
    }

    public string GetGunName()
    {
        string name = "";
        if (equippedGun != null)
        {
            name = equippedGun.Name;
            return name;
        }
        return name;
    }

    private void EquipGun(LayerMask _layerMask, Gun gun)
    {
        if (equippedGun != null) {
            if (equippedGun.Name != gun.Name)
            {
                Destroy(equippedGun.gameObject);
            }
            else
            {
                if(equippedGun.TotalBulletAmount >= 0)
                {
                    equippedGun.TotalBulletAmount += equippedGun.PickUpBulletAmount;
                    equippedGun.PlaySound(equippedGun.PickUpSound);
                }
                return;
            }
        }
        equippedGun = Instantiate(gun, gunHolder.position, gunHolder.rotation) as Gun;
        equippedGun.GunLayerMask = _layerMask;
        equippedGun.gameObject.transform.parent = gunHolder;
        if(gunOwner == GunOwner.Player)
        {
            equippedGun.PlayerGun = true;
            equippedGun.OnAmmoEmpty += OnAmmoEmpty;
            SetChildLayer(LayerMask.NameToLayer("Gun"), equippedGun.gameObject);
        }
    }

    private void OnAmmoEmpty()
    {
        equippedGun.OnAmmoEmpty -= OnAmmoEmpty;
        Destroy(equippedGun.gameObject);
        EquipGun(GameManager.instance.guns[0]);
    }

    public void EquipGun()
    {
        EquipGun(layerMask, defaultGun);
    }

    public void EquipGun(Gun gun)
    {
        EquipGun(layerMask, gun);
    }

    public void SetChildLayer(LayerMask layer, GameObject obj)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = layer;
            SetChildLayer(layer, child.gameObject);
        }
    }


}
