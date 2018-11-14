using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour {

    private string[] bookNames;
    private GameObject[] books;
    private int vol = 2;

	// Use this for initialization
	void Start () {
        bookNames = System.IO.File.ReadAllLines("Assets/Resources/BookNames.txt");
        for(int i =0; i < bookNames.Length; i++)
        {
            Debug.Log("Line in Text Document " + i + ". Reads " + bookNames[i] + "\n");
        }

        books = GameObject.FindGameObjectsWithTag("Book");
        Debug.Log(bookNames.Length);
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
            
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
