using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGen : MonoBehaviour
{
    [SerializeField] private GameObject startPoint; // starting point for the level generation
    [SerializeField] private GameObject stairPoint; // tracks where the last staircase was placed
    [SerializeField] private GameObject[] boss; // large rooms for boss fights and level ends
    [SerializeField] private GameObject[] segment; // segments of a floor
    [SerializeField] private GameObject[] stair; // staircase rooms
    [SerializeField] private GameObject[] end; // end rooms for each floor
    [SerializeField] private GameObject[] open; // segment open at the bottom to allow for stair
    [SerializeField] private GameObject[] spawn; // spawn point for player in level
    [SerializeField] private float width; // width of each room segment
    [SerializeField] private float height; // height of each room segment
    [SerializeField] private int minRooms; // minimum number of rooms on each floor
    [SerializeField] private int maxRooms; // maximum number of rooms on each floor
    [SerializeField] private int minFloors; // minimum number of floors in the level
    [SerializeField] private int maxFloors; // maximum number of floors in the level

    private bool done; // whether or not the level generation has finished
    private bool bossRoomPlaced; // whether or not a boss room has been placed
    private int floors;
    private int floor; // the floor currently being generated
    private int rooms; // the number of rooms on the current floor
    private int room; // the room currently being generated
    private int stairPos; // the room in which a staircase will be placed on this floor
    private int stairOffset; // the offset of the next floor from the 

    private const float interval = 0.25f; // amount of time before the next segment will be placed
    private float timer = 0f; // time passed since the last segment was placed

    // Start is called before the first frame update
    void Start()
    {
        // move to the starting point
        Vector2 newPos = startPoint.transform.position;
        transform.position = newPos;

        // initialize variables
        done = false;
        bossRoomPlaced = false;
        floor = 1;
        room = 0;

        // get the number of floors for the level
        floors = Random.Range(minFloors, maxFloors);
        // get the number of rooms on the first floor
        rooms = Random.Range(minRooms, maxRooms);
        // decide the position of the first staircase
        stairPos = Random.Range(1, rooms - 1);

        while (!done) Step();
    }

    private void Update()
    {
        /**
        // stop generating if done
        if (done) return;

        // generate one space at a time
        timer += Time.deltaTime;

        // check if the time has passed
        if (timer > interval)
        {
            Step();
            timer = 0;
        }*/
    }

    void Step()
    {
        // the position of the next room to be generated
        Vector2 newPos;

        if (room == 0) // place the first room in the row
        {
            newPos = PlaceStart();
        } else if (room == rooms) // place the last room in the row
        {
            newPos = PlaceEnd();
        } else if (room == stairPos) // place a staircase
        {
            newPos = PlaceStair();
        } else // place a normal room segment
        {
            newPos = PlaceSegment();
        }

        // move to the next position
        transform.position = newPos;
    }

    Vector2 PlaceSegment()
    {
        // place a segment
        // if above the previous staircase, place an open segment
        if (room == stairOffset)
        {
            Instantiate(open[Random.Range(0, open.Length)], transform.position, Quaternion.identity);
        } else
        {
            Instantiate(segment[Random.Range(0, segment.Length)], transform.position, Quaternion.identity);
        }
        // move right
        room++;
        return new Vector2(transform.position.x + width, transform.position.y);
    }

    Vector2 PlaceStart()
    {
        // place a starting room
        // if on the bottom floor, spawn the spawn room
        // if on the top floor, 50% chance of spawning a boss room
        if (floor == 1)
        {
            Instantiate(spawn[Random.Range(0, spawn.Length)], transform.position, Quaternion.identity);
        } else if (floor ==  floors && Random.Range(0f, 1f) > 0.5)
        {
            Instantiate(boss[Random.Range(0, boss.Length)], transform.position, Quaternion.identity);
            bossRoomPlaced = true;
        } else
        {
            Instantiate(end[Random.Range(0, end.Length)], transform.position, Quaternion.identity);
        }
        // move right
        room++;
        return new Vector2(transform.position.x + width, transform.position.y);
    }

    Vector2 PlaceEnd()
    {
        // place an end room
        // if on the top floor, place a boss room if one hasn't been placed at the other end
        if (floor == floors && !bossRoomPlaced)
        {
            Instantiate(boss[Random.Range(0, boss.Length)], transform.position, Quaternion.Euler(new Vector3(0, 180, 0)));
            bossRoomPlaced = true;
        }
        else
        {
            Instantiate(end[Random.Range(0, end.Length)], transform.position, Quaternion.Euler(new Vector3(0, 180, 0)));
        }

        // reset the room counter
        room = 0;
        rooms = Random.Range(minRooms, maxRooms);

        // offset the next floor, making sure that the previous floor's staircase leads to it
        stairOffset = Random.Range(1, rooms - 1);

        // move to the next floor
        floor++;
        if (floor > floors) {
            // end the level generation if there are no more floors to generate
            done = true;
        } else if (floor == floors) {
            // no stair on the top floor
            stairPos = -1;
        } else
        {
            // choose the location of the next stair
            stairPos = Random.Range(1, rooms - 1);
            for (int i = 0; i < 100; i++)
            {
                if (stairPos == stairOffset)
                {
                    // choose the location of the next stair
                    stairPos = Random.Range(1, rooms - 1);
                } else
                {
                    break;
                }
            }
        }
        // move to the first room of the next floor, based on the location of the staircase
        return new Vector2(stairPoint.transform.position.x -  (stairOffset * width), stairPoint.transform.position.y + height);
    }

    Vector2 PlaceStair()
    {
        // place a stair room
        Instantiate(stair[Random.Range(0, stair.Length)], transform.position, Quaternion.identity);
        // note the location of the stair room
        stairPoint.transform.position = new Vector2(transform.position.x, transform.position.y);
        // move right
        room++;
        return new Vector2(transform.position.x + width, transform.position.y);
    }
}
