using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	private Light fireLight;
	private float nextChange;

    public float minIntensity = 0.8f;
    public float maxIntensity = 1.0f;
    public float maxChange = 0.1f;

	void Start () 
	{
		fireLight = GetComponentInChildren<Light>();
	}

	void Update () 
	{
		if(Time.time > nextChange)
		{
            fireLight.intensity = Mathf.Clamp(fireLight.intensity + Random.Range(-maxChange, maxChange), minIntensity, maxIntensity);
			nextChange = Time.time + Random.Range(0.08f, 0.12f);
		}

	}
}
