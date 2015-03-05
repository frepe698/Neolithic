using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {

	private int unitID;

	private float animationEndTime;


	
	private Transform holdParentR;
	private Transform holdParentL;
	private Transform itemTransformR;
	private Transform itemTransformL;

    private readonly string[] armorRendererNames = 
    {
        "headarmor",
        "chestarmor",
        "bootsarmor",
    };

	private AudioSource audioSource;
		
	void Start () {
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.minDistance = 4;
		audioSource.maxDistance = 15;
		audioSource.dopplerLevel = 0;
		holdParentR = transform.FindChild("rig/hold.R");
		holdParentL = transform.FindChild("rig/hold.L");
		
	}

    public void setWeaponAnimation(string animation, bool rightHand, float speed = 1)
	{
		if(animation != null)
		{
			if(rightHand)
			{
				if(itemTransformR == null) return;
				AnimationState anim = itemTransformR.GetComponent<Animation>()[animation];
				if(anim != null)
				{
					anim.speed = speed;
					itemTransformR.GetComponent<Animation>().Stop();
					itemTransformR.GetComponent<Animation>().Play(animation);
				}
			}
			else
			{
				if(itemTransformL == null) return;
				AnimationState anim = itemTransformL.GetComponent<Animation>()[animation];
				if(anim != null)
				{
					anim.speed = speed;
					itemTransformL.GetComponent<Animation>().Stop();
					itemTransformL.GetComponent<Animation>().Play(animation);
				}
			}
		}
	}

	public void setAnimationRestart(string animation, float speed = 1)
	{
		this.GetComponent<Animation>().Stop(animation);
		this.GetComponent<Animation>()[animation].speed = speed;
		this.GetComponent<Animation>().CrossFade(animation, 0.2f);
	}

	
	public void setAnimation(string animation, float speed = 1)
	{
		this.GetComponent<Animation>()[animation].speed = speed;
		this.GetComponent<Animation>().CrossFade(animation, 0.2f);
	}

	public void playSound(string sound)
	{
        if (sound == null) return;
        //AudioClip[] clips = Resources.LoadAll("Audio/" + sound, typeof(AudioClip)) as AudioClip[];
		AudioClip clip = (AudioClip)Resources.Load ("Audio/" + sound);
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

}
