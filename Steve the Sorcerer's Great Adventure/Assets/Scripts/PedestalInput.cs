using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalInput : MonoBehaviour {

    //Doing this because it's easy
    [Header("Make sure you just tag the pedestal with the correct artifact")]
    public Transform artifactLocation;
    public float rotateSpeed = 3;
    private GameManager gameManager;
    private bool rotateArtifact;
    private Transform artifact;


    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == tag)
        {
            artifact = other.transform;
            other.transform.parent.transform.parent.transform.parent.GetComponent<PlayerController>().arm.SetActive(false);
            artifact.parent = transform;
            artifact.transform.position = artifactLocation.position;
            rotateArtifact = true;
            //send this info to the game manager
            gameManager.artifactCounter();
        }
    }

    private void Update()
    {
        if (rotateArtifact)
        {
            artifact.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
        }
    }
}
