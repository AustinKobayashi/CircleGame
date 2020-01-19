﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private bool _resetPossible;
    public int NumRounds;
    public int NumPlayers;
    public GameObject PlayerPrefab;
    
    private UnityEngine.UI.Text _winText;
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
        _winText = GameObject.FindGameObjectWithTag("winText").GetComponent<UnityEngine.UI.Text>();
        SpawnPlayers();
    }


    void Update() {
        if (_resetPossible && Input.GetKey(KeyCode.R)){
            Reset();
        }
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
        
        // _effects.SetPlayers(_players);
    }


    
    void Reset() {
        _winText.enabled = false;
        _resetPossible = false;
        foreach (GameObject player in _players) {
            Destroy(player);
        }
        // _effects.StopSlowDown();
        _players.Clear();
        SpawnPlayers();
    }



    void EndRound() {
        _winText.enabled = true;
        _winText.text = $"{_players[0].name} is the winner!";
        _curRound++;
        _resetPossible = true;
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
        Destroy(gameObject.gameObject);
        if (_players.Count <= 1) {
            EndRound();
        }
    }



    
    public void DamageCalculation(GameObject go1, GameObject go2, float Speed) {

        Rigidbody2D rigid1 = go1.GetComponent<Rigidbody2D>();
        Rigidbody2D rigid2 = go2.GetComponent<Rigidbody2D>();
        
        // _effects.StartScreenshake(Mathf.Abs(rigid1.velocity.magnitude - rigid2.velocity.magnitude));
        
        
        // if (rigid1.velocity.magnitude > rigid2.velocity.magnitude) {
        //     int damage = Mathf.RoundToInt(rigid1.velocity.magnitude / Speed);
        //     if (damage < 1) damage = 1;
        //     go2.GetComponent<Player>().TakeDamage(damage);
        //     Debug.Log(go1.name + " damaged " + go2.gameObject.name + " for " + damage);

        // } else {
        //     int damage = Mathf.RoundToInt(rigid2.velocity.magnitude / Speed);
        //     if (damage < 1) damage = 1;
        //     go1.GetComponent<Player>().TakeDamage(damage);
        //     Debug.Log(go2.name + " damaged " + go1.gameObject.name + " for " + damage);

        // }
    }
}









