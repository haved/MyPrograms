using System;
using System.Collections.Generic;

namespace HDemografiSim
{
	public class HIndexedChartLine : HChartLine
	{
		readonly List<Single> values;

		public HIndexedChartLine (String name, DColor color) : base(name, color)
		{
			values = new List<Single> ();
		}

		public void AddPoint(float y)
		{
			values.Add (y);
			if (y > biggestYValue)
				biggestYValue = y;
		}

		public void AddPoints(params float[] y)
		{
			foreach (float f in y)
				AddPoint (f);
		}

		public void RemovePoint(int index)
		{
			if (values [index] >= biggestYValue)
				UpdateBiggestYValue ();
			values.RemoveAt (index);
		}

		public void RemovePointAndAfter(int index)
		{
			for (int i = values.Count - 1; i >= index; i--)
				values.RemoveAt (i);
			UpdateBiggestYValue ();
		}

		public void ChangePoint(int index, float y)
		{
			values [index] = y;
			UpdateBiggestYValue ();
		}

		float biggestYValue;
		public void UpdateBiggestYValue()
		{
			biggestYValue = 0;
			foreach (float f in values)
				biggestYValue = Math.Max (biggestYValue, f);
		}

		public override int GetPointCount ()
		{
			return values.Count;
		}

		public override void GetPointPos (int index, out float x, out float y)
		{
			x = index;
			y = values [index];
		}

		public override float GetBiggestXValue ()
		{
			return values.Count - 1;
		}

		public override float GetBiggestYValue ()
		{
			return biggestYValue;
		}
	}
}

