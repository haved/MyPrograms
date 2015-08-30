using System;
using Gdk;

namespace HDemografiSim
{
	public abstract class HChartLine
	{
		protected DColor fillColor;
		protected DColor color;
		protected String name;

		protected HChartLine (String name, DColor color)
		{
			this.name = name;
			this.color = color;
			fillColor = new DColor(color.r*1.3f, color.g*1.3f, color.b*1.3f);
		}

		public abstract int GetPointCount();

		public abstract void GetPointPos(int index, out float x, out float y);

		public abstract float GetBiggestXValue();

		public abstract float GetBiggestYValue();

		public DColor GetColor()
		{
			return color;
		}

		public DColor GetFillColor()
		{
			return fillColor;
		}

		public String GetName()
		{
			return name;
		}
	}
}

