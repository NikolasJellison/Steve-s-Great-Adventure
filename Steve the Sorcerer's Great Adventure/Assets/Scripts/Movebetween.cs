using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movebetween : MonoBehaviour {

    public Transform[] waypoints;
    public float speed;
    private float step;
    private int counter = 0;

	// Use this for initialization
	void Start () {
        step = speed * Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[counter].position, step);
        if(Vector3.Distance(transform.position, waypoints[counter].position) < .1)
        {
            counter++;
            if(counter >= waypoints.Length)
            {
                counter = 0;
            }
        }
        //Debug.Log(counter);
	}
}
