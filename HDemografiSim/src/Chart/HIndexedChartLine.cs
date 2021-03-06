﻿using System;
using System.Collections.Generic;

namespace HDemografiSim
{
	public class HIndexedChartLine : HChartLine
	{
		readonly List<Single> values;
		float biggestYValue;

		public HIndexedChartLine (String name, DColor color) : base(name, color)
		{
			values = new List<Single> ();
		}

		public HIndexedChartLine(HIndexedChartLine copy, String name, DColor color) : base(name, color)
		{
			values = new List<Single> (copy.values);
			biggestYValue = copy.biggestYValue;
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

		public void InsertPoint(int index, float value)
		{
			if (value > biggestYValue)
				biggestYValue = value;
			values.Insert (index, value);
		}

		public void RemovePoint(int index)
		{
			if (values [index] >= biggestYValue)
				UpdateBiggestYValue ();
			values.RemoveAt (index);
		}

		public void RemovePointsBefore(int index)
		{
			for (int i = 0; i < index; i++)
				values.RemoveAt (i);
			UpdateBiggestYValue ();
		}

		public void RemovePointAndAfter(int index)
		{
			for (int i = values.Count - 1; i >= index; i--)
				values.RemoveAt (i);
			UpdateBiggestYValue ();
		}

		/**<summary> Removes all the points at the end with a value of less than 1. Updates the biggest y value afterwards. </summary>*/
		public void RemoveEmptyEnd()
		{
			while (values.Count > 0 && values [values.Count - 1] <= 1) {
				values.RemoveAt (values.Count - 1);
			}
			UpdateBiggestYValue ();
			/*if(values[values.Count-1] <= 0)
			{
				values.Remove (values.Count - 1);
				RemoveEmptyEnd (); //Recursive!
			}*/
		}

		public void GhostChangePoint(int index, float y)
		{
			values [index] = y;
		}

		public void ChangePoint(int index, float y)
		{
			values [index] = y;
			UpdateBiggestYValue ();
		}

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

		public float GetValue(int index)
		{
			return values [index];
		}

		public float GetValueOfLastPoint()
		{
			return values [values.Count - 1];
		}

		public float GetSumOfValues()
		{
			float output = 0;
			foreach (float f in values)
				output += f;

			return output;
		}

		public float GetSumOfValuesBetween(int min, int max)
		{
			float output = 0;
			for (int i = min; i <= max & i<values.Count; i++)
				output += values [i];
			return output;
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

