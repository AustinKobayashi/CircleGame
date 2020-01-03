using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int NumRounds;
    public int NumPlayers;
    public GameObject PlayerPrefab;
    
    private List<GameObject> _players = new List<GameObject>();
    private Dictionary<string, int> _scores = new Dictionary<string, int>();
    private int _curRound = 1;
    
    
    void Awake() {
        SpawnPlayers();
    }


    void Update() {
    }


    //TODO: base spawn location on number of players
    void SpawnPlayers() {
        bool populateDict = _scores.Count == 0;

        for (int i = 0; i < NumPlayers; i++) {
            Vector3 spawnLocation;
            if (i == 0) {
                spawnLocation = new Vector3(-5, Random.Range(-3f, 3f), 0);
            } else {
                spawnLocation = new Vector3(5, Random.Range(-3f, 3f), 0);
            }

            GameObject player = Instantiate(PlayerPrefab, spawnLocation, Quaternion.identity);
            player.GetComponent<Player>().SetGameManager(this);
            player.name = "Player" + i;
            
            if (populateDict)
                _scores.Add(player.name, 0);
            
            if (i == 0) {
                player.GetComponent<SpriteRenderer>().color = Color.red;
                player.GetComponent<Player>().Player1 = true;
            } else {
                player.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            
            _players.Add(player);
        }
    }


    
    void Reset() {
        foreach (GameObject player in _players) {
            Destroy(player);
        }
        _players.Clear();
        SpawnPlayers();
    }



    void EndRound() {
        if (_players.Count < 1)
            Debug.Log("No winner!");
        if (_players.Count == 1) {
            Debug.Log(_players[0].name + " won the round!");
            _scores[_players[0].name]++;
        }

        _curRound++;
        if (_curRound >= NumRounds)
            EndGame();
        else
            Reset();
    }


    void EndGame() {
        int bestScore = 0;
        string winner = "";
        
        foreach(KeyValuePair<string, int> entry in _scores) {
            if (entry.Value > bestScore) {
                bestScore = entry.Value;
                winner = entry.Key;
            }
        }
        
        Debug.Log(winner + " wins!");
    }
    
    
    
    public void Die(GameObject gameObject) {
        _players.Remove(gameObject);
        if (_players.Count <= 1) {
            EndRound();
        }
    }
}
