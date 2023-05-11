using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Filename : FluidGrid
 *  Author:Louis-E
 * Goal : Create the info grid for the search algorithm
 * 
 * Requirements : Create an instance in a script to use as a container
 */
public class FluidGrid 
{
	float cellSize;
	int gridLengthX;
	int gridLengthY;
	Vector2 gridOffset;
	

	List<int>[,] gridInfo;

	//Create the grid
	public FluidGrid(float cellSize, int gridLengthX, int gridLengthY, Vector2 offset)
	{
		this.cellSize = cellSize;
		this.gridLengthX = gridLengthX;
		this.gridLengthY = gridLengthY;
		gridOffset = offset;
		gridInfo = new List<int>[gridLengthX,gridLengthY];
		for (int i = 0; i < gridLengthX; i++)
		{
			for (int j = 0; j < gridLengthY; j++)
			{
				gridInfo[i, j] = new List<int>();
			}
		}

	}


	//Reset the value of all cells
	public void ResetGrid() 
	{
		for (int i = 0; i < gridLengthX; i++) 
		{
			for (int j = 0; j < gridLengthY; j++) 
			{
				gridInfo[i, j].Clear();
			}
		}
	}

	//Add particle to grid
	public void AddParticle( Particle p) 
	{
		Vector3 pPos = p.GetPosition();
		int x = (int)((pPos.x-gridOffset.x) / cellSize);
		int y = (int)((pPos.y-gridOffset.y) / cellSize);
		
		gridInfo[x, y].Add(p.GetIndex());

		p.SetGridPosition(new intPosition(x, y));
		
	}

	
	//Return the list of all possible neighbors for a give particle
	public List<int> PossibleNeighbors(Particle p) 
	{
		List<int> possibleNeighbors = new List<int>();
		intPosition midCell = p.GetGridPosition();

		//Check midCell
		intPosition newCoord = new intPosition(midCell.x, midCell.y);
		for (int i = 0; i < gridInfo[midCell.x, midCell.y].Count; i++) 
		{
			if(gridInfo[midCell.x, midCell.y][i] != p.GetIndex())
				possibleNeighbors.Add(gridInfo[midCell.x, midCell.y][i]);
		}
		//Check Top
		if (midCell.y + 1 < gridLengthY) 
		{
			newCoord = new intPosition(midCell.x, midCell.y + 1);
			for (int i = 0; i < gridInfo[newCoord.x, newCoord.y].Count; i++)
			{
				possibleNeighbors.Add(gridInfo[newCoord.x, newCoord.y][i]);
			}
		}
		//Check Down
		if (midCell.y - 1 >= 0) 
		{
			newCoord = new intPosition(midCell.x, midCell.y - 1);
			for (int i = 0; i < gridInfo[newCoord.x, newCoord.y].Count; i++)
			{
				possibleNeighbors.Add(gridInfo[newCoord.x, newCoord.y][i]);
			}
		}


		//Check left column
		if (midCell.x - 1 >= 0) 
		{
			//Top left
			if (midCell.y + 1 < gridLengthY) 
			{
				newCoord = new intPosition(midCell.x-1, midCell.y+1);
				for (int i = 0; i < gridInfo[newCoord.x, newCoord.y].Count; i++)
				{
					possibleNeighbors.Add(gridInfo[newCoord.x, newCoord.y][i]);
				}

			}
			//Down left
			if (midCell.y - 1 >= 0)
			{
				newCoord = new intPosition(midCell.x - 1, midCell.y - 1);
				for (int i = 0; i < gridInfo[newCoord.x, newCoord.y].Count; i++)
				{
					possibleNeighbors.Add(gridInfo[newCoord.x, newCoord.y][i]);
				}
			}

			//Left
			newCoord = new intPosition(midCell.x - 1, midCell.y);
			for (int i = 0; i < gridInfo[newCoord.x, newCoord.y].Count; i++)
			{
				possibleNeighbors.Add(gridInfo[newCoord.x, newCoord.y][i]);
			}
		}



		//Check right column
		if (midCell.x + 1 < gridLengthX)
		{
			//Top right
			if (midCell.y + 1 < gridLengthY)
			{
				newCoord = new intPosition(midCell.x + 1, midCell.y + 1);
				for (int i = 0; i < gridInfo[newCoord.x, newCoord.y].Count; i++)
				{
					possibleNeighbors.Add(gridInfo[newCoord.x, newCoord.y][i]);
				}

			}
			//Down right
			if (midCell.y - 1 >= 0)
			{
				newCoord = new intPosition(midCell.x + 1, midCell.y - 1);
				for (int i = 0; i < gridInfo[newCoord.x, newCoord.y].Count; i++)
				{
					possibleNeighbors.Add(gridInfo[newCoord.x, newCoord.y][i]);
				}
			}

			//Right
			newCoord = new intPosition(midCell.x + 1, midCell.y);
			for (int i = 0; i < gridInfo[newCoord.x, newCoord.y].Count; i++)
			{
				possibleNeighbors.Add(gridInfo[newCoord.x, newCoord.y][i]);
			}
		}

		return possibleNeighbors;
	}



}
