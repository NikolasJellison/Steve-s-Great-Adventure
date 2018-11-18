using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    //I Understand that this isn't really a player controller, more of a player manager, but i don't want to change the name

    //We use tags to distinguish which objects can get toggled in this game

    //Key for the mechanic
    [Header("Reference game manager")]
    public GameManager gameManager;
    [Tooltip("Which key activates the toggle of the environment")]public KeyCode mechanicKey = KeyCode.E;
    [Tooltip("Which key activates interaction (books)")] public KeyCode interactKey = KeyCode.Q;
    [Tooltip("Key for shooting ice-staff")] public KeyCode iceStaffShoot = KeyCode.Mouse0;
    [Tooltip("Key for shooting fire-staff")] public KeyCode fireStaffShoot = KeyCode.Mouse1;
    [Header("Materials that will get swaped")]
    public Material blueOn;
    public Material blueOff;
    public Material redOn;
    public Material redOff;
    [Header("Get these from 'Canvas.UI' prefab")]
    public Text interactionText;
    public Text notificationText;
    [Tooltip("range for interaction")]
    public float interactRange = 10;
    public float notificationLength = 10;
    [Header("Arm stuff (in player, under camera)")]
    public GameObject arm;
    public Transform hand;
    public Transform staffHold;
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
    //Staffs
    [Header("Projectiles")]
    public GameObject iceProjectile;
    public GameObject fireProjectile;
    public float projectileSpeed = 300;
    private bool hasIceStaff;
    private bool hasFireStaff;

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

        //Disable arm for start
        arm.SetActive(false);
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

        //Raycasting for books (include the arm active thing so you don't pick up a book while holding the artifact
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactRange) && !arm.activeSelf)
        {
            if (hit.transform.tag.Contains("Book"))
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

        if (Input.GetKeyDown(interactKey) && lookingAtBook && !arm.activeSelf)
        {
            currentBook = hit.transform;
            //Just making a method to organize (maybe complicate things)
            if (!hit.transform.GetComponent<Animator>().GetBool("OpenBook"))
            {
                //Debug.Log("HIT");
                bookOrginalLocation = hit.transform.position;
                bookOrginalRotation = hit.transform.rotation;
                //Debug.Log(bookOrginalLocation);
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

        //Projectiles
        if(Input.GetKeyDown(iceStaffShoot) && hasIceStaff)
        {
            shootProjectile("Ice");
        }
        if (Input.GetKeyDown(fireStaffShoot) && hasFireStaff)
        {
            shootProjectile("Fire");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Lava")
        {
            gameManager.respawn();
        }
        else if (other.tag == "Staff-Ice")
        {
            arm.SetActive(true);
            Instantiate(Resources.Load("Staff-Ice-Hold"), staffHold);
            Destroy(other.gameObject);
            hasIceStaff = true;
        }
        else if (other.tag == "Staff-Fire")
        {
            arm.SetActive(true);
            Instantiate(Resources.Load("Staff-Fire-Hold"), staffHold);
            Destroy(other.gameObject);
            hasFireStaff = true;
        }
    }
    
    private void shootProjectile(string type)
    {
        GameObject tempObject;
        
        switch (type)
        {
            case "Ice":
                tempObject = Instantiate(iceProjectile, transform.position + transform.forward + Vector3.up * .9f, transform.rotation);
                tempObject.GetComponent<Rigidbody>().AddForce(transform.forward * projectileSpeed);
                break;

            case "Fire":
                tempObject = Instantiate(fireProjectile, transform.position + transform.forward + Vector3.up * .9f, transform.rotation);
                tempObject.GetComponent<Rigidbody>().AddForce(transform.forward * projectileSpeed);
                break;
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
        //Sending junk books back
        if(currentBook.tag == "Book")
        {
            hit.transform.GetComponent<Animator>().SetBool("OpenBook", false);
            //We change this waitforseconds if we changethe animation length for closing the book
            yield return new WaitForSeconds(2);
            bookMovingToPlayer = false;
            //Moving the book back to it's place hopefully
            bookMovingToHome = true;
        }
        else
        {
            //send a string in there because we are destorying the book itself so can't read the tag
            StartCoroutine(obtainElement(currentBook.tag));
        }
    }

    private IEnumerator obtainElement(string bookTag)
    {
        //time to close the book and maybe particle effects here at some point?
        hit.transform.GetComponent<Animator>().SetBool("OpenBook", false);
        //We change this waitforseconds if we changethe animation length for closing the book
        yield return new WaitForSeconds(2);
        bookMovingToPlayer = false;
        //Destroy book
        Destroy(currentBook.gameObject);

        if (bookTag.Contains("Fire"))
        {
            notificationText.text = "You have learned the power of the FIRE element :D";
            Instantiate(Resources.Load("FireArtifact"), hand);
            //Debug.Log("Did the instantiaion");
        }
        else if (bookTag.Contains("Ice"))
        {
            //Ice element stuff
            notificationText.text = "You have learned the power of the ICE element :D";
            Instantiate(Resources.Load("IceArtifact"), hand);
        }
        else if (bookTag.Contains("Earth"))
        {
            //Earth element stuff
            notificationText.text = "You have learned the power of the EARTH element :D";
            Instantiate(Resources.Load("EarthArtifact"), hand);
        }
        else if (bookTag.Contains("Wind"))
        {
            //Wind element stuff
            notificationText.text = "You have learned the power of the WIND element :D";
            Instantiate(Resources.Load("WindArtifact"), hand);
        }

        arm.SetActive(true);

        yield return new WaitForSeconds(notificationLength);
        notificationText.text = "";
    }
}
