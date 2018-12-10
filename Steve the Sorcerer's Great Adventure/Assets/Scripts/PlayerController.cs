using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    //I Understand that this isn't really a player controller, more of a player manager, but i don't want to change the name

    //We use tags to distinguish which objects can get toggled in this game

    #region Variables
    //Key for the mechanic
    [Header("Reference game manager")]
    public GameManager gameManager;
    public BookManager bookManager;
    public Camera cam;
    public Image crosshair;
    [Tooltip("Which key activates the toggle of the environment")]public KeyCode mechanicKey = KeyCode.E;
    [Tooltip("Which key activates interaction (books)")] public KeyCode interactKey = KeyCode.Q;
    [Tooltip("Key for opening inventory")] public KeyCode openInventoryKey = KeyCode.T;
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
    //Library
    public GameObject[] instructionBooks;
    public Transform bookHold;
    [HideInInspector]public Transform currentBook; //So i can keep the book with the player and raycasting wont bug out because of bad code (public beacuse i want to add book title to notication text and that is done in the book manager script
    public float bookSpeed = 3;
    private float step;
    private Vector3 bookOrginalLocation;
    private Quaternion bookOrginalRotation;
    private bool bookMovingToPlayer;
    private bool bookMovingToHome;
    private bool holdingBook;
    private bool bookClosing;
    public GameObject inventoryPanel;
    //Current layout is blue = false and red = true. If you want to change it, go to the mechanicFlip() function/method/whatever
    private bool mechanicToggle;
    private GameObject[] blueObjects;
    private GameObject[] redObjects;
    private GameObject[] blueTexts;
    private GameObject[] redTexts;
    private GameObject[] blueEnemyProjectile;
    private GameObject[] redEnemyProjectile;
    //for books
    private RaycastHit hit;
    private bool lookingAtBook;
    public Transform bookLookAt;    //Because the model is all off and my animation is poorly planned, have to make the book lootat a empty game object away from the player
    private Ray interactRay;
    //Staffs
    [Header("Projectiles")]
    public GameObject iceProjectile;
    public GameObject fireProjectile;
    public float projectileSpeed = 300;
    private bool hasIceStaff;
    private bool hasFireStaff;
    [Header("Sounds")]
    public AudioSource discoverySound;
    public AudioSource fireballSound;
    //for damage
    public Image[] lifes;
    private int lifeCount;
    #endregion  

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
        //For library text
        if (blueTexts == null)
        {
            blueTexts = GameObject.FindGameObjectsWithTag("Blue-Text");
        }
        if(redTexts == null)
        {
            redTexts = GameObject.FindGameObjectsWithTag("Red-Text");
        }

        //Need to run it once so everything gets a material and whatnot
        mechanicFlip();
        
        //Disable arm for start
        arm.SetActive(false);
        //Diable inventory Pannel
        inventoryPanel.SetActive(false);
        //lives (array)
        lifeCount = lifes.Length - 1;
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
        //For moving the book
        step = bookSpeed * Time.deltaTime;

        //Inventory Pause
        if (Input.GetKeyDown(openInventoryKey))
        {
            if (!inventoryPanel.activeSelf)
            {
                openInventory();
            }
            else
            {
                closeInventory();
            }
        }

        if (Input.GetKeyDown(mechanicKey) && Time.timeScale !=0)
        {
            mechanicFlip();
        }

        //better raycasting for book
        interactRay = cam.ScreenPointToRay(Input.mousePosition);
        //Reseting it so it is more responsive because my code sucks
        interactionText.text = "";
        //Raycasting for books (include the arm active thing so you don't pick up a book while holding the artifact
        if (Physics.Raycast(interactRay, out hit, interactRange) && !arm.activeSelf && !bookMovingToHome && !bookClosing)
        {
            if (hit.transform.tag.Contains("Book"))
            {
                lookingAtBook = true;
                if (!holdingBook && !bookMovingToHome)
                {
                    interactionText.text = "Press " + interactKey + " to read " + hit.transform.GetComponent<BookValues>().bookName;
                }
                else if( hit.transform.name == currentBook.name)
                {
                    interactionText.text = "Press " + interactKey + " to close " + hit.transform.GetComponent<BookValues>().bookName;
                } 
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

        if (Input.GetKeyDown(interactKey) && lookingAtBook && !arm.activeSelf && !bookMovingToHome && !bookClosing)
        {
            if (!holdingBook)
            {
                currentBook = hit.transform;
                InspectBook();
            }
            else if (holdingBook)
            {
                //Update inventory with current hint 
                if (currentBook.tag.Contains("Instruction-OG"))
                {
                    currentBook.gameObject.tag = "Book-Instruction";
                    instructionBooks[bookManager.instructionCounter] = currentBook.gameObject;
                    bookManager.unlockInstruction();
                }
                //Need to let player close the book before it gets sent back, so we go to coroutine
                StartCoroutine(returnBook());
            }
            /*
            //Just making a method to organize (maybe complicate things)
            if (!hit.transform.GetComponent<Animator>().GetBool("OpenBook"))
            {
                //Debug.Log("HIT");
                bookOrginalLocation = hit.transform.position;
                bookOrginalRotation = hit.transform.rotation;
                //Debug.Log(bookOrginalLocation);
                //bookOrginalRotation = hit.transform.q
                //Better way to do this for sure(stopping floating book)
                if (currentBook.tag.Contains("Instruction-OG"))
                {
                    //Stop floating of the book if it is doing that
                    
                    currentBook.GetComponent<Animator>().SetBool("BookFloat", false);
                }
                //Below this is new code for attempting to move book to hand and whatnot
                while (Vector3.Distance(transform.position, currentBook.position) > .03)
                {
                    currentBook.position = Vector3.MoveTowards(currentBook.position, transform.position, step);
                    Debug.Log("IN the thing2 " + Vector3.Distance(transform.position, currentBook.position) + currentBook.name + currentBook.position + transform.position);

                }
                hit.transform.GetComponent<Animator>().SetBool("OpenBook", true);
                bookMovingToPlayer = true;
            }
            else if(bookMovingToPlayer)
            {
                //Update inventory with current hint 
                if (currentBook.tag.Contains("Instruction-OG"))
                {
                    bookManager.unlockInstruction();
                }
                //Need to let player close the book before it gets sent back, so we go to coroutine
                StartCoroutine(returnBook());
            }
            */
            //Determine if player is now holding book or got rid of one
            holdingBook = !holdingBook;
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

    private void InspectBook()
    {
        if (currentBook.tag == "Book-Instruction-OG")
        {
            //Stop floating of the book if it is doing that
            //Stop it from jumping around
            if(currentBook.GetComponent<BookFloat>() != null)
            currentBook.GetComponent<BookFloat>().enabled = false;
        }

        //Store book inintial place
        bookOrginalLocation = hit.transform.position;
        bookOrginalRotation = hit.transform.rotation;

        //Move book into player
        while (Vector3.Distance(transform.position, currentBook.position) < .3)
        {
            currentBook.position = Vector3.MoveTowards(currentBook.position, transform.position, step);
        }
        //Open the book
        currentBook.GetComponent<Animator>().SetBool("OpenBook", true);
        //Used in update function to keep the book with the player
        bookMovingToPlayer = true;
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
            //Player Discovery sound
            discoverySound.Play();
        }
        else if (other.tag == "Staff-Fire")
        {
            arm.SetActive(true);
            Instantiate(Resources.Load("Staff-Fire-Hold"), staffHold);
            Destroy(other.gameObject);
            hasFireStaff = true;
            //Player Discovery sound
            discoverySound.Play();
        }
    }
    
    private void shootProjectile(string type)
    {
        GameObject tempObject;
        //Hvae the issue where the projectiles aren't launching from the staff and it looks weird
        //The projectiles were hitting the 'arm' and the ball on the temp ice staffs (both are now tagged as player)
        switch (type)
        {
            case "Ice":
                tempObject = Instantiate(iceProjectile, staffHold.transform.position - staffHold.transform.forward + Vector3.up * .9f, staffHold.transform.rotation);
                tempObject.GetComponent<Rigidbody>().AddForce(-staffHold.transform.forward * projectileSpeed);
                break;

            case "Fire":
                tempObject = Instantiate(fireProjectile, staffHold.transform.position - staffHold.transform.forward + Vector3.up * .9f, staffHold.transform.rotation);
                tempObject.GetComponent<Rigidbody>().AddForce(-staffHold.transform.forward * projectileSpeed);
                //fireball sound
                fireballSound.Play();
                break;
        }
    }

    private void mechanicFlip()
    {

        //Scan for enemy projectiles
        blueEnemyProjectile = null;
        blueEnemyProjectile = GameObject.FindGameObjectsWithTag("Enemy-Projectile-Blue");
        redEnemyProjectile = null;
        redEnemyProjectile = GameObject.FindGameObjectsWithTag("Enemy-Projectile-Red");

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
            //Library
            foreach(GameObject blueTxt in blueTexts)
            {
                blueTxt.SetActive(false);
            }
            foreach(GameObject redTxt in redTexts)
            {
                redTxt.SetActive(true);
            }
            //Projectiles
            foreach(GameObject blueE in blueEnemyProjectile)
            {
                blueE.transform.Find("pSphere6").GetComponent<MeshRenderer>().material = blueOff;
            }
            foreach(GameObject redE in redEnemyProjectile)
            {
                redE.transform.Find("pSphere6").GetComponent<MeshRenderer>().material = redOn;
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
            //Library
            foreach (GameObject blueTxt in blueTexts)
            {
                blueTxt.SetActive(true);
            }
            foreach (GameObject redTxt in redTexts)
            {
                redTxt.SetActive(false);
            }
            //Projectiles
            foreach (GameObject blueE in blueEnemyProjectile)
            {
                blueE.transform.Find("pSphere6").GetComponent<MeshRenderer>().material = blueOn;
            }
            foreach (GameObject redE in redEnemyProjectile)
            {
                redE.transform.Find("pSphere6").GetComponent<MeshRenderer>().material = redOff;
            }
        }
        mechanicToggle = !mechanicToggle;
    }

    private IEnumerator returnBook()
    {
        //Sending junk books back
        if(currentBook.tag == "Book")
        {
            bookClosing = true;
            hit.transform.GetComponent<Animator>().SetBool("OpenBook", false);
            //We change this waitforseconds if we changethe animation length for closing the book
            yield return new WaitForSeconds(2);
            bookClosing = false;
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
        bookClosing = true;
        hit.transform.GetComponent<Animator>().SetBool("OpenBook", false);
        //We change this waitforseconds if we changethe animation length for closing the book
        yield return new WaitForSeconds(2);
        bookClosing = false;
        bookMovingToPlayer = false;

        //Figure out which book it is to give the player some info
        if (currentBook.tag.Contains("Instruction"))
        {
            currentBook.GetComponent<Animator>().SetBool("BookFloat", false);
            currentBook.gameObject.SetActive(false);
            yield break;
        }
        else if (bookTag.Contains("Fire"))
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
        //Destroy book
        if (currentBook.gameObject.activeSelf)
        {
            Destroy(currentBook.gameObject);
        }
        //Play Sound
        discoverySound.Play();

        arm.SetActive(true);

        yield return new WaitForSeconds(notificationLength);
        notificationText.text = "";
    }

    public void openInventory()
    {
        Time.timeScale = 0;
        inventoryPanel.SetActive(true);
        crosshair.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void closeInventory()
    {
        Time.timeScale = 1;
        inventoryPanel.SetActive(false);
        crosshair.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void openInstructionBook(int type)
    {
        if (holdingBook)
        {
            //So as it  stands, if you try to open an instruction book while reading a book, nothing happens. Which is good, but i don't know why
            //But it stays cause it does the job
            returnBook();
            while (Vector3.Distance(currentBook.position, bookOrginalLocation) > .1)
            {
                return;
            }
        }
        holdingBook = true;
        //currentBook = Instantiate(instructionBooks[type],hand.position, transform.rotation).transform;
        currentBook = instructionBooks[type].transform;
        currentBook.position = hand.position;
        currentBook.gameObject.SetActive(true);
        currentBook.transform.GetComponent<Animator>().SetBool("OpenBook", true);
        bookMovingToPlayer = true;
    }

    public void playerHit(int type)
    {
        //1 = Ice, 2 = Fire
        //Mechanic toggle: true = blue, false = red

        if (mechanicToggle && type == 2)
        {
            takeDamage();
            Debug.Log("Took damage");
        }
        else if(!mechanicToggle && type == 1)
        {
            takeDamage();
            Debug.Log("Took damage");
        }

    }

    private void takeDamage()
    {
        lifes[lifeCount].enabled = false;
        lifeCount--;
        if(lifeCount <= 0)
        {
            gameManager.respawn();
        }
    }
}
