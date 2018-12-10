using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour {

    //Instruction books (im grabbing form player controller then renaming canvas in this script)
    public PlayerController playerController;
    public GameObject inventoryPanel;
    private Text notificationText;
    [HideInInspector]public int instructionCounter;
    //Naming books
    private string[] bookStory;
    public string[] bookNames;
    private GameObject[] books;
    private int vol = 2;

	// Use this for initialization
	void Start () {
        //bookStory = System.IO.File.ReadAllLines("Assets/Resources/BookStories.txt");
        /* This was for debug
        for(int i =0; i < bookNames.Length; i++)
        {
            Debug.Log("Line in Text Document " + i + ". Reads " + bookNames[i] + "\n");
        }
        */
        books = GameObject.FindGameObjectsWithTag("Book");
        Debug.Log("Number of unique book titles found: " + bookNames.Length + "\n");
        Debug.Log("Number of unique (junk) books found: " + books.Length);
        for(int i = 0; i < books.Length; i++)
        {
            if(i >= bookNames.Length)
            {
                //Easily the ugliest double loop mostrosity you'll ever see
                //POint of this is to cycle through the given book names list and if there are more books in the enviornment than booknames. Then we cycle back through the book names and make new volumes
                int j = 0;

                while (i < books.Length)
                {
                    if (j >= bookNames.Length)
                    {
                        j = 0;
                        vol++;
                    }
                    //Debug.Log("Book(i) " + i + ". Max: " + books.Length);
                    //Debug.Log("Name(j): " + j + ". Max: " + bookNames.Length);
                    books[i].GetComponent<BookValues>().bookName = bookNames[j] + " Vol. " + vol;
                    //Debug.Log("Book Name: " + bookNames[j] + " Vol. " + vol);
                    i++;
                    j++;
                }
                break;
            }
            else
            {
                books[i].GetComponent<BookValues>().bookName = bookNames[i];
                //Debug.Log("Book Name: " + bookNames[i]);
            }

            //For intructions
            notificationText = playerController.notificationText;
            
        }

        for(int i= 0; i < books.Length; i++)
        {
            //assigning the stories here, i could've just done it with the book titles, but Then it would repeat stories 
            //Even though the stories aren't readable, i still want them to be unique for some reason

            //Only 1 line in the document for now, if anyone cares to add more, then this part gets rewritten to acocunt for more lines
            //books[i].GetComponent<BookValues>().bookStory = bookStory[0];
        }

        
        
        //Naming canvas
        for(int i = 0; i < playerController.instructionBooks.Length; i++)
        {
            string temp = "Book " + (i + 1);
            //Old because now we want to be able to pick up the inventory books in an order (we still disable the text here though
            //inventoryPanel.transform.Find(temp).GetComponent<Text>().text = playerController.instructionBooks[i].GetComponent<BookValues>().bookName;
            inventoryPanel.transform.Find(temp).gameObject.SetActive(false);
            //and buttons
            temp = "Button " + (i + 1);
            inventoryPanel.transform.Find(temp).gameObject.SetActive(false);

        }
	}

    public void unlockInstruction()
    {
        //Need to assign the book to the next inventory space
        instructionCounter++;
        string temp = "Book " + (instructionCounter);
        inventoryPanel.transform.Find(temp).GetComponent<Text>().text = playerController.instructionBooks[instructionCounter - 1].GetComponent<BookValues>().bookName;
        inventoryPanel.transform.Find(temp).gameObject.SetActive(true);
        //and buttons
        temp = "Button " + (instructionCounter);
        inventoryPanel.transform.Find(temp).gameObject.SetActive(true);
        StartCoroutine(displayNotifcationText());
    }

    private IEnumerator displayNotifcationText()
    {
        playerController.discoverySound.Play();
        notificationText.text = playerController.currentBook.name + " Added to Inventory. To access Inventory, press '" + playerController.openInventoryKey + "'";
        yield return new WaitForSeconds(8);
        if(notificationText.text != "")
        {
            notificationText.text = "";
        }
    }
}
