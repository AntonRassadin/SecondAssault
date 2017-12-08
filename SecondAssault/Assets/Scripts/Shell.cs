using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {

    [SerializeField] private Vector2 forceMinMax = new Vector2(75f, 125f);
    [SerializeField] private float fadeTime = 2f;
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private AudioClip[] audioClips;

    private Rigidbody shellRigidbody;
    private AudioSource audioSource;
   
	// Use this for initialization
	void Start () {
        shellRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        float randomForce = Random.Range(forceMinMax.x, forceMinMax.y);
        shellRigidbody.AddForce(transform.right * randomForce);
        shellRigidbody.AddTorque(Random.insideUnitSphere);

        StartCoroutine(Fade());
        StartCoroutine(PlaySound());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(.5f);
        int randomIndex = Random.Range(0, audioClips.Length - 1);
        //AudioManager.instance.PlayCLipAtPos(audioClips[randomIndex], transform.position);
        float volume = PlayerPrefs.GetFloat("volume", .5f);
        audioSource.PlayOneShot(audioClips[randomIndex], volume);
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);

        Material mat = GetComponent<Renderer>().material;
        Color originalColor = mat.color;
        Color targetColor = Color.clear;

        float percent = 0;
        float fadeSpeed = 1 / fadeTime;

        while(percent < 1)
        {
            percent += fadeSpeed * Time.deltaTime;
            mat.color = Color.Lerp(originalColor, targetColor, percent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
