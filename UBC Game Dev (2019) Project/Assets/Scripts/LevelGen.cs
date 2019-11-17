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

    // chance for a walker to create another walker at each step
    [SerializeField] [Range (0,1)] private float branchChance = 0.02f;
    // chance that a walker will turn after moving
    [SerializeField] [Range(0, 1)] private float turnChance = 0.3f;
    // degree to which the map spreads horizontally rather than vertically
    [SerializeField] [Range(0, 1)] private float horizontality = 0.5f;
    // chance that a walker will be deleted after each move
    [SerializeField] [Range(0, 1)] private float stopChance = 0.1f;
    // minimum number of rooms allowed
    [SerializeField] private int roomMin = 10;
    // maximum number of rooms allowed
    [SerializeField] private int roomCap = 25;
    // maximum number of walkers allowable at a time
    [SerializeField] private int walkerCap = 3;

    // determine probabilities of moving up/down or left/right
    [SerializeField] [Range(-1, 1)] private float verticalBalance = 0f;
    [SerializeField] [Range(-1, 1)] private float horizontalBalance = 0f;

    // represents a walker for the random walk algorithm
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

    private int[,] map =
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

    // Start is called before the first frame update
    void Start()
    {
        Walk();
    }

    // place a rectangle in the tilemap
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

    void Walk()
    {
        int[,] rooms = new int[roomCap * 4, roomCap * 4];

        // instantiate the lists
        List<Walker> walkers = new List<Walker>();
        List<Walker> toAdd = new List<Walker>();
        List<Walker> toRemove = new List<Walker>();

        // create the first walker
        int center = roomCap * 2;
        walkers.Add(new Walker(center, center, Walker.Direction.RIGHT));
        rooms[center, center] = 1;
        baseMap.SetTile(new Vector3Int(0, 0, 0), tileB);
        int roomCount = 1;

        // guard against infinite loops
        // each loop call represents each walker taking a step
        while (roomCount < roomCap && walkers.Count > 0)
        {
            foreach (Walker walker in walkers)
            {
                // move the walker
                MoveWalker(walker);

                // update the grid
                if (rooms[walker.X, walker.Y] != 1)
                {
                    rooms[walker.X, walker.Y] = 1;
                    roomCount++;

                    // add the output to the tilemap
                    // this will become more complicated later
                    baseMap.SetTile(new Vector3Int(walker.X - center, walker.Y - center, 0), tileA);
                }

                // queue walkers to be added
                if (Random.Range(0f, 1f) < branchChance)
                {
                    toAdd.Add(new Walker(walker.X, walker.Y, walker.direction));
                }

                // queue walkers to be deleted
                if (Random.Range(0f, 1f) < stopChance)
                {
                    toRemove.Add(walker);
                }
            }

            // resolve walkers being removed
            foreach (Walker walker in toRemove)
            {
                if (walkers.Count > 1 || roomCount >= roomMin)
                {
                    walkers.Remove(walker);
                }
            }

            toRemove.Clear();
            toRemove.TrimExcess();

            // resolve new walkers
            foreach (Walker walker in toAdd)
            {
                if (walkers.Count < walkerCap)
                {
                    walkers.Add(walker);
                }
            }

            toAdd.Clear();
            toRemove.TrimExcess();
        }
    }

    // move a walker one space
    void MoveWalker(Walker walker)
    {
        // decide whether the walker should change directions
        float rand = Random.Range(0f, 1f);

        if (Random.Range(0f,1f) < turnChance)
        {
            if (rand <= horizontality && walker.direction == Walker.Direction.DOWN || walker.direction == Walker.Direction.UP)
            {
                walker.direction = (Random.Range(-1f, 1f) > horizontalBalance) ? Walker.Direction.LEFT : Walker.Direction.RIGHT;
            } else if (rand > horizontality)
            {
                walker.direction = (Random.Range(-1f, 1f) > verticalBalance) ? Walker.Direction.UP : Walker.Direction.DOWN;
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            baseMap.ClearAllTiles();
            Walk();
        }
    }
}
