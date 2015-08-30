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

		HIndexedChartLine birthRate;
		HChart rateChart;

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

			birthRate = new HIndexedChartLine ("Birth rate", new DColor(200, 140, 255));
			birthRate.AddPoints (1, 1.3f, 1.4f, 1.2f, 1.35f, 2f, 2.6f, 2.7f, 3.2f, 4, 3.6f);

			rateChart = new HChart ("Rates Chart", birthRate);

			var box = new VBox (true, 10);
			box.Add (populationChart);
			box.Add (rateChart);

			window.Add (box);
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
