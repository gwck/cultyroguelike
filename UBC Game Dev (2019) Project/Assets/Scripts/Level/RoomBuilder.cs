using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Tilemap baseMap;
    [SerializeField] private Tile tileA;
    [SerializeField] private Tile tileB;
    [SerializeField] private TextAsset presets;

    List<int[,]> Presets1;

    private const int RWIDTH = 8;
    private const int RHEIGHT = 6;

    struct DoorSet
    {
        public bool bottom, top, left, right, rbottom, rtop, uleft, uright;
    }
    void Start()
    {
        Presets1 = new List<int[,]>();
        ReadPresets();
    }

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
        Tile[,] room = Border(tileA, new Tile[RWIDTH, RHEIGHT]);
        DoorSet ds = Doors(x, y, map);

        if (ds.left)
        {
            room[0, 1] = null;
            room[0, 2] = null;
        }

        if (ds.right)
        {
            room[7, 1] = null;
            room[7, 2] = null;
        }

        if (ds.bottom)
        {
            room[3, 0] = null;
            room[4, 0] = null;
        }

        if (ds.top)
        {
            room[3, 5] = null;
            room[4, 5] = null;
        }

        PlaceRoom((x - map.GetLength(0) / 2) * RWIDTH, (y - map.GetLength(1) / 2) * RHEIGHT, room);
    }

    // Builds a 2x1 room.
    void BuildRoom2(int x, int y, int [,] map)
    {
        Tile[,] room = Border(tileA, new Tile[RWIDTH * 2, RHEIGHT]);
        DoorSet ds = Doors(x, y, map);

        if (ds.left)
        {
            room[0, 1] = null;
            room[0, 2] = null;
        }

        if (ds.right)
        {
            room[15, 1] = null;
            room[15, 2] = null;
        }

        if (ds.bottom)
        {
            room[3, 0] = null;
            room[4, 0] = null;
        }

        if (ds.top)
        {
            room[3, 5] = null;
            room[4, 5] = null;
        }

        if (ds.rbottom)
        {
            room[11, 0] = null;
            room[12, 0] = null;
        }

        if (ds.rtop)
        {
            room[11, 5] = null;
            room[12, 5] = null;
        }

        PlaceRoom((x - map.GetLength(0) / 2) * RWIDTH, (y - map.GetLength(1) / 2) * RHEIGHT, room);
    }

    // Builds a 2x2 room.
    void BuildRoom4(int x, int y, int [,] map)
    {
        Tile[,] room = Border(tileA, new Tile[RWIDTH * 2, RHEIGHT * 2]);
        DoorSet ds = Doors(x, y, map);

        if (ds.left)
        {
            room[0, 1] = null;
            room[0, 2] = null;
        }

        if (ds.right)
        {
            room[15, 1] = null;
            room[15, 2] = null;
        }

        if (ds.bottom)
        {
            room[3, 0] = null;
            room[4, 0] = null;
        }

        if (ds.top)
        {
            room[3, 11] = null;
            room[4, 11] = null;
        }

        if (ds.uleft)
        {
            room[0, 7] = null;
            room[0, 8] = null;
        }

        if (ds.uright)
        {
            room[15, 7] = null;
            room[15, 8] = null;
        }

        if (ds.rbottom)
        {
            room[11, 0] = null;
            room[12, 0] = null;
        }

        if (ds.rtop)
        {
            room[11, 11] = null;
            room[12, 11] = null;
        }

        PlaceRoom((x - map.GetLength(0) / 2) * RWIDTH, (y - map.GetLength(1) / 2) * RHEIGHT, room);
    }

    bool RoomCheck(int x, int y, int [,] map)
    {
        return LevelGen.IsInGrid(x, y, map) && map[x, y] != 0;
    }

    // Create a struct to represent the doors in a given room.
    DoorSet Doors(int x, int y, int [,] map)
    {
        DoorSet ds = new DoorSet();

        ds.bottom = false;
        ds.top = false;
        ds.left = false;
        ds.right = false;
        ds.rbottom = false;
        ds.rtop = false;
        ds.uleft = false;
        ds.uright = false;

        // Check the possible doors for each room type and store them in the struct.
        // Recall that in the tilemap, y increases upwards and x rightwards.
        switch(map[x,y])
        {
            case 1:
                ds.bottom = RoomCheck(x, y - 1, map);
                ds.top = RoomCheck(x, y + 1, map);
                ds.left = RoomCheck(x - 1, y, map);
                ds.right = RoomCheck(x + 1, y, map);
                break;
            case 2:
                ds.bottom = RoomCheck(x, y - 1, map);
                ds.top = RoomCheck(x, y + 1, map);
                ds.rbottom = RoomCheck(x + 1, y - 1, map);
                ds.rtop = RoomCheck(x + 1, y + 1, map);
                ds.left = RoomCheck(x - 1, y, map);
                ds.right = RoomCheck(x + 2, y, map);
                break;
            case 4:
                ds.bottom = RoomCheck(x, y - 1, map);
                ds.top = RoomCheck(x, y + 2, map);
                ds.rbottom = RoomCheck(x + 1, y - 1, map);
                ds.rtop = RoomCheck(x + 1, y + 2, map);
                ds.left = RoomCheck(x - 1, y, map);
                ds.right = RoomCheck(x + 2, y, map);
                ds.uleft = RoomCheck(x - 1, y + 1, map);
                ds.uright = RoomCheck(x + 2, y + 1, map);
                break;
        }

        return ds;
    }

    void ReadPresets()
    {
        try
        {
            string raw = presets.text;
            string[] lines = raw.Split('\r');
            if (lines[0].Split(' ')[1].Trim() == "1")
            {

            }
            foreach (string line in lines)
            {
                Debug.Log(line);
            }
        } catch (System.Exception)
        {
            Debug.Log("Read failed.");
        }
    }

    // Places a room in the tilemaps.
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

    // Make a border of the specified tile around the room.
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