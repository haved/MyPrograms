using System;
using System.Collections.Generic;

namespace HDemografiSim
{
	public class HDoubleChart : HChart
	{
		readonly List<HChartLine> secondaryLines;

		float SecondaryYSize;

		public HDoubleChart (String name, HChartLine[] secondaryLines, params HChartLine[] primaryLines) : base(name, primaryLines)
		{
			this.secondaryLines = new List<HChartLine> (secondaryLines);
			NumberSpacingXRight = 40;
		}

		public override void ResetScale ()
		{
			SecondaryYSize = 0;
			base.ResetScale ();
		}

		public override void UpdateScale ()
		{
			base.UpdateScale ();
			foreach (HChartLine line in secondaryLines) {
				XSize = Math.Max (XSize, line.GetBiggestXValue());
				SecondaryYSize = Math.Max (SecondaryYSize, line.GetBiggestYValue());
			}
		}
	}
}

