using UnityEngine;
using System.Collections;

public class WorldSection {

	public static readonly int SIZE = 64;

	private Vector2i tileMapPos;

	private Vector3[] newVertices;
	private Vector2[] newUV;
	private Vector3[] normals;
	private int[] triangles;
	private Mesh mesh;

	public WorldSection(Vector2i worldPos)
	{
		tileMapPos = new Vector2i(worldPos.x*SIZE, worldPos.y*SIZE);
		createMeshFromTileMap();

	}
	

	public float getVertHeight(int x, int y)
	{
		return newVertices[x*SIZE + y].y;
	}

	void createMeshFromTileMap()
	{
		int vertCount = SIZE+1;
		newVertices = new Vector3[(vertCount)*(vertCount)];
		triangles = new int[SIZE*SIZE*6];
		
		int lastTile = 0;
		for(int x = 0; x < vertCount; x++)
		{
			for(int y = 0; y < vertCount; y++)
			{
				newVertices[x*vertCount + y] = new Vector3(x, calcVertHeight(x+tileMapPos.x, y+tileMapPos.y), y);
				
				if(y < SIZE && x < SIZE)
				{
					triangles[(lastTile*6) + 0] = (x*vertCount + y) + 0;
					triangles[(lastTile*6) + 1] = (x*vertCount + y) + 1;
					triangles[(lastTile*6) + 2] = (x*vertCount + y) + vertCount;
					
					triangles[(lastTile*6) + 3] = (x*vertCount + y) + vertCount;
					triangles[(lastTile*6) + 4] = (x*vertCount + y) + 1;
					triangles[(lastTile*6) + 5] = (x*vertCount + y) + vertCount+1;
					lastTile++;
				}
			}
		}
	}

	void createMeshFromTileMapOverlap()
	{
		int size = SIZE+2;
		int vertCount = size+1;
		newVertices = new Vector3[(vertCount)*(vertCount)];
		triangles = new int[size*size*6];
		
		int lastTile = 0;
		for(int x = 0; x < vertCount; x++)
		{
			for(int y = 0; y < vertCount; y++)
			{
				newVertices[x*vertCount + y] = new Vector3(x-1, calcVertHeightOverlap(x-1+tileMapPos.x, y-1+tileMapPos.y), y-1);
				
				if(y < size && x < size)
				{
					int vert = x*vertCount + y;
					triangles[(lastTile*6) + 0] = vert + 0;
					triangles[(lastTile*6) + 1] = vert + 1;
					triangles[(lastTile*6) + 2] = vert + vertCount;
					
					triangles[(lastTile*6) + 3] = vert + vertCount;
					triangles[(lastTile*6) + 4] = vert + 1;
					triangles[(lastTile*6) + 5] = vert + vertCount+1;
					lastTile++;
				}
			}
		}
	}

	public Vector3 getTriangleNormal(int index)
	{
		Vector3 v1 = newVertices[triangles[index+1]] - newVertices[triangles[index]];
		Vector3 v2 = newVertices[triangles[index+2]] - newVertices[triangles[index]];

		return Vector3.Cross(v1, v2);
	}

	public void calculateNormals(Vector3[,] triNormals)
	{
		int vertCount = SIZE+1;
		normals = new Vector3[vertCount*vertCount];
		for(int x = 0; x < vertCount; x++)
		{
			for(int y = 0; y < vertCount; y++)
			{
				int mapX = 1;
				int mapY = 1;
				Vector3 normal = Vector3.zero;
				bool up = tileMapPos.y > 0 || y > 0;
				bool right = tileMapPos.x < (World.sectionCount-1)*SIZE || x < SIZE;
				bool down = tileMapPos.y < (World.sectionCount-1)*SIZE || y < SIZE;
				bool left = tileMapPos.x > 0 || x > 0;
				if(up)
				{
					normal += triNormals[mapX, mapY-1];
					if(right)
					{
						normal += triNormals[mapX, mapY-1];
					}

				}
				if(right)
				{
					normal += triNormals[mapX, mapY];
				}
				if(down)
				{
					normal += triNormals[mapX, mapY];
					if(left)
					{
						normal += triNormals[mapX-1, mapY];
					}
				}
				if(left)
				{
					normal += triNormals[mapX-1, mapY];
				}

				//normals[x + y*vertCount] = normal.normalized;
				normals[x + y*vertCount] = Vector3.up;

			}
		}
		if(mesh == null)
		{
			generateMesh();
		}
		mesh.normals = normals;
		calculateTangents(mesh);
	}

	private void generateMesh()
	{
		mesh = new Mesh();
		
		mesh.vertices = newVertices;
		newUV = new Vector2[newVertices.Length];
		for(int uv = 0; uv < newUV.Length; uv++)
		{
			newUV[uv] = new Vector2((newVertices[uv].x+tileMapPos.x)/(SIZE*World.sectionCount), (newVertices[uv].z+tileMapPos.y)/(SIZE*World.sectionCount));
		}
		mesh.uv = newUV;
		mesh.subMeshCount = 1;
		mesh.SetTriangles(triangles, World.GRASS);
		
		//mesh.RecalculateNormals();
		//mesh.RecalculateBounds();
		


	}

