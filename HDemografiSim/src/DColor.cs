using System;

namespace HDemografiSim
{
	public struct DColor
	{
		public float r, g, b;
		public DColor(float r, float g, float b)
		{
			this.r = Math.Max(Math.Min(r, 1), 0);
			this.g = Math.Max(Math.Min(g, 1), 0);
			this.b = Math.Max(Math.Min(b, 1), 0);
		}

		public DColor(byte r, byte g, byte b) : this(r/255f, g/255f, b/255f) {}
	}
}

