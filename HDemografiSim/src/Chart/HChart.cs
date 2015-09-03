using System;
using System.Collections;
using System.Collections.Generic;
using Gtk;
using Cairo;

namespace HDemografiSim
{
	[System.ComponentModel.ToolboxItem (true)]
	public class HChart : DrawingArea
	{
		const float numberSizeX = 60;
		const float numberSizeY = 30;

		protected String Name;
		protected List<HChartLine> Lines;

		protected float XSize, YSize;
		protected float NumbersX, NumbersY;

		protected float NumberSpacingXLeft = 30;
		protected float NumberSpacingXRight = 12;
		float numberSpacingXTotal;
		protected float NumberSpacingYUp = 12;
		protected float NumberSpacingYDown = 17;
		float numberSpacingYTotal;

		public HChart (String name, params HChartLine[] lines)
		{
			this.Name = name;
			this.Lines = new List<HChartLine>(lines);

			ModifyBg (StateType.Normal, new Gdk.Color (230, 230, 230));
		}

		protected void UpdateLineSpacingSums()
		{
			numberSpacingXTotal = NumberSpacingXLeft + NumberSpacingXRight;
			numberSpacingYTotal = NumberSpacingYUp + NumberSpacingYDown;
		}

		public int GetLineCount()
		{
			return Lines.Count;
		}

		public HChartLine GetLineAt(int index)
		{
			return Lines [index];
		}

		public void AddLine(HChartLine line)
		{
			Lines.Add (line);
		}

		public void RemoveLineAt(int index)
		{
			Lines.RemoveAt (index);
		}

		public virtual void ResetScale()
		{
			XSize = 0;
			YSize = 0;
			UpdateScale ();
		}

		public virtual void UpdateScale()
		{
			//xSize = 0;
			//ySize = 0;
			foreach (HChartLine line in Lines) {
				XSize = Math.Max (line.GetBiggestXValue (), XSize);
				YSize = Math.Max (line.GetBiggestYValue (), YSize);
			}
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			
			// Insert button press handling code here.
			return base.OnButtonPressEvent (ev);
		}

		const float referenceLineColor = 0.7f;
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			base.OnExposeEvent (evnt);

			using (var g = Gdk.CairoHelper.Create (evnt.Window)) {
				UseStandardLineDrawing (g);
				DrawVerticalReferenceLines (g, evnt.Area, XSize);
				DrawHorizontalReferenceLines (g, evnt.Area, YSize);

				DrawTitle (g, Name);
				for (int i = 0; i < Lines.Count; i++) {
					DrawLineInfo (g, Lines [i], i);
				}

				TransformForLineDrawing (g, evnt.Area);
				DrawLines (g, evnt.Area, Lines, XSize, YSize);
			}

			return true;
		}

		protected void UseStandardLineDrawing(Context g)
		{
			g.Antialias = Antialias.Subpixel;
			g.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Normal); //Draw backround reference lines.
			g.SetFontSize (12);
			g.SetSourceRGB (referenceLineColor, referenceLineColor, referenceLineColor);
			g.LineWidth = 2;
		}

		protected void DrawHorizontalReferenceLines(Context g, Gdk.Rectangle area, float ySize)
		{
			for (int i = 0; i <= NumbersY; i++) {
				float actualLoc = (1 - i/NumbersY) * (area.Height - numberSpacingYTotal) + NumberSpacingYUp;
				String text = (i*ySize/NumbersY).ToString ("0.0");
				TextExtents size = g.TextExtents (text);
				g.MoveTo (Math.Max(NumberSpacingXLeft-size.Width, 0), actualLoc + size.Height / 2);
				g.ShowText (text);
				g.MoveTo (NumberSpacingXLeft, actualLoc);
				g.LineTo (area.Width-NumberSpacingXRight, actualLoc);
				g.Stroke ();
			}
		}

		protected void DrawRightsideHorisontalNumbers(Context g, Gdk.Rectangle area, float ySize)
		{
			for (int i = 0; i <= NumbersY; i++) {
				float actualLoc = (1 - i/NumbersY) * (area.Height - numberSpacingYTotal) + NumberSpacingYUp;
				String text = (i*ySize/NumbersY).ToString ("0.0");
				TextExtents size = g.TextExtents (text);
				g.MoveTo (area.Width-NumberSpacingXRight, actualLoc + size.Height / 2);
				g.ShowText (text);
			}
		}

		protected void DrawVerticalReferenceLines(Context g, Gdk.Rectangle area, float xSize)
		{
			for (int i = 0; i <= NumbersX; i++) {
				float actualLoc = i * (area.Width - numberSpacingXTotal) / NumbersX + NumberSpacingXLeft;
				String text = (i*xSize/NumbersX).ToString ("0.0");
				g.MoveTo (actualLoc - g.TextExtents (text).Width / 2, area.Height - 5);
				g.ShowText (text);
				g.MoveTo (actualLoc, area.Height - NumberSpacingYDown);
				g.LineTo (actualLoc, NumberSpacingYUp);
				g.Stroke ();
			}
		}

		protected void DrawTitle(Context g, String name)
		{
			g.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Bold); //Draw the title and line names/colors.
			g.SetFontSize (14);
			g.SetSourceRGBA (0, 0, 0, 0.5f);
			g.MoveTo (NumberSpacingXLeft+15, NumberSpacingYUp+7);
			g.ShowText (name);
		}

		protected void DrawLineInfo(Context g, HChartLine line, int index)
		{
			g.MoveTo (NumberSpacingXLeft+15, NumberSpacingYUp+15 + index * 15);
			g.LineTo (NumberSpacingXLeft+25, NumberSpacingYUp+15 + index * 15);
			line.UseColor (g);
			g.LineWidth = 11;
			g.Stroke ();
			g.MoveTo (NumberSpacingXLeft+26, NumberSpacingYUp+20 + index * 15);
			g.SetSourceRGBA (0, 0, 0, 0.5f);
			g.ShowText (line.GetName ());
		}

		protected void TransformForLineDrawing(Context g, Gdk.Rectangle area)
		{
			g.Scale (1, -1); //Draw the lines themself.
			g.Translate (NumberSpacingXLeft, -area.Height + NumberSpacingYDown);

			g.Antialias = Antialias.Subpixel;
			g.LineWidth = 4;
		}

		protected void DrawLines(Context g, Gdk.Rectangle area, IEnumerable lines, float xSize, float ySize)
		{
			float xScale = (area.Width-numberSpacingXTotal) / xSize;
			float yScale = (area.Height-numberSpacingYTotal) / ySize;

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

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			UpdateScale ();
			NumbersX = (int)((allocation.Width-numberSpacingXTotal) / numberSizeX)+1;
			NumbersY = (int)((allocation.Height-numberSpacingYTotal) / numberSizeY)+1;
		}

		protected override void OnSizeRequested (ref Requisition requisition)
		{
			//base.OnSizeRequested(ref requisition);
			// Calculate desired size here.
			requisition.Width = 400;
			requisition.Height = 240;
			UpdateLineSpacingSums ();
		}
	}
}

