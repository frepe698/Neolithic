using UnityEngine;
using System.Collections;

public class LigthningBolt : MonoBehaviour {

    public LineRenderer line;

    public int segmentCount = 10;
    public float minYDist = 1;
    public float maxYDist = 2;
    public float dist = 0.5f;

    private float nextUpdate = 0;

	// Use this for initialization
	void Start ()
    {
        if (line == null) return;

        line.SetVertexCount(10);
	}

    void Update()
    {
        if (Time.time > nextUpdate)
        {
            float nextY = 0;
            line.SetPosition(0, new Vector3(0, 0, 0));
            for (int i = 1; i < segmentCount; i++)
            {
                nextY += Random.Range(minYDist, maxYDist);
                line.SetPosition(i, new Vector3(Random.Range(-dist, dist), nextY, Random.Range(-dist, dist)));
            }

            nextUpdate = Time.time + Random.Range(0.05f, 0.14f);
        }
    }

	
}
