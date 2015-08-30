using System;
using Gdk;

namespace HDemografiSim
{
	public abstract class HChartLine
	{
		protected DColor color;
		protected String name;

		protected HChartLine (String name, DColor color)
		{
			this.name = name;
			this.color = color;
		}

		public abstract int GetPointCount();

		public abstract void GetPointPos(int index, out float x, out float y);

		public abstract float GetBiggestXValue();

		public abstract float GetBiggestYValue();

		public DColor GetColor()
		{
			return color;
		}

		public String GetName()
		{
			return name;
		}
	}
}

