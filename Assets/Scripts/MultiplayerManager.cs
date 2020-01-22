using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviour
{
    public Button BackButton;
    public GameObject NetManager;
    // Start is called before the first frame update
    void Start()
    {
        BackButton.onClick.AddListener(() => {
            Destroy(NetManager);
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
        });
    }
}
