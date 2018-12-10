using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

    [Header("1 = Ice, 2 = Fire")]
    public int projectileType;
    public float speed = 3;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update () {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        transform.LookAt(player.transform);
        if(Vector3.Distance(transform.position, player.transform.position) < 1)
        {
            player.GetComponent<PlayerController>().playerHit(projectileType);
            Destroy(gameObject);
        }
	}
}
