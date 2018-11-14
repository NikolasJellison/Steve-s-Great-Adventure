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
    [Header("Place where the book will be held")]
    public Transform bookHold;
    private Transform currentBook; //So i can keep the book with the player and raycasting wont bug out because of bad code
    public float bookSpeed = 3;
    private float step;
    private Vector3 bookOrginalLocation;
    private Quaternion bookOrginalRotation;
    private bool bookMovingToPlayer;
    private bool bookMovingToHome;
    //Current layout is blue = false and red = true. If you want to change it, go to the mechanicFlip() function/method/whatever
    private bool mechanicToggle;
    private GameObject[] blueObjects;
    private GameObject[] redObjects;
    //for books
    private RaycastHit hit;
    private bool lookingAtBook;
    public Transform bookLookAt;    //Because the model is all off and my animation is poorly planned, have to make the book lootat a empty game object away from the player

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
        //For moving the book
        step = bookSpeed * Time.deltaTime;
        //So i don't use get component in update
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
            //Edge case where you look at book but look away and slight error
            lookingAtBook = false;
        }

        if (Input.GetKeyDown(interactKey) && lookingAtBook)
        {
            currentBook = hit.transform;
            //Just making a method to organize (maybe complicate things)
            if (!hit.transform.GetComponent<Animator>().GetBool("OpenBook"))
            {
                Debug.Log("HIT");
                bookOrginalLocation = hit.transform.position;
                bookOrginalRotation = hit.transform.rotation;
                Debug.Log(bookOrginalLocation);
                //bookOrginalRotation = hit.transform.q
                //Below this is new code for attempting to move book to hand and whatnot
                while (Vector3.Distance(transform.position, hit.transform.position) < .3)
                {
                    hit.transform.position = Vector3.MoveTowards(hit.transform.position, transform.position, step);
                }
                hit.transform.GetComponent<Animator>().SetBool("OpenBook", true);
                bookMovingToPlayer = true;
            }
            else if(bookMovingToPlayer)
            {
                //Need to let player close the book before it gets sent back, so we go to coroutine
                StartCoroutine(returnBook());
            }
        }

        //Keep the book with the player
        if (bookMovingToPlayer)
        {
            currentBook.position = Vector3.MoveTowards(currentBook.position, bookHold.position, step);
            currentBook.LookAt(bookLookAt);
        }
        if (bookMovingToHome)
        {
            if(Vector3.Distance(currentBook.position, bookOrginalLocation) > .1)
            {
                //Debug.Log("we got it ");
                //Debug.Log(currentBook.position);
                //Debug.Log("OG" + bookOrginalLocation.position);
                currentBook.position = Vector3.MoveTowards(currentBook.position, bookOrginalLocation, step);
                currentBook.rotation = bookOrginalRotation;
                //Debug.Log(bookOrginalLocation.position);
            }
            else
            {
                bookMovingToHome = false;
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

    private IEnumerator returnBook()
    {
        hit.transform.GetComponent<Animator>().SetBool("OpenBook", false);
        //We change this waitforseconds if we changethe animation length for closing the book
        yield return new WaitForSeconds(2);
        bookMovingToPlayer = false;
        //Moving the book back to it's place hopefully
        bookMovingToHome = true;
    }
}
