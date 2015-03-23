using UnityEngine;
using System.Collections;

public class DeactivateParticleOnDisable : MonoBehaviour {

    ParticleSystem particles;

    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        if (particles == null) Destroy(this);
    }

    void OnDisable()
    {
        particles.Stop();
    }

    void OnEnable()
    {
        particles.Play();
    }
}
