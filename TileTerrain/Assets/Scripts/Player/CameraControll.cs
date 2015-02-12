using UnityEngine;
using System.Collections;

public class CameraControll : MonoBehaviour {

	public float speed = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*if(Input.GetKey("w")){
			transform.Translate(Vector3.forward*Time.deltaTime*speed);
		}
		if(Input.GetKey("a")){
			transform.Translate(Vector3.left*Time.deltaTime*speed);
		}
		if(Input.GetKey("s")){
			transform.Translate(Vector3.back*Time.deltaTime*speed);
		}
		if(Input.GetKey("d")){
			transform.Translate(Vector3.right*Time.deltaTime*speed);
		}*/
		transform.position = transform.position + (transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")))*speed;
		Vector3 rot = (transform.localEulerAngles + new Vector3(0, Input.GetAxis("Mouse ScrollWheel")*60, 0));
		transform.localEulerAngles = rot;
		RaycastHit hit;
		if(Physics.Raycast(transform.position, Vector3.down, out hit))
		{
			transform.position = new Vector3(transform.position.x, hit.point.y + 10, transform.position.z);
		}
	}
}
