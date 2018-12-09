using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookFloat : MonoBehaviour {
    //Because i don't know how animations work, they would always go to a fixed location in the world
    //I probably needed to make like a  parent object and have the object move based on that or something
    public float vertSpeed = 1.5f;
    public float rotationSpeed = 360;
    public float height = 2;
    public bool rotate = true;
    private float vStep;
    private float rStep;
    private Vector3 originalPosition;
    private Vector3 verticalPosition;
    private bool movingUp = true;

	// Use this for initialization
	void Start () {
        originalPosition = transform.position;
        verticalPosition = new Vector3 (transform.position.x, transform.position.y + height, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        vStep = vertSpeed * Time.deltaTime;
        rStep = rotationSpeed * Time.deltaTime;
        //Moving up and down
        if (movingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, verticalPosition, vStep);
            if(Vector3.Distance(transform.position, verticalPosition) < .1)
            {
                movingUp = false;
            }
        }
        if (!movingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, vStep);
            if (Vector3.Distance(transform.position, originalPosition) < .1)
            {
                movingUp = true;
            }
        }
        if(rotate)
        transform.Rotate(Vector3.up * rStep);
    }
}
