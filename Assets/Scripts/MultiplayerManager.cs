using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey || Input.touchCount > 0)
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
        
    }
}
