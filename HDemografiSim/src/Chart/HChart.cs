using System;
using Gtk;
using Cairo;

namespace HDemografiSim
{
	[System.ComponentModel.ToolboxItem (true)]
	public class HChart : DrawingArea
	{
		String name;
		HChartLine[] lines;

		float xSize, ySize;

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
			xSize *= 1.02f;
			ySize *= 1.15f;
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			
			// Insert button press handling code here.
			return base.OnButtonPressEvent (ev);
		}

		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			base.OnExposeEvent (evnt);

			using (var g = Gdk.CairoHelper.Create (evnt.Window)) {
				g.Antialias = Antialias.Subpixel;
				g.LineWidth = 9;

				g.SelectFontFace("Dialog", FontSlant.Normal, FontWeight.Bold);
				g.SetFontSize (14);
				g.MoveTo (10, 20);
				g.ShowText (name);
				g.SelectFontFace("Dialog", FontSlant.Normal, FontWeight.Normal);
				for (int i = 0; i < lines.Length; i++) {
					g.MoveTo (10, 30 + i * 15);
					g.LineTo (20, 30 + i * 15);
					lines [i].UseColor (g);
					g.Stroke ();
					g.MoveTo (21, 35 + i * 15);
					g.SetSourceRGB (0, 0, 0);
					g.ShowText (lines[i].GetName());
				}

				g.Save ();

				g.Scale (1, -1);
				g.Translate (0, -evnt.Area.Height);

				float xScale = evnt.Area.Width / xSize;
				float yScale = evnt.Area.Height / ySize;

				g.Antialias = Antialias.Subpixel;
				g.LineWidth = 4;

				foreach (HChartLine line in lines) {
					if (line.GetPointCount () < 1)
						continue;

					float newX, newY;
					line.GetPointPos (0, out newX, out newY);

					//g.MoveTo (newX*xScale, 0);
					g.MoveTo (newX*xScale, newY*yScale);

					for (int j = 0; j < line.GetPointCount () - 1; j++) {
						line.GetPointPos (j + 1, out newX, out newY);
						g.LineTo (newX*xScale, newY*yScale);
					}

					line.UseColor (g);
					g.Stroke ();
				}

				g.Restore ();
			}

			return true;
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			UpdateScale ();
		}

		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			//base.OnSizeRequested(ref requisition);
			// Calculate desired size here.
			requisition.Width = 400;
			requisition.Height = 240;
		}
	}
}

