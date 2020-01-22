using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CircleNetworkManager : NetworkManager
{
    public UnityEngine.UI.Text TitleText;
    public string DefaultText;
    // Start is called before the first frame update
    //Host code
    public override void OnStartHost() {
        TitleText.text = "Waiting for players..";
    }
    
    //Client code
    public override void OnStartClient(NetworkClient client) {
        if (NetworkServer.connections.Count == 0)
            TitleText.text = "Attempting to connect..";
    }
    public override void OnStopClient() {
        TitleText.text = DefaultText;
    }
        // Client callbacks
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        if (NetworkServer.connections.Count == 0)
            TitleText.text = "Connected!\nWaiting for host to start..";
    }
    public override void OnClientDisconnect(NetworkConnection conn) {
        StopClient();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Multiplayer");
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
