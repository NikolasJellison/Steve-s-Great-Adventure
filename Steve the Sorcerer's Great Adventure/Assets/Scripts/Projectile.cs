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
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
