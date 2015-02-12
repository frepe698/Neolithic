using UnityEngine;
using System.Collections;

public class MenuUnit : MonoBehaviour {

	public int id;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(animation.isPlaying == false)
			animation.CrossFade("idle_unarmed");
	}

	public void onSelect()
	{
		animation.CrossFade("chop_unarmed");
	}
}
