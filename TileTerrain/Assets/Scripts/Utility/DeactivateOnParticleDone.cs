using UnityEngine;
using System.Collections;

public class DeactivateOnParticleDone : MonoBehaviour {

    private ParticleSystem particle;
	void Start () {
        particle = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (particle.isStopped)
        {
            gameObject.SetActive(false);
        }
	}
}
