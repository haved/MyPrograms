package me.haved.hdemografi;

import java.awt.Color;

import javax.swing.JFrame;
import javax.swing.JSplitPane;

public class HDemografiMain extends JFrame
{
	private static final long serialVersionUID = -8868468441099640102L;
	
	static HDemografiMain Instance;
	
	HChart population;
	HChart rates;
	HChart ageRepresentation;
	HChart deathLikeliness;
	
	public HDemografiMain()
	{
		super("HDemografi");
		
		HChartLine populationLine = new HChartLine("Population", Color.RED);
		populationLine.addPoint(new HChartLinePoint(1, 1));
		populationLine.addPoint(new HChartLinePoint(5, 6));
		populationLine.addPoint(new HChartLinePoint(3.5f, 3));
		populationLine.addPoint(new HChartLinePoint(4, 5));
		population = new HChart("PopChart", new HChartLine[] {populationLine});
		
		HChartLine birthRateLine = new HChartLine("Birth rate", Color.BLUE.brighter());
		birthRateLine.addPoint(new HChartLinePoint(1, 2));
		birthRateLine.addPoint(new HChartLinePoint(3, 3));
		birthRateLine.addPoint(new HChartLinePoint(3.5f, 2.9f));
		birthRateLine.addPoint(new HChartLinePoint(6, 0.4f));
		HChartLine deathRateLine = new HChartLine("Death rate", Color.BLUE.darker().darker());
		deathRateLine.addPoint(new HChartLinePoint(1, 1.4f));
		deathRateLine.addPoint(new HChartLinePoint(3, 2));
		deathRateLine.addPoint(new HChartLinePoint(3.5f, 2.4f));
		deathRateLine.addPoint(new HChartLinePoint(6, 1.8f));
		rates = new HChart("Rates", new HChartLine[] {birthRateLine, deathRateLine});
		
		JSplitPane verticalSplitter = new JSplitPane(JSplitPane.VERTICAL_SPLIT, population, rates);
		verticalSplitter.setResizeWeight(0.5f);
		
		add(verticalSplitter);
		pack();
		setDefaultCloseOperation(EXIT_ON_CLOSE);
	}
	
	public static void main(String[] args)
	{
		Instance = new HDemografiMain();
		Instance.setVisible(true);
	}
}
