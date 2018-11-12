using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    //I Understand that this isn't really a player controller, more of a player manager, but i don't want to change the name

    //We use tags to distinguish which objects can get toggled in this game

    //Key for the mechanic
    [Tooltip("Which key activates the toggle of the environment")]public KeyCode mechanicKey = KeyCode.E;
    [Tooltip("Which key activates interaction (books)")] public KeyCode interactKey = KeyCode.Q;
    [Header("Materials that will get swaped")]
    public Material blueOn;
    public Material blueOff;
    public Material redOn;
    public Material redOff;
    public Text interactionText;
    [Tooltip("range for interaction")]
    public float interactRange = 10;
    //Current layout is blue = false and red = true. If you want to change it, go to the mechanicFlip() function/method/whatever
    private bool mechanicToggle;
    private GameObject[] blueObjects;
    private GameObject[] redObjects;
    //for books
    private RaycastHit hit;
    private bool lookingAtBook;

    private void Start()
    {
        //Finding all objects that can be toggled
        if(blueObjects == null)
        {
            blueObjects = GameObject.FindGameObjectsWithTag("Blue");
        }
        if(redObjects == null)
        {
            redObjects = GameObject.FindGameObjectsWithTag("Red");
        }
        //Need to run it once so everything gets a material and whatnot
        mechanicFlip();
    }

    //Didn't allow fast pressing of key
    /*private void FixedUpdate()
    {
        if (Input.GetKeyDown(mechanicKey))
        {
            mechanicFlip();
        }
    }
    */
    private void Update()
    {
        if (Input.GetKeyDown(mechanicKey))
        {
            mechanicFlip();
        }

        //Raycasting for books
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactRange))
        {
            if (hit.transform.tag == "Book")
            {
                lookingAtBook = true; 
                interactionText.text = "Press " + interactKey + " to read " + hit.transform.GetComponent<BookValues>().bookName;
            }
            else
            {
                lookingAtBook = false;
            }
        }
        else
        {
            interactionText.text = "";
        }


        if (Input.GetKeyDown(interactKey) && lookingAtBook)
        {
            //Just making a method to organize (maybe complicate things)
            if (!hit.transform.GetComponent<Animator>().GetBool("OpenBook"))
            {
                hit.transform.GetComponent<Animator>().SetBool("OpenBook", true);
            }
            else
            {
                hit.transform.GetComponent<Animator>().SetBool("OpenBook", false);
            }
            
        }
    }

    private void mechanicFlip()
    {
        if (!mechanicToggle)
        {
            //Swap blue with red
            foreach(GameObject blue in blueObjects)
            {
                blue.gameObject.GetComponent<MeshCollider>().enabled = false;
                blue.gameObject.GetComponent<MeshRenderer>().material = blueOff;
            }
            //now for red to turn on
            foreach(GameObject red in redObjects)
            {
                red.gameObject.GetComponent<MeshCollider>().enabled = true;
                red.gameObject.GetComponent<MeshRenderer>().material = redOn;
            }
        }
        else if (mechanicToggle)
        {
            //turn blue on
            foreach (GameObject blue in blueObjects)
            {
                blue.gameObject.GetComponent<MeshCollider>().enabled = true;
                blue.gameObject.GetComponent<MeshRenderer>().material = blueOn;
            }
            //now for red to turn off
            foreach (GameObject red in redObjects)
            {
                red.gameObject.GetComponent<MeshCollider>().enabled = false;
                red.gameObject.GetComponent<MeshRenderer>().material = redOff;
            }
        }
        mechanicToggle = !mechanicToggle;
    }
}
