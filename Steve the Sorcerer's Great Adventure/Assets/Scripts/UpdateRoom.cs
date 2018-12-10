using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateRoom : MonoBehaviour {
    public int roomNum;
    public AudioSource libMusic;
    public AudioSource room3Music;
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag== "Player")
        {
            if(roomNum == 2)
            {
                libMusic.Play();
            }
            else if(roomNum == 3)
            {
                gameManager.checkpointCounter = 2;
                libMusic.Stop();
                room3Music.Play();
            }
            else if(roomNum == 4)
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}
