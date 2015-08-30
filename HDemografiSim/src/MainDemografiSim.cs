using System;
using Gtk;

namespace HDemografiSim
{
	class MainDemograftSim
	{
		public static MainDemograftSim Instance;

		Window window;

		HIndexedChartLine populationLine;
		HChart populationChart;

		HIndexedChartLine birthRate;
		HIndexedChartLine deathRate;
		HChart rateChart;

		HIndexedChartLine ageDistribution;
		HChart ageDistributionChart;

		HStandardChartLine chanceOfDeath;
		HChart chanceOfDeathChart;



		public MainDemograftSim()
		{
			window = new Window ("HDemografiSim");
			window.DeleteEvent += (o, eargs) => {
				Application.Quit();
				eargs.RetVal = true;
			};
			window.ModifyBg (StateType.Normal, new Gdk.Color (130, 130, 160));

			populationLine = new HIndexedChartLine ("Population", new DColor(200, 140, 255));
			populationLine.AddPoints (3000, 3200, 3300, 3500, 3200, 3100, 3400, 3600, 4000);

			populationChart = new HChart ("Population chart", populationLine);

			birthRate = new HIndexedChartLine ("Birth rate", new DColor(100, 140, 255));
			birthRate.AddPoints (1, 1.3f, 1.4f, 1.2f, 1.35f, 2f, 2.6f, 2.7f, 3.2f, 4, 3.6f);
			deathRate = new HIndexedChartLine ("Death rate", new DColor(55, 80, 180));
			deathRate.AddPoints (0.5f, 1.2f, 1.4f, 1.6f, 1.3f, 1.6f, 2f, 2f, 2.5f, 4, 3);
			rateChart = new HChart ("Rates Chart", birthRate, deathRate);

			var populationRatesBox = new VBox (true, 10);
			populationRatesBox.Add (populationChart);
			populationRatesBox.Add (rateChart);


			ageDistribution = new HIndexedChartLine ("Age Distribution", new DColor(250, 100, 255));
			populationLine.AddPoints (50, 52, 65, 67, 70, 80, 75, 82, 90, 98, 100, 93, 85, 40, 12, 5, 2, 1);

			ageDistributionChart = new HChart ("Age chart", ageDistribution);

			chanceOfDeath = new HStandardChartLine ("Chance of death", new DColor(255, 80, 180));
			deathRate.AddPoints (0.5f, 1.2f, 1.4f, 1.6f, 1.3f, 1.6f, 2f, 2f, 2.5f, 4, 3);
			chanceOfDeathChart = new HChart ("Death Chart", chanceOfDeath);

			var ageDeathBox = new VBox (true, 10);
			ageDeathBox.Add (ageDistributionChart);
			ageDeathBox.Add (chanceOfDeathChart);

			var allChartsBox = new HBox (true, 10);
			allChartsBox.Add (ageDeathBox);
			allChartsBox.Add (populationRatesBox);

			window.Add (allChartsBox);
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
