using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameMaster : MonoBehaviour
{

    public int spawnDelay;

    public static GameMaster gm; //reference to instance of GameMaster

    public Transform playerPrefab;
    public Transform spawnPoint;

    private CinemachineVirtualCamera myCinemachine;

    private void Start()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        }
        myCinemachine = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
    }

    public IEnumerator RespawnPlayer()
    {
        //Let some audio play here?

        yield return new WaitForSeconds(spawnDelay); //waits for certain seconds before player respawns

        var spawnedPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation); //instantiates object (player) at specific position and rotation (spawnpoint)
        FindPlayer(spawnedPlayer);

        //Put respawn effects here?

        

    }
    public static void KillPlayer(PlayerController player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.RespawnPlayer()); //cannot call other methods in static method
    }

    private void FindPlayer(Transform spawnedPlayer)
    {
        myCinemachine.m_Follow = spawnedPlayer;

    }
}
