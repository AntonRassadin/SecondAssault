using UnityEngine;

public class Effects : MonoBehaviour {

    [SerializeField] private ParticleSystem flash;
    [SerializeField] private Light flashLight;
    [SerializeField] private float flashTime = 0.1f;
    // Use this for initialization
    void Start()
    {
        DeactivateFlash();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate()
    {
        flash.Play();
        flashLight.enabled = true;
        Invoke("DeactivateFlash", flashTime);
    }

    private void DeactivateFlash()
    {
        flashLight.enabled = false;
    }

}
