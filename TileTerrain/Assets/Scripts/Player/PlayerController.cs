using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private float speed = 4;
	private float speedMult = 1;
	private float moveSensitivity = 0.05f;
	private float zoom = 0;
	public float maxzoom = 1;
	public float minzoom = 0;

	private Path path;
	private Vector2i endTile = new Vector2i(); 
	private Vector2 dest;
	private bool moving = false;

	private Camera playerCamera;

	// Use this for initialization
	void Start () {
		playerCamera = Camera.main;
		ground();
		animation["run"].speed = 1;
		zoom = minzoom;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("b"))
		{
			Vector2i pos = World.getMap().basePos;
			transform.position = new Vector3(pos.x, 0, pos.y);
			ground();
		}
		if(Input.GetMouseButton(1) || Input.GetMouseButton(0))
		{
			RaycastHit hit;
			if(Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit))
			{
				Vector2 point = new Vector2();
				if(Pathfinder.getClosestWalkablePoint(World.tileMap, new Vector2(hit.point.x, hit.point.z),get2DPos(), out point,0))
				{
					Vector2i clickedTile = new Vector2i(point);
					if(clickedTile == endTile)
					{
						if(path.getCheckPointCount() < 1)
						{
							//path.addCheckPoint(point);
							dest = point;
						}
						else
						{
							path.setPoint(path.getCheckPointCount()-1, point);
						}

						moving = true;

					}
					else
					{
						path = Pathfinder.findPath(World.tileMap,get2DPos(),point,0);
						if (path.getCheckPointCount()>0)
						{
//													for(int i = 0; i < path.getCheckPointCount()-1; i++)
//													{
//														Debug.DrawLine(new Vector3(path.getPoint(i).x, 1, path.getPoint(i).y), new Vector3(path.getPoint(i+1).x, 1, path.getPoint(i+1).y), Color.white, 3);
//													}
							moving = true;
							dest = path.popCheckPoint();
						}
					}
					endTile = clickedTile;

				}


			}
		}
		zoom = Mathf.Clamp(zoom + Input.GetAxis("Mouse ScrollWheel"), minzoom, maxzoom);


		if(moving)
		{
			if(Vector2.Distance(dest, get2DPos()) < moveSensitivity)
			{
				//transform.position = new Vector3(dest.x,transform.position.y, dest.y);
				if(path.getCheckPointCount() > 0) dest = path.popCheckPoint();
				else moving = false;
			}
			else
			{
				Vector2 dir = (dest-get2DPos()).normalized;
				transform.position = transform.position + new Vector3(dir.x, 0, dir.y)*(speed*speedMult*speedMult*Time.deltaTime);
				transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y)+180, 0);
				ground();
				animation.CrossFade("run", 0.1f);
				animation["run"].speed = speedMult*speedMult;
			}
		}
		else
		{
			if(animation.isPlaying)
			{
				animation.Stop("run");

			}
			else
			{
				if(Random.value < 0.2f)
				{
					animation.CrossFade("idle2", 0.2f);
				}
				else
				{
					animation.CrossFade("idle", 0.2f);
				}
			}

		}
		playerCamera.transform.position = transform.position + new Vector3(0.75f-zoom*0.375f, 2-zoom*1f, -0.75f+zoom*0.375f);

	}

	void ground()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position + Vector3.up*50, Vector3.down, out hit))
		{
			transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
			speedMult = hit.normal.y;
		}
	}

	public Vector2 get2DPos()
	{
		return new Vector2(transform.position.x, transform.position.z);
	}
}
