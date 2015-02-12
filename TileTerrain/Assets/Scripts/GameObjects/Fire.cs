using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	private Light fireLight;
	private float nextChange;

	void Start () 
	{
		fireLight = GetComponentInChildren<Light>();
	}

	void Update () 
	{
		if(Time.time > nextChange)
		{
			fireLight.intensity = Mathf.Clamp(fireLight.intensity + Random.Range(-0.1f, 0.1f), 0.8f, 1f);
			nextChange = Time.time + Random.Range(0.08f, 0.12f);
		}

	}
}
