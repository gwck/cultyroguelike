using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Tilemap baseMap;
    [SerializeField] private Tile tileA;
    [SerializeField] private Tile tileB;

    [SerializeField] [Range (3, 32)] private int roomWidth = 6;
    [SerializeField] [Range (3, 32)] private int roomHeight = 4;

    // Clears all tiles in all tilemaps.
    public void ClearAllTiles()
    {
        baseMap.ClearAllTiles();
    }

    // Determines the size of the room, then builds it.
    public void BuildRoom(int x, int y, int [,] map)
    {

        switch(map[x,y])
        {
            case 1: BuildRoom1(x, y, map); break;
            case 2: BuildRoom2(x, y, map); break;
            case 4: BuildRoom4(x, y, map); break;
        }
    }
    // Builds a 1x1 room.
    void BuildRoom1(int x, int y, int [,] map)
    {
        Tile[,] room = Border(tileA, new Tile[roomWidth, roomHeight]);

        // Left door.
        if (LevelGen.IsInGrid(x - 1, y, map) && map[x - 1, y] != 0)
        {
            room[0, 1] = null;
            room[0, 2] = null;
        }

        // Right door.
        if (LevelGen.IsInGrid(x + 1, y, map) && map[x + 1, y] != 0)
        {
            room[room.GetLength(0) - 1, 1] = null;
            room[room.GetLength(0) - 1, 2] = null;
        }

        // Roof hole.
        if (LevelGen.IsInGrid(x, y - 1, map) && map[x, y - 1] != 0)
        {
            room[room.GetLength(0) / 2, 0] = null;
        }

        // Floor hole.
        if (LevelGen.IsInGrid(x, y + 1, map) && map[x, y + 1] != 0)
        {
            room[room.GetLength(0) / 2, room.GetLength(1) - 1] = null;
        }

        PlaceRoom((x - map.GetLength(0) / 2) * roomWidth, (y - map.GetLength(1) / 2) * roomHeight, room);
    }

    // Builds a 2x1 room.
    void BuildRoom2(int x, int y, int [,] map)
    {
        Tile[,] room = Border(tileA, new Tile[roomWidth * 2, roomHeight]);

        // Left door.
        if (LevelGen.IsInGrid(x - 1, y, map) && map[x - 1, y] != 0)
        {
            room[0, 1] = null;
            room[0, 2] = null;
        }

        // Right door.
        if (LevelGen.IsInGrid(x + 2, y, map) && map[x + 2, y] != 0)
        {
            room[room.GetLength(0) - 1, 1] = null;
            room[room.GetLength(0) - 1, 2] = null;
        }

        PlaceRoom((x - map.GetLength(0) / 2) * roomWidth, (y - map.GetLength(1) / 2) * roomHeight, room);
    }

    // Builds a 2x2 room.
    void BuildRoom4(int x, int y, int [,] map)
    {
        Tile[,] room = Border(tileA, new Tile[roomWidth * 2, roomHeight * 2]);
        
        // Bottom left door.
        if (LevelGen.IsInGrid(x - 1, y, map) && map[x - 1, y] != 0)
        {
            room[0, 1] = null;
            room[0, 2] = null;
        }

        // Bottom right door.
        if (LevelGen.IsInGrid(x + 2, y, map) && map[x + 2, y] != 0)
        {
            room[room.GetLength(0) - 1, 1] = null;
            room[room.GetLength(0) - 1, 2] = null;
        }

        PlaceRoom((x - map.GetLength(0) / 2) * roomWidth, (y - map.GetLength(1) / 2) * roomHeight, room);
    }

    void PlaceRoom(int xOffset, int yOffset, Tile[,] room)
    {
        for (int y = 0; y < room.GetLength(1); y++)
        {
            for (int x = 0; x < room.GetLength(0); x++)
            {
                baseMap.SetTile(new Vector3Int(x + xOffset, y + yOffset, 0), room[x, y]);
            }
        }
    }

    // Place a rectangle in the tilemap.
    // If filled is false, only the border is printed.
    Tile[,] Border(Tile tile, Tile[,] room)
    {
        for (int i = 0; i < room.GetLength(0); i++)
        {
            room[i, 0] = tile;
            room[i, room.GetLength(1) - 1] = tile;
        }

        for (int i = 0; i < room.GetLength(1); i++)
        {
            room[0, i] = tile;
            room[room.GetLength(0) - 1, i] = tile;
        }

        return room;
    }
}