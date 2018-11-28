using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {
    public Transform player;
    public Transform home;
    private NavMeshAgent me;
    public float wanderRange;
    private Vector3 wanderDestination;
    private bool wandering;


	// Use this for initialization
	void Start () {
        me = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {

        if(wandering && Vector3.Distance(transform.position, wanderDestination) < .5)
        {
            wandering = false;
        }
        //sloppy but i need to get this working
        if (!wandering && Vector3.Distance(transform.position, player.position) > wanderRange)
        {
            wandering = true;
            wanderDestination = new Vector3(Random.Range(home.position.x, home.position.x + wanderRange), home.position.y, Random.Range(home.position.z, home.position.z + wanderRange));
            me.destination = wanderDestination;
        }

        if (Vector3.Distance(transform.position, player.position) < wanderRange)
        {
            wandering = false;
            me.destination = player.position;
        }
    }
}
