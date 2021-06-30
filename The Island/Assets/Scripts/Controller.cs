using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{

	//DO NOT CHANGE THESE NUMBERS EVER.
	private const int ALIVE_REQ = 4;
	private const int DEAD_REQ = 4;

	public const int WorldSize = 512;       //Height and width of world
	public const int UndergroundY = 135;    //level to begin stone majority
	public const int seaLevel = 189;        //Constants for sea generation NOTE: Difference between sealevel and seafloor does not affect sand dunes.
	public const int seaFloor = 129;
	public const int OceanRange = 40;       //The maximum allowed height for hills and sand dunes.
	public const int HillRange = 40;
	public const int endRange = 75;         //When a hill/sand dune get within endRange of its generation area (EX: Worldsize for hills) it'll begin making the final down/up hill to meet up with a beach.

	public int seed;
	public bool seedFlag = false;
	public Block[,] overWorld;
	public Tilemap map;
	public TileBase[] textures;


	// Start is called before the first frame update
	void Start()
	{
		overWorld = new Block[WorldSize, WorldSize];
		if (!seedFlag) {
			//Pick a random int.
			seed = Random.Range(System.Int32.MinValue, System.Int32.MaxValue);
		}
		Random.InitState(seed);
		//Console.WriteLine("Using seed: "+seed);

		//Setup basic flat world.
		for (int x = 0; x < WorldSize; x++)
		{
			for (int y = 0; y < WorldSize; y++)
			{
				if (y < UndergroundY) overWorld[x, y] = new Block(1);
				else if (y < seaLevel && x > WorldSize / 2) overWorld[x, y] = new Block(2);
				else overWorld[x, y] = new Block(0);
			}
		}

		//variables are made in the main function so the two functions can share their start and end points.
		int yf, ys;
		yf = Random.Range(0, 10) + 5;
		ys = Random.Range(0, 10) + 5;
		hillCreate(2, yf, ys, 5);
		oceanCreate(6, yf, ys);

		//Records ocean water locations for use later in game.
		//This is to help make it so ocean water is infinite, when something tries to remove ocean water from the z array this tells it to stop unless replaced by a solid block.
		//If a solid block is in the Z in the ocean and then remove game will auto fill in water in that location.
		//NOTE: water in the z or with air behind it with slowly filter away if it is NOT within the ocean water area. 
		/*
		 * oceanWater=new boolean[Sealevel-SeaFloor+1][WorldSize/2];
		for(int x=0;x<WorldSize/2;x++){
			for(int y=Sealevel;y>=SeaFloor;y--){
				System.out.println((y-SeaFloor)+" "+x);
				if(overWorld[y][x].ID==7)
					oceanWater[y-SeaFloor][x]=true;
				else
					oceanWater[y-SeaFloor][x]=false;
			}
		}
		for(int i = 0; i < 3; i++)
		{
			//to create a smoother transition between dirt and stone.
			transition(2, 1, UndergroundY, 2, 98, 2);
		}

		veinMaker(6, 1, 0, (WorldSize / 20), Sealevel - ((Sealevel - SeaFloor) * 3 / 8), Sealevel, 57, 7, 4, true);//blend sand with stone near the start of the world
		veinMaker(6, 1, (WorldSize / 2) - (WorldSize / 20), WorldSize / 2, Sealevel - 30, Sealevel, 57, 7, 4, true);//blend sand with stone near the end of the world
		veinMaker(6, 1, (WorldSize / 25), (WorldSize / 2) - (WorldSize / 25), SeaFloor - 5, Sealevel, 62, 7, 4, true);//Creates sand veins in the stone to create a decent mix
		veinMaker(6, 2, WorldSize * 49 / 50, WorldSize * 13 / 25, UndergroundY, Sealevel + yf + ys, 62, 7, 4, true);//helps make the beach seem more natural.

		//Both of these help make the two beaches mix with dirt a bit more naturally.
		veinMaker(2, 6, (WorldSize / 2) - endRange, (WorldSize * 13 / 25), UndergroundY - 10, Sealevel + ys, 50, 15, 5, true);
		veinMaker(2, 6, WorldSize * 24 / 25, endRange, UndergroundY - 10, Sealevel + yf, 50, 15, 5, true);

		veinMaker(1, 2, 0, WorldSize, UndergroundY, Sealevel + 91, 47, 10, 7, true);//adds pockets of stone into the dirt.
		veinMaker(2, 1, 0, WorldSize, 0, UndergroundY, 44, 7, 4, true);//adds dirt below sea level.

		addZ();//sets z array equal to current world without mineral veins.

		//mineral veins are created
		veinMaker(4, 1, 0, WorldSize, 0, Sealevel + (HillRange * 3 / 4), 40, 4, 2, false);//coal
		veinMaker(3, 1, 0, WorldSize, 0, UndergroundY * 7 / 8, 37, 3, 1, false);//iron
		*/

		caveCreate(45);
		//oceanSpread();//Begins letting the ocean water spill out filling caves on ocean floor.
		//makeTrees(77, 10, 3, 3, 7);

		Vector3Int pos = new Vector3Int(0, 0, 0);
		for(int x = 0; x < WorldSize * 2; x++)
		{
			for(int y = 0; y < WorldSize; y++)
			{
				pos = new Vector3Int(x, y, 0);
				int id = overWorld[x % WorldSize, y].getID() - 1;
				if(id != -1)	map.SetTile(pos, textures[id]);
			}
		}


	}

	/*void transition(int oBid, int nBid, int y, int varience, int chance, int step)
	{
		int xl = WorldSize;
		int c = Random.Range(1, 4);
		for(int i = 0; i < c; i++)
		{

		}
	}*/

	void caveCreate(int chance)
	{
		for (int x = 0; x < WorldSize; x++)
		{
			for (int y = 0; y < WorldSize; y++)
			{
				int roll = Random.Range(0, 99);
				if (x < 105 && roll < chance + 3)
					overWorld[x, y].setMarker(true);
				else if (roll < chance)
					overWorld[x, y].setMarker(true);
			}
		}

		for (int n = 0; n < 15; n++)
		{
			step(0, 0, WorldSize, WorldSize);
		}

		for (int x = 0; x < WorldSize; x++)
		{
			for (int y = 0; y < WorldSize; y++)
			{
				if (overWorld[x, y].getMarker() && overWorld[x, y].getID() != 7)
				{
					overWorld[x, y] = new Block(0);
				}
				else if (overWorld[x, y].getMarker())
				{
					overWorld[x, y].setMarker(false);
				}
			}
		}
	}

	void step(int x1, int y1, int x2, int y2)
	{
		Block[,] temp = new Block[WorldSize, WorldSize];

		if (x1 > x2)
		{
			x2 += WorldSize;
		}
		for (int y = y1; y < y2; y++)
		{
			for (int x = x1; x < x2; x++)
			{
				int liveCount = countAliveNeighbours(y, x % WorldSize);
				if(overWorld[x % WorldSize, y].getMarker())
				{
					if(liveCount < ALIVE_REQ)
					{
						temp[x % WorldSize, y] = overWorld[x % WorldSize, y];
						temp[x % WorldSize, y].setMarker(false);
					}
					else
					{
						temp[x % WorldSize, y] = overWorld[x % WorldSize, y];
					}
				}
				else
				{
					if(liveCount > DEAD_REQ)
					{
						temp[x % WorldSize, y] = overWorld[x % WorldSize, y];
						temp[x % WorldSize, y].setMarker(true);
					}
					else
					{
						temp[x % WorldSize, y] = overWorld[x % WorldSize, y];
					}
				}
			}
		}

		for(int i = x1; i < x2; i++)
		{
			for(int j = y1; j < y2; j++)
			{
				overWorld[i % WorldSize, j] = temp[i % WorldSize, j];
			}
		}
	}

	private int countAliveNeighbours(int y, int x)
	{
		int count = 0;
		for(int nx = x + WorldSize - 1; nx < x + WorldSize + 2; nx++)
		{
			for(int ny = y - 1; ny < y + 2; ny++)
			{
				if (!((nx == x + WorldSize && ny == y) || ny < 0 || ny >= WorldSize))
				{
					if (overWorld[nx % WorldSize, ny].getMarker()) count++;
				}
			}
		}
		return count;
	}

	void hillCreate(int bid, int yf, int ys, int altBid = -1)
	{
		int run = 0, height = 0;                        //Width and height of the hill segement generated. Note height will be doubled in generate compared to variable.
		int preY = 0;                                   //Y coord for the end of the previous segment.
		int x = 0, y = 0;                               //Local x of segment being generated.
		int originalX = WorldSize / 2;                  //Global x setting tracking the start position for each hill segment.
		bool direction = true;                          //Flag to set if hill segement curves up or down. TRUE = up, FALSE = down.
		y = ys;
		preY = y;
		height = Random.Range(0, (HillRange + 1) / 2) + 1;
		run = Random.Range(0, 70) + 5;
		//Generate first segment outside loop to get started due to it having larger constraints.
		for(x = 0; x < run; x++)
		{
			y = (int)(System.Math.Round((System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height + height + preY));
			//TODO add grass before dirt filldown.
			if (altBid != -1 && overWorld[x, y].isAir()) 
			{
				overWorld[x, y] = new Block(altBid);
				y--;
			}
			fillDown(x + originalX, y + seaLevel, bid);
		}

		while(x + originalX <= WorldSize - endRange)
		{
			originalX += run;
			preY = y;
			run = Random.Range(5, 50);
			height = Random.Range(1, 10);

			direction = (Random.Range(0, 1) % 2 == 0);
			if ((2 * height) + preY > HillRange)
				direction = false;
			if (preY - (2 * height) < 0)
				direction = true;

			for(x = 0; x < run; x++)
			{
				if (direction)
				{
					//Up hill formula.
					y = (int)(System.Math.Round((System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height + height + preY));
				}
				else
				{
					//Down hill formula.
					y = (int)(System.Math.Round((-1 * System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height - height + preY));
				}
				fillDown(x + originalX, y + seaLevel, bid);
			}

		}

		originalX += run;
		preY = y;
		run = WorldSize - originalX;
		//Final downhill slope to connect to sea.
		if(y - yf > 0)
		{
			height = (y - yf) / 2;
			direction = false;
		}
		else
		{
			height = (yf - y) / 2;
			direction = true;
		}
		for (x = 0; x < run; x++)
		{
			if (direction)
			{
				//Up hill formula.
				y = (int)(System.Math.Round((System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height + height + preY));
			}
			else
			{
				//Down hill formula.
				y = (int)(System.Math.Round((-1 * System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height - height + preY));
			}
			fillDown(x + originalX, y + seaLevel, bid);
		}
	}

	void oceanCreate(int bid, int yf, int ys)
	{
		int run = 0, height = 0;                        //Width and height of the hill segement generated. Note height will be doubled in generate compared to variable.
		int preY = 0;                                   //Y coord for the end of the previous segment.
		int x = 0, y = 0;                               //Local x of segment being generated.
		int originalX = 0;                              //Global x setting tracking the start position for each hill segment.
		bool direction = false;                         //Flag to set if hill segement curves up or down. TRUE = up, FALSE = down.
		preY = yf + seaLevel - seaFloor;
		height = Random.Range(yf, 4) / 2;
		run = Random.Range(10, 21);
		//Generate a small beach before a drop off into the sea.
		for (x = 0; x < run; x++)
		{
			y = (int)(System.Math.Round((-1 * System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height - height + preY));
			clear(x + originalX, y + seaFloor);
			fillDown(x + originalX, y + seaFloor, bid);
		}

		originalX += run;
		preY = y;
		height = Random.Range((seaLevel - seaFloor) * 3 / 8, (seaLevel - seaFloor) / 8);
		run = Random.Range(20, 25);
		for (x = 0; x < run; x++)
		{
			y = (int)(System.Math.Round((-1 * System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height - height + preY));
			clear(x + originalX, y + seaFloor);
			fillDown(x + originalX, y + seaFloor, bid);
		}

		while (x + originalX <= (WorldSize / 2) - endRange)
		{
			originalX += run;
			preY = y;
			run = Random.Range(5, 25);
			height = Random.Range(1, 7);

			direction = (Random.Range(0, 1) % 2 == 0);
			if ((2 * height) + preY > OceanRange)
				direction = false;
			if (preY - (2 * height) < 0)
				direction = true;

			for (x = 0; x < run; x++)
			{
				if (direction)
				{
					//Up hill formula.
					y = (int)(System.Math.Round((System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height + height + preY));
				}
				else
				{
					//Down hill formula.
					y = (int)(System.Math.Round((-1 * System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height - height + preY));
				}
				clear(x + originalX, y + seaFloor);
				fillDown(x + originalX, y + seaFloor, bid);
			}

		}

		originalX += run;
		preY = y;
		run = (WorldSize / 2) - originalX;
		height = (ys + seaLevel - seaFloor - y) / 2;
		for (x = 0; x < run; x++)
		{
			//Up hill formula.
			y = (int)(System.Math.Round((System.Math.Sin((x * System.Math.PI / run) - (0.5 * System.Math.PI))) * height + height + preY));
			clear(x + originalX, y + seaFloor);
			fillDown(x + originalX, y + seaFloor, bid);
		}

		//Fills the ocean with water once the sand dunes are made.
		for(int i = 0; i < WorldSize / 2; i++)
		{
			fillDown(i, seaLevel, 7);
		}
	}

	private void clear(int x, int y)
	{
		for(int i = y; i < seaLevel; i++)
		{
			overWorld[x, i] = new Block(0);
		}
	}

	private void fillDown(int x, int y, int bid)
	{
		bool flag = true;
		while (flag)
		{
			if (!overWorld[x, y].isAir()) 
				flag = false;
			else
			{
				overWorld[x, y] = new Block(bid);
				y--;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
