using UnityEngine;
using System.Collections;

public class WorldSection {

	public static readonly int SIZE = 64;

	private readonly Vector2i tileMapPos;

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


	public Vector3 getTriangleNormal(int index)
	{
        index *= 3;
		Vector3 v1 = newVertices[triangles[index+1]] - newVertices[triangles[index]];
		Vector3 v2 = newVertices[triangles[index+2]] - newVertices[triangles[index]];

        Vector3 normal = Vector3.Normalize(Vector3.Cross(v1, v2)); 

       /* if(false)
        {
            Debug.Log("Index: " + index + ": " + newVertices[triangles[index]] + ": " + normal);

            World.normalsstart.Add(newVertices[triangles[index]] + new Vector3(tileMapPos.x, 0, tileMapPos.y));
            World.normalsend.Add(newVertices[triangles[index]] + normal + new Vector3(tileMapPos.x, 0, tileMapPos.y));

        }*/
              
		return normal;
	}

	public void calculateNormals(Vector3[,] triNormals)
	{
		int vertCount = SIZE+1;
		normals = new Vector3[vertCount*vertCount];
		for(int x = 0; x < vertCount; x++)
		{
			for(int y = 0; y < vertCount; y++)
			{
				int mapX = (x + tileMapPos.x)*2;
				int mapY = y + tileMapPos.y;
                
				Vector3 normal = Vector3.zero;
				bool up = mapY > 0;
				bool right = mapX < World.sectionCount*SIZE*2;
				bool down = mapY < World.sectionCount*SIZE;
				bool left = mapX > 0;
                if (up)
                {

                    if (right)
                    {
                        normal += triNormals[mapX, mapY - 1];
                        normal += triNormals[mapX + 1, mapY - 1];
                    }
                    else
                    {
                       
                        normal += new Vector3(0, 2, 0);
                    }
                    if (left)
                    {
                        normal += triNormals[mapX - 1, mapY - 1];
                    }
                    else
                    {
                        normal += new Vector3(0, 1, 0);
                    }

                }
                else
                {
                    normal += new Vector3(0, 3, 0);
                }

				
				if(down)
				{
					if(left)
					{
						normal += triNormals[mapX - 1, mapY];
                        normal += triNormals[mapX - 2, mapY]; 
					}
                    else
                    {
                        normal += new Vector3(0, 2, 0);
                    }
                    if(right)
                    {
                        normal += triNormals[mapX, mapY];
                    }
                    else
                    {
                       normal += new Vector3(0, 1, 0);
                    }
				}
                else
                {
                    normal += new Vector3(0, 3, 0);
                }


                //normals[y + x * vertCount] = Vector3.Normalize(normal);
                normals[y + x * vertCount] = new Vector3(0,1,0);
                //if ( /*(x == 0 && y == SIZE) || (x == SIZE && y == 0) || */(x == 0 || y == 0) || (x == SIZE || y == SIZE)) 
                //{
                      //Debug.Log(tileMapPos.x + ", " + tileMapPos.y + ": " + x + ", " + y + ": " + ": " + (x + y * vertCount) + ": " + normals[x + y * vertCount]);
                      //World.normalsstart.Add(new Vector3(tileMapPos.x, 0, tileMapPos.y) + newVertices[y + x * vertCount]);
                      //World.normalsend.Add(new Vector3(tileMapPos.x, 0, tileMapPos.y) + newVertices[y + x * vertCount] + normals[y + x * vertCount]);
                
                //}
			}
		}
		if(mesh == null)
		{
			generateMesh();
		}
		mesh.normals = normals;
		calculateTangents();
	}

    public void calculateNormalsNew(Vector3[,] triNormals)
    {
        int vertCount = SIZE + 1;
        normals = new Vector3[vertCount * vertCount];
        for (int x = 0; x < vertCount; x++)
        {
            for (int y = 0; y < vertCount; y++)
            {
                int mapX = (x + tileMapPos.x) * 2;
                int mapY = y + tileMapPos.y;

                Vector3 normal = Vector3.zero;
                bool up = mapY > 0;
                bool right = mapX < World.sectionCount * SIZE * 2;
                bool down = mapY < World.sectionCount * SIZE;
                bool left = mapX > 0;
                if (up)
                {

                    if (right)
                    {
                        normal += triNormals[mapX, mapY - 1];
                        normal += triNormals[mapX + 1, mapY - 1];
                    }
                    else
                    {

                        normal += new Vector3(0, 2, 0);
                    }
                    if (left)
                    {
                        normal += triNormals[mapX - 1, mapY - 1];
                    }
                    else
                    {
                        normal += new Vector3(0, 1, 0);
                    }

                }
                else
                {
                    normal += new Vector3(0, 3, 0);
                }


                if (down)
                {
                    if (left)
                    {
                        normal += triNormals[mapX - 1, mapY];
                        normal += triNormals[mapX - 2, mapY];
                    }
                    else
                    {
                        normal += new Vector3(0, 2, 0);
                    }
                    if (right)
                    {
                        normal += triNormals[mapX, mapY];
                    }
                    else
                    {
                        normal += new Vector3(0, 1, 0);
                    }
                }
                else
                {
                    normal += new Vector3(0, 3, 0);
                }


                normals[y + x * vertCount] = Vector3.Normalize(normal);
                //normals[y + x * vertCount] = new Vector3(0,1,0);
                if ( /*(x == 0 && y == SIZE) || (x == SIZE && y == 0) || */(x == 0 || y == 0) || (x == SIZE || y == SIZE))
                {
                    Debug.Log(tileMapPos.x + ", " + tileMapPos.y + ": " + x + ", " + y + ": " + ": " + (x + y * vertCount) + ": " + normals[x + y * vertCount]);
                    World.normalsstart.Add(new Vector3(tileMapPos.x, 0, tileMapPos.y) + newVertices[y + x * vertCount]);
                    World.normalsend.Add(new Vector3(tileMapPos.x, 0, tileMapPos.y) + newVertices[y + x * vertCount] + normals[y + x * vertCount]);

                }
            }
        }
        if (mesh == null)
        {
            generateMesh();
        }
        mesh.normals = normals;
        calculateTangents();
    }
    //Räknar vi ut i fel x led och y led?

    public void setNormals(Vector3[] normals)
    {
        int startIndex = tileMapPos.x * tileMapPos.y;
        Vector3[] newNormals = new Vector3[newVertices.Length];
        for (int i = 0; i < newVertices.Length; i++)
        {
            newNormals[i] = normals[i + startIndex]; 
        }

        if (mesh == null) generateMesh();
        mesh.normals = newNormals;
        calculateTangents();
        //World.RecalculateTangents(mesh);
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

	void calculateTangents()
	{
		Vector3[] normals = mesh.normals;
		int count = normals.Length;

		Vector4[] tangents = new Vector4[count];
		for(int i = 0; i < count; i++)
		{
			Vector3 n = normals[i];
            Vector3 t;

            Vector3 c1 = Vector3.Cross(n, new Vector3(0.0f, 0.0f, 1.0f));
            Vector3 c2 = Vector3.Cross(n, new Vector3(0.0f, 1.0f, 0.0f));

            if (Vector3.Magnitude(c1) > Vector3.Magnitude(c2))
            {
                t = c1;
            }
            else
            {
                t = c2;
            }

            Vector3.Normalize(t);

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
