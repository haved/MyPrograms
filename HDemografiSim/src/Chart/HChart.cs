using System;
using Gtk;

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

			ModifyFg (StateType.Normal, new Gdk.Color (100, 255, 140));
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

			Gdk.GC normal = Style.BaseGC (StateType.Normal);

			GdkWindow.DrawLine(normal, 0, 0, 400, 200);

			foreach (HChartLine line in lines)
			{
				if (line.GetPointCount () < 1)
					continue;
				float newX=0, newY=0;
				line.GetPointPos (0, out newX, out newY);
				for (int j = 0; j < line.GetPointCount ()-1; j++) {
					float cX = newX, cY = newY;
					line.GetPointPos (j+1, out newX, out newY);
					GdkWindow.DrawLine (normal, (int)(cX / xSize * evnt.Area.Width), (int)(1-(cY / ySize) * evnt.Area.Height), 
						(int)(newX / xSize * evnt.Area.Width), (int)(1-(newY / ySize) * evnt.Area.Height));
					GdkWindow.DrawRectangle(normal, true, new Gdk.Rectangle(
						(int)(cX / xSize * evnt.Area.Width)-4, (int)(1-(cY / ySize) * evnt.Area.Height)-4, 8, 8));
				}
				GdkWindow.DrawRectangle(normal, true, new Gdk.Rectangle(
					(int)(newX / xSize * evnt.Area.Width)-4, (int)(1-(newY / ySize) * evnt.Area.Height)-4, 8, 8));
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

