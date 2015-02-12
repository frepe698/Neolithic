using UnityEngine;
using System.Collections;

public class Cave  {

	private readonly int MARGIN = 10;
	private int size;
	private Vector2 position;
	private Tile[,] tiles;

	private Vector2i entrancePosition;
	private Vector2i bossPosition;

	public Cave(int size, Vector2 position)
	{
		this.size = size;
		this.position = position;
	}

	private void generateCave()
	{
		//Randomizing entrance and boss spawn
		int ex = Random.Range(MARGIN, size - MARGIN); //somewhere along the x axis
		int ey = MARGIN + (size - 20)* Random.Range(0,2); //either side of the cave
		entrancePosition = new Vector2i(ex, ey);

		int bx = Random.Range(MARGIN, size - MARGIN); //somewhere along the x axis
		int by = size - ey; //other side of the cave
		bossPosition = new Vector2i(bx, by);


	}
}
