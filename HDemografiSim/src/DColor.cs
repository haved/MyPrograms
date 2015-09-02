using System;

namespace HDemografiSim
{
	public struct DColor
	{
		public float R, G, B, A;
		public DColor(float r, float g, float b) : this(r, g, b, 1) {}

		public DColor(float r, float g, float b, float a)
		{
			this.R = Math.Max(Math.Min(r, 1), 0);
			this.G = Math.Max(Math.Min(g, 1), 0);
			this.B = Math.Max(Math.Min(b, 1), 0);
			this.A = Math.Max(Math.Min(a, 1), 0);
		}

		public DColor(byte r, byte g, byte b) : this(r/255f, g/255f, b/255f) {}

		public DColor HalfTransparent()
		{
			return new DColor (R, G, B, A*0.5f);
		}
	}
}

