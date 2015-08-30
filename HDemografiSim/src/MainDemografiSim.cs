using System;
using Gtk;

namespace HDemografiSim
{
	class MainDemograftSim
	{
		public static MainDemograftSim Instance;

		Window window;

		HStandardChartLine populationLine;
		HChart populationChart;

		public MainDemograftSim()
		{
			window = new Window ("HDemografiSim");
			window.DeleteEvent += (o, eargs) => {
				Application.Quit();
				eargs.RetVal = true;
			};
			window.ModifyBg (StateType.Normal, new Gdk.Color (100, 255, 140));

			populationLine = new HStandardChartLine ("Population", new DColor(100, 140, 255));
			populationLine.AddPoint (0, 0);
			populationLine.AddPoint (1, 2.2f);
			populationLine.AddPoint (3, 3);
			populationLine.AddPoint (5, 6);

			populationChart = new HChart ("Population chart", populationLine);

			window.Add (populationChart);
			window.ShowAll ();
		}

		public static void Main (string[] args)
		{
			Application.Init ();
			Instance = new MainDemograftSim ();
			Application.Run ();
		}
	}
}
