using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwnEffectLight : MonoBehaviour {

    private Light spawnLight;
   

	private void Start () {
        spawnLight = GetComponent<Light>();
        StartCoroutine(LightOn());
	}
	
	private void Update () {

	}

    private IEnumerator LightOn()
    {
        float percent = 0;
        float speed = 1 / 0.3f;
        while (percent < 1) {
            percent += Time.deltaTime * speed;
            spawnLight.intensity = Mathf.Lerp(0, 5, percent);
            yield return null;
        }
        percent = 1;
        while(percent > 0)
        {
            percent -= Time.deltaTime * speed;
            spawnLight.intensity = Mathf.Lerp(0, 5, percent);
            yield return null;
        }
    }
}