	public Mesh getMesh()
	{

		return mesh;
	}

	public static Texture2D[] getWorldSplatTexture(int[,] tileMapColor, int texSize)
	{
		SplatMapColor[] colors = new SplatMapColor[]
		{
			new SplatMapColor(new Color(1,0,0,0), new Color(0,0,0,0)),
			new SplatMapColor(new Color(0,1,0,0), new Color(0,0,0,0)),
			new SplatMapColor(new Color(0,0,1,0), new Color(0,0,0,0)),
			new SplatMapColor(new Color(0,0,0,1), new Color(0,0,0,0)),
			new SplatMapColor(new Color(0,0,0,0), new Color(1,0,0,0)),
			new SplatMapColor(new Color(0,0,0,0), new Color(0,1,0,0)),
			new SplatMapColor(new Color(0,0,0,0), new Color(0,0,1,0)),
			new SplatMapColor(new Color(0,0,0,0), new Color(0,0,0,1)),
		};
		Texture2D[] textures = new Texture2D[SplatMapColor.colorCount];
		for(int i = 0; i< SplatMapColor.colorCount; i++)
		{
			textures[i] = new Texture2D(texSize, texSize);
		}

		for(int x = 0; x < texSize; x++)
		{
			for(int y = 0; y < texSize; y++)
			{	
				int xplus = x;
				int xminus = x-1;
				int yplus = y;
				int yminus = y-1;
				
				SplatMapColor color = colors[tileMapColor[xplus, yplus]];
				int colorCount = 1;
				if(xminus >= 0 && yminus >= 0) 
				{
					color += colors[tileMapColor[xminus, yminus]];
					colorCount++;
				}
				if(xminus >= 0)
				{
					color+= colors[tileMapColor[xminus, yplus]];
					colorCount++;
				}
				if(yminus >= 0)
				{
					color+= colors[tileMapColor[xplus, yminus]];
					colorCount++;
				}
				color/=colorCount;
				for(int i = 0; i < SplatMapColor.colorCount; i++)
				{
					textures[i].SetPixel(x, y, color.colors[i]);
				}
			}
		}
		for(int i = 0; i < SplatMapColor.colorCount; i++)
		{
			textures[i].wrapMode = TextureWrapMode.Clamp;
			textures[i].Apply();
		}
		return textures;
	}

	public static Texture2D[] getNoiseWorldSplatTexture(int texSize)
	{
		SplatMapColor[] colors = new SplatMapColor[]
		{
			new SplatMapColor(new Color(1,0,0,0), new Color(0,0,0,0)),
			new SplatMapColor(new Color(0,1,0,0), new Color(0,0,0,0)),
			new SplatMapColor(new Color(0,0,1,0), new Color(0,0,0,0)),
			new SplatMapColor(new Color(0,0,0,1), new Color(0,0,0,0)),
			new SplatMapColor(new Color(0,0,0,0), new Color(1,0,0,0)),
			new SplatMapColor(new Color(0,0,0,0), new Color(0,1,0,0)),
			new SplatMapColor(new Color(0,0,0,0), new Color(0,0,1,0)),
			new SplatMapColor(new Color(0,0,0,0), new Color(0,0,0,1)),
		};
		Texture2D[] textures = new Texture2D[SplatMapColor.colorCount];
		for(int i = 0; i< SplatMapColor.colorCount; i++)
		{
			textures[i] = new Texture2D(texSize, texSize);
		}

		for(int x = 0; x < texSize; x++)
		{
			for(int y = 0; y < texSize; y++)
			{
				SplatMapColor color = colors[(int)(((float)x/(float)texSize)*8)];
				for(int i = 0; i < SplatMapColor.colorCount; i++)
				{
					textures[i].SetPixel(x, y, color.colors[i]);
				}
			}
		}
		for(int i = 0; i < SplatMapColor.colorCount; i++)
		{
			textures[i].wrapMode = TextureWrapMode.Clamp;
			textures[i].Apply();
		}
		return textures;
	}

	void calculateTangents(Mesh mesh)
	{
		Vector3[] normals = mesh.normals;
		int count = normals.Length;

		Vector4[] tangents = new Vector4[count];
		for(int i = 0; i < count; i++)
		{
			Vector3 n = normals[i];
			tangents[i] = new Vector4(n.x, n.y, n.z, 1);
		}

		mesh.tangents = tangents;
	}

	float calcVertHeight(int x, int y)
	{
		float height = 0;
		int div = 0;
		for(int i = x-1; i < x+1; i++)
		{
			for(int j = y-1; j< y+1; j++)
			{
				if(World.tileMap.isValidTile(i, j))
				{
					height += World.tileMap.getTile(i, j).height;
					div++;
				}
			}
		}
		return height/(div+0.00000001f);
	}

	float calcVertHeightOverlap(int x, int y)
	{
		float height = 0;
		int div = 0;
		for(int i = x-1; i < x+1; i++)
		{
			for(int j = y-1; j< y+1; j++)
			{
				if(World.tileMap.isValidTile(i, j))
				{
					height += World.tileMap.getTile(i, j).height;
					div++;
				}
			}
		}
		return height/(div+0.00000001f);
	}
}
