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

    // Minimum number of rooms allowed.
    [SerializeField] private int roomMin = 10;

    // Mximum number of rooms allowed.
    [SerializeField] private int roomCap = 25;

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

    // Run the random walk algorithm and return an array containing the results.
    // 0 in the array is an empty space, 1 is a filled space.
    int[,] Walk()
    {
        int[,] rooms = new int[roomCap * 4, roomCap * 4];
        int center = roomCap * 2;

        // Instantiate the lists.
        List<Walker> walkers = new List<Walker>();
        List<Walker> toAdd = new List<Walker>();
        List<Walker> toRemove = new List<Walker>();

        // Color the origin point for the initial walkers, for testing purposes.
        //baseMap.SetTile(new Vector3Int(0, 0, 0), tileB);
        rooms[center, center] = 1;
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
        while (roomCount < roomCap && walkers.Count > 0)
        {
            foreach (Walker walker in walkers)
            {
                MoveWalker(walker);

                // Update the grid.
                if (rooms[walker.X, walker.Y] != 1)
                {
                    rooms[walker.X, walker.Y] = 1;
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

        return rooms;
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
        } else
        {
            if (Random.Range(0f, 1f) < verticalTurnChance)
            {
                walker.direction = (Random.Range(-1f, 1f) > verticalSkew) ? Walker.Direction.UP : Walker.Direction.DOWN;
            }
        }

        // move the walker based on its direction
        switch(walker.direction)
        {
            case Walker.Direction.RIGHT:    walker.X++; break;
            case Walker.Direction.LEFT:     walker.X--; break;
            case Walker.Direction.UP:       walker.Y++; break;
            case Walker.Direction.DOWN:     walker.Y--; break;
        }
    }

    void Generate()
    {
        baseMap.ClearAllTiles();
        // Run the algorithm.
        int[,] map = Walk();
        int roomWidth = 24;
        int roomHeight = 16;

        // Place a rectangle to represent each room.
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] == 1)
                {
                    Rectangle(tileA, (x - roomCap * 2) * roomWidth, (y - roomCap * 2) * roomHeight, roomWidth, roomHeight, false);
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
