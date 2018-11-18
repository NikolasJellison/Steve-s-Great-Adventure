using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {

    public float jumpPadStrength = 1000;
    public bool jumpPadOn;


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && jumpPadOn)
        {
            Debug.Log("launching player");
            other.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPadStrength);
        }
    }
}
