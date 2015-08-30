﻿using System;
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

		SpinButton fertilitySpinner;

		public MainDemograftSim()
		{
			window = new Window ("HDemografiSim");
			window.DeleteEvent += (o, eargs) => {
				Application.Quit();
				eargs.RetVal = true;
			};
			window.ModifyBg (StateType.Normal, new Gdk.Color (130, 130, 160));

			ageDistribution = new HIndexedChartLine ("Age Distribution", new DColor(250, 100, 255));
			ageDistribution.AddPoints (50, 52, 65, 67, 70, 80, 75, 82, 90, 98, 100, 93, 85, 40, 12, 5, 2, 1);
			ageDistributionChart = new HChart ("Age chart", ageDistribution);

			populationLine = new HIndexedChartLine ("Population", new DColor(200, 140, 255));
			populationChart = new HChart ("Population chart", populationLine);
			RecalculatePopulation ();

			birthRate = new HIndexedChartLine ("Birth rate", new DColor(100, 140, 255));
			deathRate = new HIndexedChartLine ("Death rate", new DColor(55, 80, 180));
			rateChart = new HChart ("Rates Chart", birthRate, deathRate);
			//RecalculateRates (0, 0, 1);

			chanceOfDeath = new HStandardChartLine ("Chance of death", new DColor(255, 80, 180));
			chanceOfDeath.AddPoint (0, 0.08f);
			chanceOfDeath.AddPoint (8, 0.01f);
			chanceOfDeath.AddPoint (16, 0.05f);
			chanceOfDeath.AddPoint (25, 0.03f);
			chanceOfDeath.AddPoint (60, 0.1f);
			chanceOfDeath.AddPoint (80, 0.2f);
			chanceOfDeath.AddPoint (90, 0.24f);
			chanceOfDeath.AddPoint (120, 1);
			chanceOfDeathChart = new HChart ("Death Chart", chanceOfDeath);

			var charts = new Table (2, 2, true);
			charts.SetRowSpacing (0, 10);
			charts.SetColSpacing (0, 10);
			charts.Attach (ageDistributionChart, 0, 1, 0, 1);
			charts.Attach (populationChart, 1, 2, 0, 1);
			charts.Attach (chanceOfDeathChart, 0, 1, 1, 2);
			charts.Attach (rateChart, 1, 2, 1, 2);

			var prevYear = new Button ("Prev Year");
			prevYear.Clicked += (sender, e) => PrevYear ();
			var nextYear = new Button ("Next Year");
			nextYear.Clicked += (sender, e) => NextYear ();
			var delYears = new Button ("Delete Years");
			delYears.Clicked += (sender, e) => DeleteYears ();
			var fertilityRateLabel = new Label ("Fertility rate:");
			fertilityRateLabel.ModifyFont(Pango.FontDescription.FromString("Sans 12"));
			fertilitySpinner = new SpinButton (0, 100, 0.1f);
			fertilitySpinner.Value = 2.100f;
			fertilitySpinner.SnapToTicks = false;

			var fertilitySettings = new HBox (false, 1);
			fertilitySettings.SetSizeRequest (10, -1);
			fertilitySpinner.ModifyBg (StateType.Normal, new Gdk.Color (150, 190, 200));
			fertilitySettings.Add (fertilityRateLabel);
			fertilitySettings.Add (fertilitySpinner);

			var bottomBar = new Table (1, 5, false);
			bottomBar.SetSizeRequest (-1, 30);
			bottomBar.Attach (prevYear, 0, 1, 0, 1);
			bottomBar.Attach (nextYear, 1, 2, 0, 1);
			bottomBar.Attach (delYears, 2, 3, 0, 1);
			bottomBar.Attach (fertilitySettings, 3, 5, 0, 1);

			var everythingBox = new VBox (false, 1);
			everythingBox.PackStart (charts, true, true, 0);
			everythingBox.PackStart (bottomBar, false, false, 0);

			window.Add (everythingBox);
			window.ShowAll ();
		}

		public void PrevYear()
		{

		}

		public void NextYear()
		{
			float prevPopulation = populationLine.GetValueOfLastPoint ();
			//People die
			float deaths = KillSome(); //Doesn't update any charts. Happens later.
			float births = CalculateBirths();
			Console.Out.WriteLine ("prevPopulation: {0}, births: {1}, deaths: {2}, maxAge: {3}", prevPopulation, births, deaths, ageDistribution.GetPointCount());

			//People are born and everyone age to make space for the young'ns
			AgeEveryone (births); //Updates the age chart
			RecalculateRates(births, deaths, prevPopulation); //Updates the rate chart
			RecalculatePopulation(); //Updates the population chart
		}

		float KillSome()
		{
			float deaths = 0;
			for (int i = 0; i < ageDistribution.GetPointCount (); i++) {
				float loss = chanceOfDeath.GetInterpolatedValue (i)*ageDistribution.GetValue(i);
				ageDistribution.GhostChangePoint (i, ageDistribution.GetValue (i) - loss);
				deaths += loss;
			}
			ageDistribution.UpdateBiggestYValue ();
			return deaths;
		}

		const int MIN_FERTILE_AGE = 15;
		const int MAX_FERTILE_AGE = 49;
		const int FERTILE_AGE_COUNT = MAX_FERTILE_AGE - MIN_FERTILE_AGE + 1;
		const float PERCENT_WOMEN = 0.5f;
		float CalculateBirths()
		{
			return (float)(ageDistribution.GetSumOfValuesBetween (MIN_FERTILE_AGE, MAX_FERTILE_AGE) * PERCENT_WOMEN * fertilitySpinner.Value / FERTILE_AGE_COUNT);
		}

		void AgeEveryone(float newborns)
		{
			ageDistribution.InsertPoint (0, newborns);
			ageDistribution.RemoveEmptyEnd ();
			ageDistributionChart.UpdateScale ();
			ageDistributionChart.QueueDraw ();
		}

		void RecalculateRates(float births, float deaths, float prevPopulation)
		{
			birthRate.AddPoint (births / prevPopulation * 1000);
			deathRate.AddPoint (deaths / prevPopulation * 1000);
			rateChart.UpdateScale ();
			rateChart.QueueDraw ();
		}

		void RecalculatePopulation()
		{
			populationLine.AddPoint ((int)ageDistribution.GetSumOfValues ());
			populationChart.UpdateScale ();
			populationChart.QueueDraw ();
		}

		public void DeleteYears()
		{

		}

		public static void Main (string[] args)
		{
			Application.Init ();
			Instance = new MainDemograftSim ();
			Application.Run ();
		}
	}
}
