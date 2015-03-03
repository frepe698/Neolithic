using UnityEngine;
using System.Collections;

public class MenuUnit : MonoBehaviour {

	public int id;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<Animation>().isPlaying == false)
			GetComponent<Animation>().CrossFade("idle_unarmed");
	}

	public void onSelect()
	{
		GetComponent<Animation>().CrossFade("chop_unarmed");
	}
}
