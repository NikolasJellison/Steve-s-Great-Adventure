using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour {

    private string[] bookNames;
    private GameObject[] books;

	// Use this for initialization
	void Start () {
        bookNames = System.IO.File.ReadAllLines("Assets/Resources/BookNames.txt");
        for(int i =0; i < bookNames.Length; i++)
        {
            Debug.Log("Line in Text Document " + i + ". Reads " + bookNames[i] + "\n");
        }

        books = GameObject.FindGameObjectsWithTag("Book");
        
        for(int i = 0; i < books.Length; i++)
        {
            books[i].GetComponent<BookValues>().bookName = bookNames[i];
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
