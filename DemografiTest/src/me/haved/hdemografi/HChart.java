package me.haved.hdemografi;

import javax.swing.JComponent;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;

public class HChart extends JComponent
{
	private static final long serialVersionUID = 1880246289532404595L;
	
	private String name;
	
	private float xSize;
	private float ySize;
	
	private HChartLine[] lines;
	
	public HChart(String name, HChartLine[] lines)
	{
		init(name, lines);
	}
	
	private void init(String name, HChartLine[] lines)
	{
		this.name = name;
		this.lines = lines;
		float newXSize=0, newYSize=0;
		for(HChartLine line:lines){
			newXSize = Math.max(newXSize, line.getBiggestXValue());
			newYSize = Math.max(newYSize, line.getBiggestYValue());
		}
		useXSize(newXSize);
		useYSize(newYSize);
		setPreferredSize(new Dimension(350, 200));
	}
	
	private static final Color BG_COLOR = new Color(230, 230, 230);
	private static final Color NAME_COLOR = Color.DARK_GRAY;
	
	@Override
	public void paintComponent(Graphics g)
	{
		g.setColor(BG_COLOR);
		g.fillRect(0, 0, getWidth(), getHeight());
		g.setColor(NAME_COLOR);
		g.drawString(name, 10, 10);
		
		for(int i = 0; i < lines.length; i++)
		{
			HChartLine line = lines[i];
			g.setColor(NAME_COLOR);
			g.drawString(line.getName(), 30, 28 + 20*i);
			g.setColor(line.getColor());
			g.fillRect(10, 15 + 20 * i, 15, 15);
			
			if(line.GetPointCount()==0)
				continue;
			HChartLinePoint nextPoint = line.GetPoint(0);
			for(int j = 0; j < line.GetPointCount()-1; j++)
			{
				HChartLinePoint currentPoint=nextPoint;
				nextPoint = line.GetPoint(j+1);
				g.fillRect((int)(currentPoint.x/xSize*getWidth()-4), (int)((1-currentPoint.y/ySize)*getHeight()-4), 8, 8);
				g.drawLine((int)(currentPoint.x/xSize*getWidth()), (int)((1-currentPoint.y/ySize)*getHeight()), 
						(int)(nextPoint.x/xSize*getWidth()), (int)((1-nextPoint.y/ySize)*getHeight()));
			}
			g.fillRect((int)(nextPoint.x/xSize*getWidth()-4), (int)((1-nextPoint.y/ySize)*getHeight()-4), 8, 8);
		}
	}
	
	public float getXSize() {
		return xSize;
	}

	public void useXSize(float xSize) {
		this.xSize = xSize*1.02f;
	}

	public float getYSize() {
		return ySize;
	}

	public void useYSize(float ySize) {
		this.ySize = ySize*1.1f;
	}
}
