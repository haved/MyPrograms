using System;

namespace HDemografiSim
{
	public struct DColor
	{
		public float r, g, b;
		public DColor(float r, float g, float b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}

		public DColor(byte r, byte g, byte b)
		{
			this.r = r/255f;
			this.g = g/255f;
			this.b = b/255f;
		}
	}
}

