using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public Button LocalPlayerButton;
    
    public Button MultiplayerButton;
    public string LocalScene;
    public string MultiplayerScene;
    // Start is called before the first frame update
    void Start()
    {
		LocalPlayerButton.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(LocalScene);
        });
        MultiplayerButton.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(MultiplayerScene);
        });
    }
}
