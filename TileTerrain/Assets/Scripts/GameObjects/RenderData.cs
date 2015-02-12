using UnityEngine;
using System.Collections;

public class RenderData {

	public readonly Mesh mesh;
	public readonly Material material;
	public readonly MaterialPropertyBlock property;

	public RenderData(GameObject go)
	{
		mesh = go.GetComponent<MeshFilter>().sharedMesh;
		material = go.GetComponent<Renderer>().sharedMaterial;
		property = new MaterialPropertyBlock();
		go.GetComponent<Renderer>().GetPropertyBlock(property);
		property.Clear();
	}
}
