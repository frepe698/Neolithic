using UnityEngine;
using System.Collections;

public class Line {
	public Vector2 start;
	public Vector2 end;

	public Line(Vector2 start, Vector2 end)
	{
		this.start = start;
		this.end = end;

	}

	public Line(Vector2i start, Vector2i end)
	{
		this.start = new Vector2(start.x, start.y);
		this.end = new Vector2(end.x, end.y);
		
	}

	public static bool intersects(Line line1, Line line2)
	{

		float x1 = line1.start.x;
		float x2 = line1.end.x;
		float x3 = line2.start.x;
		float x4 = line2.end.x;
		float y1 = line1.start.y;
		float y2 = line1.end.y;
		float y3 = line2.start.y;
		float y4 = line2.end.y;

		float d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
		// If d is zero, there is no intersection
		if (d == 0) return false;
		
		// Get the x and y
		float pre = (x1*y2 - y1*x2), post = (x3*y4 - y3*x4);
		float x = ( pre * (x3 - x4) - (x1 - x2) * post ) / d;
		float y = ( pre * (y3 - y4) - (y1 - y2) * post ) / d;
		
		// Check if the x and y coordinates are within both lines
		if ( x < Mathf.Min(x1, x2) || x > Mathf.Max(x1, x2) ||
		    x < Mathf.Min(x3, x4) || x > Mathf.Max(x3, x4) ) return false;
		if ( y < Mathf.Min(y1, y2) || y > Mathf.Max(y1, y2) ||
		    y < Mathf.Min(y3, y4) || y > Mathf.Max(y3, y4) ) return false;
		
		// Return the point of intersection
		return true;
	}

    

    public static bool intersectsSquare(Line line, Vector2 c1, Vector2 c2)
    {
        //Calc min and max values for square
        float xmin = Mathf.Min(c1.x, c2.x);
        float xmax = Mathf.Max(c1.x, c2.x);

        float ymin = Mathf.Min(c1.y, c2.y);
        float ymax = Mathf.Max(c1.y, c2.y);

        //Line left of square
        if (line.start.x < xmin && line.end.x < xmin) return false;
        //Line right of square
        if (line.start.x > xmax && line.end.x > xmax) return false;
        //Line below square
        if (line.start.y < ymin && line.end.y < ymin) return false;
        //Line above square
        if (line.start.y > ymax && line.end.y > ymax) return false;
        //Line inside square
        if (line.start.x > xmin && line.end.x > xmin && line.start.x < xmax && line.end.x < xmax &&
            line.start.y > ymin && line.end.y > ymin && line.start.y < ymax && line.end.y < ymax) return false;

        Line right = new Line(new Vector2(xmax, ymax), new Vector2(xmax, ymin));
        Line left = new Line(new Vector2(xmin, ymax), new Vector2(xmin, ymin));

        Line top = new Line(new Vector2(xmax, ymax), new Vector2(xmin, ymax));
        Line bottom = new Line(new Vector2(xmin, ymin), new Vector2(xmax, ymin));

        return (Line.intersects(line, right)
            || Line.intersects(line, left)
            || !Line.intersects(line, top)
            || !Line.intersects(line, bottom));
    }

    public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2,
    Vector2 pe2)
    {
        Debug.DrawLine(new Vector3(ps1.x, 3, ps1.y), new Vector3(pe1.x, 3, pe1.y), Color.blue, 1);
        Debug.DrawLine(new Vector3(ps2.x, 3, ps2.y), new Vector3(pe2.x, 3, pe2.y), Color.red, 1);
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
        {
            Debug.Log("Line 1: " + ps1.x + ", " + ps1.y + ";" + pe1.x + ", " + pe1.y);
            Debug.Log("Line 2: " + ps2.x + ", " + ps2.y + ";" + pe2.x + ", " + pe2.y);
            throw new System.Exception("Lines are parallel");

        }

        // now return the Vector2 intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }

    public static Vector2 LineIntersectionPoint(Line line1, Line line2)
    {
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = line1.end.y - line1.start.y;
        float B1 = line1.start.x - line1.end.x;
        float C1 = A1 * line1.start.x + B1 * line1.start.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = line2.end.y - line2.start.y;
        float B2 = line2.start.x - line2.end.x;
        float C2 = A2 * line2.start.x + B2 * line2.start.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            throw new System.Exception("Lines are parallel");

        // now return the Vector2 intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }

	public static bool operator==(Line line1, Line line2)
	{
		return line1.minX() == line2.minX() && line1.maxX() == line2.maxX () &&
			line1.minY() == line2.minY() && line1.maxY() == line2.maxY ();
	}

	public static bool operator!=(Line line1, Line line2)
	{
		return !(line1.minX() == line2.minX() && line1.maxX() == line2.maxX () &&
			line1.minY() == line2.minY() && line1.maxY() == line2.maxY ());
	}

	public override bool Equals(object v)
	{
		if(v is Line)
		{
			return this == (Line)v;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return 0;
	}

	public int minX()
	{
		if(start.x < end.x)
		{
			return (int)start.x;
		}
		return (int)end.x;
	}

	public int maxX()
	{
		if(start.x > end.x)
		{
			return (int)start.x;
		}
		return (int)end.x;
	}

	public int minY()
	{
		if(start.y < end.y)
		{
			return (int)start.y;
		}
		return (int)end.y;
	}
	
	public int maxY()
	{
		if(start.y > end.y)
		{
			return (int)start.y;
		}
		return (int)end.y;
	}

	public Vector2 minXv()
	{
		if(start.x < end.x)
		{
			return start;
		}
		return end;
	}

	public Vector2 maxXv()
	{
		if(start.x > end.x)
		{
			return start;
		}
		return end;
	}
	public Vector2 minYv()
	{
		if(start.y < end.y)
		{
			return start;
		}
		return end;
	}
	public Vector2 maxYv()
	{
		if(start.y > end.y)
		{
			return start;
		}
		return end;
	}

	public Vector2 min()
	{
		return new Vector2(minX(), minY());
	}

	public Vector2 max()
	{
		return new Vector2(maxX(), maxY());
	}

	public void draw(Color color)
	{
		Debug.DrawLine(new Vector3(start.x, 2, start.y), new Vector3(end.x, 2, end.y), color);
	}

	public float distanceFromPoint(Vector2 p) {
		// Return minimum distance between line segment vw and point p
		float l2 = lengthSquared(start, end);  // i.e. |w-v|^2 -  avoid a sqrt
		if (l2 == 0.0) return Vector2.Distance(p, start);   // v == w case
		// Consider the line extending the segment, parameterized as v + t (w - v).
		// We find projection of point p onto the line. 
		// It falls where t = [(p-v) . (w-v)] / |w-v|^2
		float t = Vector2.Dot(p - start, end - start) / l2;
		if (t < 0.0) return Vector2.Distance(p, start);       // Beyond the 'v' end of the segment
		else if (t > 1.0) return Vector2.Distance(p, end);  // Beyond the 'w' end of the segment
		Vector2 projection = start + t * (end - start);  // Projection falls on the segment
		return Vector2.Distance(p, projection);
	}

	private float lengthSquared(Vector2 start, Vector2 end)
	{
		return (start.x - end.x)*(start.x - end.x) + (start.y - end.y)*(start.y - end.y);
	}
}
