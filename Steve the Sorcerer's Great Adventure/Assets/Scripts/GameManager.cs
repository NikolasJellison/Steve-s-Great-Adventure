using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header(":D")]
    [Tooltip("Put the 3 room checkpoints here")]
    public Transform[] respawnPoints;
    public GameObject player;
    [HideInInspector] public int checkpointCounter;
    [Header("Room one Reset info")]
    public GameObject lava;
    private Vector3 lavaOriginal;
    [HideInInspector] public int artifactsPlaced;
    [Header("Library stuff")] public GameObject doorToRoom3;

    private void Start()
    {
        lavaOriginal = lava.transform.position;
    }

    public void respawn()
    {
        player.transform.position = respawnPoints[checkpointCounter].position;
        //This is where we reset things if needed
        //Room 1 = 0 in checkpoint counter and etc because array
        switch (checkpointCounter)
        {
            case 0:
                lava.transform.position = lavaOriginal;
                break;

            case 1:
                //Reset library if needed
                break;

            case 2:
                //reset enemy room
                //coming back to this one eventually
                break;

        }
    }
    public void NextLevel()
    {
        checkpointCounter++;
    }
    public void artifactCounter()
    {
        //Library stuff
        artifactsPlaced++;
        if (artifactsPlaced == 4)
        {
            //Also place other particle effects and stuff that we would want to use here
            Destroy(doorToRoom3);
        }
    }
    
}
