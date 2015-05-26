using UnityEngine;
using System.Collections;
using System;

public class EffectController : MonoBehaviour {

    public float lifeTime = 1;

    public GameObjectSpawn[] gameObjectSpawns;
    public SoundPlayer[] sounds;

    private float startTime;

    private AudioSource audioSource;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        startTime = Time.time;
        Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () 
    {
        foreach (GameObjectSpawn gos in gameObjectSpawns)
        {
            if (!gos.spawned && Time.time >= startTime + gos.spawnTime)
            {
                gos.gameObject.SetActive(true);
                Destroy(gos.gameObject, gos.lifeTime);
                gos.spawned = true;
            }
        }
        foreach (SoundPlayer sp in sounds)
        {
            if (!sp.played && Time.time >= startTime + sp.time)
            {
                audioSource.PlayOneShot(sp.clip);
                sp.played = true;
            }
        }
	}

    [Serializable]
    public class GameObjectSpawn
    {
        public GameObject gameObject;
        public float spawnTime;
        public float lifeTime;
        [HideInInspector]
        public bool spawned = false;
    }

    [Serializable]
    public class SoundPlayer
    {
        public AudioClip clip;
        public float time;

        [HideInInspector]
        public bool played = false;
    }
}


