using System;
using Gdk;

namespace HDemografiSim
{
	public abstract class HChartLine
	{
		protected DColor FillColor;
		protected DColor Color;
		protected String Name;

		protected HChartLine (String name, DColor color)
		{
			Name = name;
			Color = color;
			FillColor = new DColor(color.R*1.3f, color.G*1.3f, color.B*1.3f);
		}

		public abstract int GetPointCount();

		public abstract void GetPointPos(int index, out float x, out float y);

		public abstract float GetBiggestXValue();

		public abstract float GetBiggestYValue();

		public DColor GetColor()
		{
			return Color;
		}

		public void UseColor(Cairo.Context g)
		{
			g.SetSourceRGBA (Color.R, Color.G, Color.B, Color.A);
		}

		public DColor GetFillColor()
		{
			return FillColor;
		}

		public String GetName()
		{
			return Name;
		}
	}
}

