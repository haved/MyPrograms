using System;
using System.Collections.Generic;
using Gdk;

namespace HDemografiSim
{
	public class HStandardChartLine : HChartLine
	{
		public class Point
		{
			public readonly float x;
			public float y;
			public Point(float x, float y)
			{
				this.x = x;
				this.y = y;
			}
		}

		readonly List<Point> points;

		public HStandardChartLine (String name, DColor color) : base(name, color)
		{
			points = new List<Point> ();
		}

		public void AddPoint(float x, float y)
		{
			for(int i = 0; i < points.Count; i++)
				if(x < points[i].x)
				{
					points.Insert(i, new Point(x, y));
					return;
				}
			points.Add(new Point(x, y));
		}

		public Point GetPoint(int index)
		{
			return points [index];
		}

		public float GetInterpolatedValue(float x)
		{
			if(points.Count <= 1){
				if (points.Count == 0)
					return 0;
				else if (points.Count == 1)
					return points [0].y;
			}

			if (x < points [0].x) //Index is smaller than the 0th element. We know we have 2.
				return Interpolate (points[0], points [1], x);
			
			for (int i = 1; ; i++) {
				if (x < points [i].x | i == points.Count-1) {
					return Interpolate (points[i-1], points [i], x);
				}
			}
		}

		public static float Interpolate(Point a, Point b, float x)
		{
			x = (x - a.x) / (b.x - a.x);
			return a.y * (1 - x) + b.y * x;
		}

		public override int GetPointCount()
		{
			return points.Count;
		}

		public override void GetPointPos(int index, out float x, out float y)
		{
			x = points [index].x;
			y = points [index].y;
		}

		public override float GetBiggestXValue()
		{
			return points.Count == 0 ? 0 : points [points.Count - 1].x;
		}

		public override float GetBiggestYValue()
		{
			float output = 0;
			foreach (Point p in points)
				output = Math.Max (p.y, output);
			return output;
		}
	}
}

