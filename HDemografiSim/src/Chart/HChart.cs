using System;
using Gtk;
using Cairo;

namespace HDemografiSim
{
	[System.ComponentModel.ToolboxItem (true)]
	public class HChart : DrawingArea
	{
		const float numberSizeX = 60;
		const float numberSizeY = 30;

		String name;
		HChartLine[] lines;

		float xSize, ySize;
		float numbersX, numbersY;

		public HChart (String name, params HChartLine[] lines)
		{
			this.name = name;
			this.lines = lines;

			ModifyBg (StateType.Normal, new Gdk.Color (230, 230, 230));
		}

		public void UpdateScale()
		{
			xSize = 0;
			ySize = 0;
			foreach (HChartLine line in lines) {
				xSize = Math.Max (line.GetBiggestXValue (), xSize);
				ySize = Math.Max (line.GetBiggestYValue (), ySize);
			}
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			
			// Insert button press handling code here.
			return base.OnButtonPressEvent (ev);
		}

		const float referenceLineColor = 0.7f;
		const float numberSpacingXLeft = 20;
		const float numberSpacingXRight = 12;
		const float numberSpacingXTotal = numberSpacingXLeft + numberSpacingXRight;
		const float numberSpacingYUp = 12;
		const float numberSpacingYDown = 17;
		const float numberSpacingYTotal = numberSpacingYUp + numberSpacingYDown;
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			base.OnExposeEvent (evnt);

			using (var g = Gdk.CairoHelper.Create (evnt.Window)) {
				g.Antialias = Antialias.Subpixel; {
					g.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Normal); //Draw backround reference lines.
					g.SetFontSize (12);
					g.SetSourceRGB (referenceLineColor, referenceLineColor, referenceLineColor);
					g.LineWidth = 2;

					for (int i = 0; i <= numbersX; i++) {
						float actualLoc = i * (evnt.Area.Width - numberSpacingXTotal) / numbersX + numberSpacingXLeft;
						String text = (i*xSize/numbersX).ToString ("0.0");
						g.MoveTo (actualLoc - g.TextExtents (text).Width / 2, evnt.Area.Height - 5);
						g.ShowText (text);
						g.MoveTo (actualLoc, evnt.Area.Height - numberSpacingYDown);
						g.LineTo (actualLoc, 0);
						g.Stroke ();
					}

					for (int i = 0; i <= numbersY; i++) {
						float actualLoc = (1 - i/numbersY) * (evnt.Area.Height - numberSpacingYTotal) + numberSpacingYUp;
						String text = (i*ySize/numbersY).ToString ("0.0");
						g.MoveTo (5, actualLoc + g.TextExtents (text).Height / 2);
						g.ShowText (text);
						g.MoveTo (numberSpacingXLeft, actualLoc);
						g.LineTo (evnt.Area.Width, actualLoc);
						g.Stroke ();
					}
				} {
					g.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Bold); //Draw the title and line names/colors.
					g.SetFontSize (14);
					g.SetSourceRGB (0, 0, 0);
					g.MoveTo (10, 20);
					g.ShowText (name);
					g.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Normal);
					g.LineWidth = 11;
					for (int i = 0; i < lines.Length; i++) {
						g.MoveTo (10, 30 + i * 15);
						g.LineTo (20, 30 + i * 15);
						lines [i].UseColor (g);
						g.Stroke ();
						g.MoveTo (21, 35 + i * 15);
						g.SetSourceRGB (0, 0, 0);
						g.ShowText (lines [i].GetName ());
					}
				}
				g.Save (); {
					g.Scale (1, -1); //Draw the lines themself.
					g.Translate (numberSpacingXLeft, -evnt.Area.Height + numberSpacingYDown);

					float xScale = (evnt.Area.Width-numberSpacingXTotal) / xSize;
					float yScale = (evnt.Area.Height-numberSpacingYTotal) / ySize;

					g.Antialias = Antialias.Subpixel;
					g.LineWidth = 4;

					foreach (HChartLine line in lines) {
						if (line.GetPointCount () < 1)
							continue;

						float newX, newY;
						line.GetPointPos (0, out newX, out newY);

						//g.MoveTo (newX*xScale, 0);
						g.MoveTo (newX * xScale, newY * yScale);

						for (int j = 0; j < line.GetPointCount () - 1; j++) {
							line.GetPointPos (j + 1, out newX, out newY);
							g.LineTo (newX * xScale, newY * yScale);
						}

						line.UseColor (g);
						g.Stroke ();
					}
				}
				g.Restore ();
			}

			return true;
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			UpdateScale ();
			numbersX = (int)((allocation.Width-numberSpacingXTotal) / numberSizeX)+1;
			numbersY = (int)((allocation.Height-numberSpacingYTotal) / numberSizeY)+1;
		}

		protected override void OnSizeRequested (ref Requisition requisition)
		{
			//base.OnSizeRequested(ref requisition);
			// Calculate desired size here.
			requisition.Width = 400;
			requisition.Height = 240;
		}
	}
}

