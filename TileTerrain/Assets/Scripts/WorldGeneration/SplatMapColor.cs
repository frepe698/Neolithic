using UnityEngine;
using System.Collections;

public class SplatMapColor {

	public static readonly int colorCount = 2;
	public Color[] colors;

	public SplatMapColor()
	{
		colors = new Color[]
		{
			new Color(0, 0, 0, 0),
			new Color(0, 0, 0, 0),
		};
	}

	public SplatMapColor(Color color1, Color color2)
	{
		this.colors = new Color[]
		{
			color1, color2
		};
	}

	public static SplatMapColor operator+ (SplatMapColor col1, SplatMapColor col2)
	{
//		for(int i = 0; i < colorCount; i++)
//		{
//			col1.colors[i] += col2.colors[i];
//		}
		return new SplatMapColor(col1.colors[0] + col2.colors[0], col1.colors[1] + col2.colors[1]);
	}

	public static SplatMapColor operator- (SplatMapColor col1, SplatMapColor col2)
	{
		for(int i = 0; i < colorCount; i++)
		{
			col1.colors[i] -= col2.colors[i];
		}
		return col1;
	}

	public static SplatMapColor operator/ (SplatMapColor col1, float div)
	{
		SplatMapColor result = new SplatMapColor();
		for(int i = 0; i < colorCount; i++)
		{
			result.colors[i] = col1.colors[i] / div;
		}
		return result;
	}

	public static SplatMapColor operator* (SplatMapColor col1, float div)
	{
		for(int i = 0; i < colorCount; i++)
		{
			col1.colors[i] *= div;
		}
		return col1;
	}
}
