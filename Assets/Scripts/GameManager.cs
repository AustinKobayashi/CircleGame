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
    public Vector3[] Spawns = new Vector3[] {
        new Vector3(-5, 0, 0),
        new Vector3(5, 0, 0),
        new Vector3(-5, 0, 0),
        new Vector3(-5, 0, 0),
        new Vector3(-5, 0, 0),
        new Vector3(-5, 0, 0),
        new Vector3(-5, 0, 0),
        new Vector3(-5, 0, 0)
    };

    public Vector4[] Colors;
        
    private Effects _effects;
    
    void Awake() {
        _effects = GetComponent<Effects>();
        SpawnPlayers();
    }


    void Update() {
    }


    //TODO: base spawn location on number of players
    void SpawnPlayers() {
        bool populateDict = _scores.Count == 0;

        for (int i = 0; i < NumPlayers; i++) {
            Vector3 spawnLocation = Spawns[i];

            GameObject player = Instantiate(PlayerPrefab, spawnLocation, Quaternion.identity);
            player.GetComponent<Player>().SetGameManager(this);
            player.GetComponent<SpriteRenderer>().color = Colors[i];
            player.name = "Player" + i;
            
            if (populateDict)
                _scores.Add(player.name, 0);
            
            if (i == 0) {
                player.GetComponent<Player>().Player1 = true;
            } 
            
            _players.Add(player);
        }
        
        _effects.SetPlayers(_players);
    }


    
    void Reset() {
        foreach (GameObject player in _players) {
            Destroy(player);
        }
        _effects.StopSlowDown();
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



    
    public void DamageCalculation(GameObject go1, GameObject go2, float Speed) {

        Rigidbody2D rigid1 = go1.GetComponent<Rigidbody2D>();
        Rigidbody2D rigid2 = go2.GetComponent<Rigidbody2D>();
        
        _effects.StartScreenshake(Mathf.Abs(rigid1.velocity.magnitude - rigid2.velocity.magnitude));
        
        if (rigid1.velocity.magnitude > rigid2.velocity.magnitude) {
            int damage = Mathf.RoundToInt(rigid1.velocity.magnitude / Speed);
            if (damage < 1) damage = 1;
            go2.GetComponent<Player>().TakeDamage(damage);
            Debug.Log(go1.name + " damaged " + go2.gameObject.name + " for " + damage);

        } else {
            int damage = Mathf.RoundToInt(rigid2.velocity.magnitude / Speed);
            if (damage < 1) damage = 1;
            go1.GetComponent<Player>().TakeDamage(damage);
            Debug.Log(go2.name + " damaged " + go1.gameObject.name + " for " + damage);

        }
    }
}









