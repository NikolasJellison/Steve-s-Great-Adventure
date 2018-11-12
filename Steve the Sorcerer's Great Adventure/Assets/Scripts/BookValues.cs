using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookValues : MonoBehaviour {

    public string bookName;
    public Text bookTitleText;
    public string bookText;

    private void Start()
    {
        //Delaying this so that bookmanager can run and name all the books before i assign the text canvas with their names
        //Otherwise the canvas stays blank
        Invoke("nameBook", 5);
    }

    public void nameBook()
    {
        bookTitleText.text = bookName;
    }
}
