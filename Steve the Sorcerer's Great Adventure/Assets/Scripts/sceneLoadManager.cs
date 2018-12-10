using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneLoadManager : MonoBehaviour {

   public void loadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void loadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
