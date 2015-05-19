using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {

	private int unitID;

	private float animationEndTime;

    private List<GameObject> effectObjects;
	
	private Transform holdParentR;
	private Transform holdParentL;
	private Transform itemTransformR;
	private Transform itemTransformL;

    private const int ARMOR_TYPES = 3;
    private readonly string[] armorRendererNames = 
    {
        "headarmor",
        "chestarmor",
        "bootsarmor",
    };

	private AudioSource audioSource;
    private Collider collider;
    
		
	void Start () {
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.minDistance = 4;
		audioSource.maxDistance = 15;
		audioSource.dopplerLevel = 0;
		holdParentR = transform.FindChild("rig/hold.R");
		holdParentL = transform.FindChild("rig/hold.L");
        collider = GetComponent<Collider>();
        
	}

    void OnDisable()
    {
        if (effectObjects != null)
        {
            foreach (GameObject g in effectObjects)
            {
                if (g != null) Destroy(g);
            }
        }
    }

    public void setWeaponAnimation(string animation, bool rightHand, float speed = 1)
	{
		if(animation != null)
		{
            
			if(rightHand)
			{
				if(itemTransformR == null) return;
                Animation animator = itemTransformR.GetComponent<Animation>();
                AnimationState anim = animator[animation];
				if(anim != null)
				{
					anim.speed = speed;
                    animator.Stop();
                    animator.Play(animation);
				}
			}
			else
			{
				if(itemTransformL == null) return;
                Animation animator = itemTransformL.GetComponent<Animation>();
				AnimationState anim = animator[animation];
				if(anim != null)
				{
					anim.speed = speed;
                    animator.Stop();
                    animator.Play(animation);
				}
			}
		}
	}

	public void setAnimationRestart(string animation, float speed = 1)
	{
        Animation animator = this.GetComponent<Animation>();
		animator.Stop(animation);
		animator[animation].speed = speed;
		animator.CrossFade(animation, 0.2f);
	}

	
	public void setAnimation(string animation, float speed = 1)
	{
        Animation animator = this.GetComponent<Animation>();
        animator[animation].speed = speed;
        animator.CrossFade(animation, 0.2f);
	}

	public void playSound(string sound)
	{
        if (sound == null) return;
        //AudioClip[] clips = Resources.LoadAll("Audio/" + sound, typeof(AudioClip)) as AudioClip[];
		AudioClip clip = (AudioClip)Resources.Load ("Audio/" + sound);
        if (clip == null) return;
		audioSource.PlayOneShot(clip);
	}

	public void setWeapon(WeaponData item)
	{
		if(itemTransformR != null) Destroy(itemTransformR.gameObject);
		if(itemTransformL != null) Destroy(itemTransformL.gameObject);
		bool rightHand = item.rightHand;

		setHoldItem("Holdable/"+item.modelName.ToLower(), rightHand);

		string offhandName = item.getOffhandModelName();
		if(offhandName != null)
		{
			setHoldItem("Holdable/"+offhandName, !rightHand);
		}

	}

	private void setHoldItem(string path, bool rightHand)
	{
		if(rightHand)
		{
			if(holdParentR == null) return;
			
			if(itemTransformR != null) Destroy(itemTransformR.gameObject);
			GameObject itemPrefab = (GameObject)Resources.Load (path);
			
			GameObject go = (GameObject)Instantiate(itemPrefab);
			itemTransformR = go.transform;
			itemTransformR.SetParent(holdParentR);
			itemTransformR.localPosition = Vector3.zero;
			itemTransformR.localEulerAngles = new Vector3(0,180,-90);
		}
		else
		{
			if(holdParentL == null) return;
			
			if(itemTransformL != null) Destroy(itemTransformL.gameObject);
			GameObject itemPrefab = (GameObject)Resources.Load (path);
			
			GameObject go = (GameObject)Instantiate(itemPrefab);
			itemTransformL = go.transform;
			itemTransformL.SetParent(holdParentL);
			itemTransformL.localPosition = Vector3.zero;
			itemTransformL.localEulerAngles = new Vector3(0,180,-90);
		}
	}

    public void unequipAll()
    {
        if (itemTransformL != null) Destroy(itemTransformL.gameObject);
        if (itemTransformR != null) Destroy(itemTransformR.gameObject);
        for (int i = 0; i < ARMOR_TYPES; i++)
        {
            unequipArmor(i);
        }
    }

    public void unequipArmor(int armorType)
    {
        SkinnedMeshRenderer unitArmorRenderer = transform.FindChild(armorRendererNames[armorType]).GetComponent<SkinnedMeshRenderer>();
        if (unitArmorRenderer != null)
        {
            unitArmorRenderer.enabled = false;
        }
    }

    public void equipArmor(string unitName, ArmorData data)
    {
        string datapath = "Armor/"+unitName+"/"+data.modelName;
        GameObject armor = (GameObject)Resources.Load(datapath);
        SkinnedMeshRenderer loadedArmorRenderer = armor.GetComponent<SkinnedMeshRenderer>();
        if (loadedArmorRenderer == null)
        {
            Debug.LogWarning("Could not load armor at " + datapath);
        }
        SkinnedMeshRenderer unitArmorRenderer = transform.FindChild(armorRendererNames[data.armorType]).GetComponent<SkinnedMeshRenderer>();
        if (unitArmorRenderer != null)
        {
            unitArmorRenderer.sharedMesh = loadedArmorRenderer.sharedMesh;
            unitArmorRenderer.materials = loadedArmorRenderer.sharedMaterials;
            unitArmorRenderer.enabled = true;
        }
        
    }

	public void setID(int id)
	{
		this.unitID = id;
	}

	public int getID()
	{
		return unitID;
	}

    public GameObject addEffectObject(GameObject prefab, Vector3 position)
    {
        if (effectObjects == null) effectObjects = new List<GameObject>();
        for (int i = 0; i < effectObjects.Count; i++)
        {
            GameObject g = effectObjects[i];
            if (g == null)
            {
                effectObjects.RemoveAt(i);
                i--;
                continue;
            }
        }
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(transform);
        go.transform.localPosition = position + new Vector3(0, collider.bounds.size.y, 0);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = new Vector3(1, 1, 1);

        effectObjects.Add(go);
        return go;
    }

    public GameObject addEffectObject(GameObject prefab, Vector3 position, float time)
    {
        if (effectObjects == null) effectObjects = new List<GameObject>();

        for (int i = 0; i < effectObjects.Count; i++)
        {
            GameObject g = effectObjects[i];
            if (g == null)
            {
                effectObjects.RemoveAt(i);
                i--;
                continue;
            }
            if (g.name.Equals(prefab.name))
            {
                g.GetComponent<EffectController>().lifeTime = time;
                return g;
            }
        }
        GameObject go = Instantiate(prefab);
        go.name = prefab.name;
        go.transform.SetParent(transform);
        go.transform.localPosition = position + new Vector3(0, collider.bounds.size.y, 0);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = new Vector3(1, 1, 1);
        go.GetComponent<EffectController>().lifeTime = time;

        effectObjects.Add(go);
        return go;
    }

}
