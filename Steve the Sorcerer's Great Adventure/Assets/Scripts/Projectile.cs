using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [Header("1 = Ice, 2 = Fire")]
    public int projectileType;

    private void OnTriggerEnter(Collider other)
    {
        //Need to check if projectile type matches fuse box type
        if(other.tag == "FuseBox" && projectileType == other.GetComponent<FuseBox>().FuseBoxType)
        {
            other.GetComponent<FuseBox>().fuseBoxHit();
        }
        else if(other.tag != "Player")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Had issue where the projectiles would destroy on player if you looked too high
        //Correction, the objects were hitting the collider for the 'arm' object, so I taged the arm as player
        if(collision.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}
