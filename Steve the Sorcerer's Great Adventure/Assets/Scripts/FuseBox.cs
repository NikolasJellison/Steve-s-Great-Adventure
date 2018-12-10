using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : MonoBehaviour {

    public GameObject hostJumpPad;
    public Material activationColor;
    public GameObject[] particlesActivation;
    [Header("1 = Ice, 2 = Fire")] public int FuseBoxType;

    public void fuseBoxHit()
    {
        hostJumpPad.GetComponent<JumpPad>().jumpPadOn = true;
        GetComponent<Renderer>().material = activationColor;
        for(int i = 0; i < particlesActivation.Length; i++)
        {
            particlesActivation[i].SetActive(true);
        }
        
    }
}
