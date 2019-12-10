using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGen : MonoBehaviour
{
    [SerializeField] private Tilemap baseMap;
    [SerializeField] private Tile tileA;
    [SerializeField] private Tile tileB;

    // Chance for a walker to create another walker at each step.
    [SerializeField] [Range (0,1)] private float branchChance = 0.02f;

    // Chance that a walker will turn at each step.   
    // Making horizontalTurnChance greater than verticalTurnChance will cause the
    // map to spread out horizontally more than vertically.
    [SerializeField] [Range(0, 1)] private float horizontalTurnChance = 0.3f;
    [SerializeField] [Range(0, 1)] private float verticalTurnChance = 0.3f;

    // Chance that a walker will be deleted after each move.
    [SerializeField] [Range(0, 1)] private float stopChance = 0.1f;

    // Chance that a square of rooms is merged into a large room.
    [SerializeField] [Range(0, 1)] private float merge2Chance = 0.5f;
    [SerializeField] [Range(0, 1)] private float merge4Chance = 0.5f;

    // Minimum number of rooms allowed.
    [SerializeField] private int roomMin = 10;

    // Mximum number of rooms allowed.
    [SerializeField] private int maxRooms = 25;

    // Number of walkers to start with. This can exceed maximum walkers.
    [SerializeField] private int initialWalkers = 1;

    // Maximum number of walkers allowable at a time.
    [SerializeField] private int walkerCap = 3;

    // Cause the map to grow in a particular direction.
    // Values of 0 cause no skew; a 1 or -1 would only allow growth in one direction.
    [SerializeField] [Range(-1, 1)] private float verticalSkew = 0f;
    [SerializeField] [Range(-1, 1)] private float horizontalSkew = 0f;

    // Represents a walker for the random walk algorithm.
    // A walker has X,Y coordinates and a direction.
    private class Walker
    {
        public enum Direction { RIGHT,DOWN,LEFT,UP }

        public int X;
        public int Y;
        public Direction direction;

        public Walker(int x, int y, Direction d)
        {
            X = x;
            Y = y;
            direction = d;
        }
    }

    // A simple coordinate array to represent a room for testing purposes.
    private int[,] room =
        {   {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2},
            {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2},
            {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2},
            {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2},
            {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2},
            {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2},
            {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2},
            {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2},
            {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};

    // Start is called before the first frame update.
    void Start()
    {
        Generate();
    }

    // Place a rectangle in the tilemap.
    // If filled is false, only the border is printed.
    void Rectangle(Tile tile, int xOffset, int yOffset, int width, int height, bool filled)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (filled || x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    baseMap.SetTile(new Vector3Int(x + xOffset, y + yOffset, 0), tile);
                }
            }
        }
    }

    bool IsInGrid(int x, int y, int[,] grid)
    {
        return x >= 0 && y >= 0 && x < grid.GetLength(0) && y < grid.GetLength(1);
    }

    // Run the random walk algorithm and return an array containing the results.
    // 0 in the array is an empty space, 1 is a filled space.
    int[,] Walk()
    {
        int[,] map = new int[maxRooms * 4, maxRooms * 4];
        int center = maxRooms * 2;

        // Instantiate the lists.
        List<Walker> walkers = new List<Walker>();
        List<Walker> toAdd = new List<Walker>();
        List<Walker> toRemove = new List<Walker>();

        // Color the origin point for the initial walkers, for testing purposes.
        //baseMap.SetTile(new Vector3Int(0, 0, 0), tileB);
        map[center, center] = 1;
        int roomCount = 1;

        // Create the initial walkers with random directions.
        for (int i = 0; i < initialWalkers; i++)
        {
            var direction = Walker.Direction.RIGHT;
            switch (Random.Range(0,4))
            {
                case 1: direction = Walker.Direction.DOWN;  break;
                case 2: direction = Walker.Direction.LEFT;  break;
                case 3: direction = Walker.Direction.UP;    break;
            }
            walkers.Add(new Walker(center, center, direction));
        }
        
        // Each loop call represents each walker taking a step.
        while (roomCount < maxRooms && walkers.Count > 0)
        {
            foreach (Walker walker in walkers)
            {
                MoveWalker(walker);

                // Update the grid.
                if (map[walker.X, walker.Y] != 1)
                {
                    map[walker.X, walker.Y] = 1;
                    roomCount++;

                    // Add the output to the tilemap.
                    //baseMap.SetTile(new Vector3Int(walker.X - center, walker.Y - center, 0), tileA);
                }

                // Queue walkers to be added.
                if (Random.Range(0f, 1f) < branchChance)
                {
                    toAdd.Add(new Walker(walker.X, walker.Y, walker.direction));
                }

                // Queue walkers to be deleted.
                if (Random.Range(0f, 1f) < stopChance)
                {
                    toRemove.Add(walker);
                }
            }

            // Resolve walkers being added.
            foreach (Walker walker in toAdd)
            {
                if (walkers.Count < walkerCap)
                {
                    walkers.Add(walker);
                }
            }

            toAdd.Clear();

            // Resolve walkers being deleted.
            foreach (Walker walker in toRemove)
            {
                if (walkers.Count > 1 || roomCount >= roomMin)
                {
                    walkers.Remove(walker);
                }
            }

            toRemove.Clear();
        }

        return map;
    }

    // move a walker one space
    void MoveWalker(Walker walker)
    {
        bool isMovingVertically = (walker.direction == Walker.Direction.UP) || (walker.direction == Walker.Direction.DOWN);

        if (isMovingVertically)
        {
            if (Random.Range(0f, 1f) < horizontalTurnChance)
            {
                walker.direction = (Random.Range(-1f, 1f) > horizontalSkew) ? Walker.Direction.LEFT : Walker.Direction.RIGHT;
            }
        }
        else
        {
            if (Random.Range(0f, 1f) < verticalTurnChance)
            {
                walker.direction = (Random.Range(-1f, 1f) > verticalSkew) ? Walker.Direction.UP : Walker.Direction.DOWN;
            }
        }

        // Move the walker based on its direction.
        switch (walker.direction)
        {
            case Walker.Direction.RIGHT: walker.X++; break;
            case Walker.Direction.LEFT: walker.X--; break;
            case Walker.Direction.UP: walker.Y++; break;
            case Walker.Direction.DOWN: walker.Y--; break;
        }
    }

    // Generate a room map based on stacked rows of rooms.
    int[,] Rows()
    {
        int mapWidth = (int)Mathf.Sqrt((float)maxRooms) + 1;
        int[,] map = new int[mapWidth, mapWidth];
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                map[x, y] = 1;
            }

        }

        return map;
    }

    // Merges squares of two rooms into a single large room, represented by a 2 at the left of that room.
    int[,] Merge2(int[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] == 1 &&
                    IsInGrid(x + 1, y, map) && map[x + 1, y] == 1)
                {
                    if (Random.Range(0f, 1f) < merge2Chance)
                    {
                        map[x, y] = 2;
                        map[x + 1, y] = 0;
                    }
                }
            }
        }
        return map;
    }

    // Merges squares of four rooms into a single large room, represented by a 4 at the top-left of that room.
    int[,] Merge4(int[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] == 1 &&
                    IsInGrid(x + 1, y, map) && map[x + 1, y] == 1 &&
                    IsInGrid(x, y + 1, map) && map[x, y + 1] == 1 &&
                    IsInGrid(x + 1, y + 1, map) && map[x + 1, y + 1] == 1)
                {
                    if (Random.Range(0f, 1f) < merge4Chance)
                    {
                        map[x, y] = 4;
                        map[x + 1, y] = 0;
                        map[x, y + 1] = 0;
                        map[x + 1, y + 1] = 0;
                    }
                }
            }
        }
        return map;
    }


    [SerializeField] private int roomWidth = 6;
    [SerializeField] private int roomHeight = 4;

    void Generate()
    {
        baseMap.ClearAllTiles();
        // Run the algorithm.
        int[,] map = Walk();
        map = Merge4(map);
        map = Merge2(map);

        // Place a rectangle to represent each room.
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] == 1)
                {
                    Rectangle(tileA, (x - map.GetLength(0) / 2) * roomWidth, (y - map.GetLength(1) / 2) * roomHeight, roomWidth, roomHeight, false);
                }
                else if (map[x, y] == 2)
                {
                    Rectangle(tileB, (x - map.GetLength(0) / 2) * roomWidth, (y - map.GetLength(1) / 2) * roomHeight, roomWidth * 2, roomHeight, false);
                }
                else if (map[x, y] == 4)
                {
                    Rectangle(tileB, (x - map.GetLength(0) / 2) * roomWidth, (y - map.GetLength(1) / 2) * roomHeight, roomWidth * 2, roomHeight * 2, false);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Print a new test map.
        if (Input.GetKeyDown("g"))
        {
            Generate();
        }
    }
}
