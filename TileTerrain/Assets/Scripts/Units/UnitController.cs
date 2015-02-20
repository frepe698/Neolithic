using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {

	private int unitID;

	private float animationEndTime;


	
	private Transform holdParentR;
	private Transform holdParentL;
	private Transform itemTransformR;
	private Transform itemTransformL;

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
				AnimationState anim = itemTransformR.animation[animation];
				if(anim != null)
				{
					anim.speed = speed;
					itemTransformR.animation.Stop();
					itemTransformR.animation.Play(animation);
				}
			}
			else
			{
				if(itemTransformL == null) return;
				AnimationState anim = itemTransformL.animation[animation];
				if(anim != null)
				{
					anim.speed = speed;
					itemTransformL.animation.Stop();
					itemTransformL.animation.Play(animation);
				}
			}
		}
	}

	public void setAnimationRestart(string animation, float speed = 1)
	{
		this.animation.Stop(animation);
		this.animation[animation].speed = speed;
		this.animation.CrossFade(animation, 0.2f);
	}

	
	public void setAnimation(string animation, float speed = 1)
	{
		this.animation[animation].speed = speed;
		this.animation.CrossFade(animation, 0.2f);
	}

	public void playSound(string sound)
	{
        if (sound == null) return;
        //AudioClip[] clips = Resources.LoadAll("Audio/" + sound, typeof(AudioClip)) as AudioClip[];
		AudioClip clip = (AudioClip)Resources.Load ("Audio/" + sound);
		audioSource.PlayOneShot(clip);
	}

	public void setItem(WeaponData item)
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

	public void setID(int id)
	{
		this.unitID = id;
	}

	public int getID()
	{
		return unitID;
	}

}
