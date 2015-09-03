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

		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			base.OnExposeEvent (evnt);

			using (var g = Gdk.CairoHelper.Create (evnt.Window)) {
				UseStandardLineDrawing (g);
				DrawVerticalReferenceLines (g, evnt.Area, XSize);
				DrawHorizontalReferenceLines (g, evnt.Area, YSize);
				DrawRightsideHorisontalNumbers (g, evnt.Area, SecondaryYSize);

				DrawTitle (g, Name);
				int i;
				for (i = 0; i < Lines.Count; i++) {
					DrawLineInfo (g, Lines [i], i);
				}
				for (int j = 0; j < secondaryLines.Count; j++) {
					DrawLineInfo (g, secondaryLines [j], i);
					i++;
				}

				TransformForLineDrawing (g, evnt.Area);
				DrawLines (g, evnt.Area, Lines, XSize, YSize);
				DrawLines (g, evnt.Area, secondaryLines, XSize, SecondaryYSize);
			}

			return true;
		}
	}
}

