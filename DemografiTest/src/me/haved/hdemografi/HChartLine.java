package me.haved.hdemografi;

import java.util.ArrayList;
import java.awt.Color;

public class HChartLine {
	private ArrayList<HChartLinePoint> points;
	
	private Color color;
	private String name;
	
	public HChartLine(String name, Color color)
	{
		points = new ArrayList<HChartLinePoint>();
		this.name = name;
		this.color = color;
	}
	
	public HChartLine(ArrayList<HChartLinePoint> points, String name, Color color)
	{
		this.points = points;
		this.name = name;
		this.color = color;
	}
	
	public void addPoint(HChartLinePoint point)
	{
		for(int i = 0; i < points.size(); i++)
			if(point.x < points.get(i).x)
			{
				points.add(i, point);
				return;
			}
		points.add(point);
	}
	
	public float getBiggestXValue()
	{
		if(points.size()==0)
			return 0;
		return points.get(points.size()-1).x;
	}
	
	public float getBiggestYValue()
	{
		float out = 0;
		for(HChartLinePoint point:points)
			out = Math.max(out, point.y);
		
		return out;
	}
	
	public int GetPointCount()
	{
		return points.size();
	}
	
	public HChartLinePoint GetPoint(int index)
	{
		return points.get(index);
	}
	
	public Color getColor() {
		return color;
	}

	public void setColor(Color color) {
		this.color = color;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}
}
