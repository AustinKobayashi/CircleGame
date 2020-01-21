using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private bool _roundOver;
    private bool _gameOver;
    private bool _cooldown;
    public int NumRounds;
    public int NumPlayers;
    public GameObject PlayerPrefab;
    public Text WinText;
    
    private List<GameObject> _players = new List<GameObject>();
    private Dictionary<string, int> _scores = new Dictionary<string, int>();
    private int _curRound = 1;
    
    public Vector3[] Spawns;

    public Vector4[] Colors;
        
    private Effects _effects;

    private Effectsv2 _effects2;
    
    
    
    void Awake() {
        _effects = GetComponent<Effects>();
        _effects2 = GetComponent<Effectsv2>();
        SpawnPlayers();
    }


    
    void Update() {
        if (!_cooldown && _roundOver && (Input.GetKey(KeyCode.Space) || Input.touchCount > 0)) {
            Reset();
        }
        
        if (!_cooldown && _gameOver && (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)){ 
            GoToMenu();
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
            player.GetComponent<Player>().SetEffects(_effects2);

            if (populateDict)
                _scores.Add(player.name, 0);

            if (i == 0) {
                var playerScript = player.GetComponent<Player>();
                playerScript.Player1 = true;
                playerScript.SetInitialAngle(180);
            }

            _players.Add(player);
        }
        // _effects.SetPlayers(_players);
    }


    
    void Reset() {
        _effects2.ResetCamera();
        WinText.enabled = false;
        _roundOver = false;
        foreach (GameObject player in _players) {
            Destroy(player);
        }
        _players.Clear();
        SpawnPlayers();
    }

    
    
    private string WriteWinText(string text, bool end = false) {
        string scores = _scores.Aggregate("", (acc, score) => acc += $"{score.Key}: {score.Value}".PadRight(20)).TrimEnd();
        string resetText = end ? "Tap or Press Space to end game" : "Tap or Press Space to restart";
        return $"{text}\n{scores}\n{resetText}";
    }


    
    private async void StartCoolDown() {
        _cooldown = true;
        await Task.Delay(1000);
        _cooldown = false;
    }
    
    
    
    void EndRound() {
        _effects2.StartScreenshake();
        StartCoolDown();
        
        if (_players.Count < 1){    
            WinText.enabled = true;
            WinText.text = WriteWinText("Tie");
            
        } else if (_players.Count == 1) {
            _scores[_players[0].name]++;
            if (_scores["Player0"] >= Mathf.Ceil(NumRounds / 2f) || _scores["Player1"] >= Mathf.Ceil(NumRounds / 2f)) {
                EndGame();
                return;
            }
            WinText.enabled = true;
            WinText.text = WriteWinText($"{_players[0].name} wins round {_curRound}!");
        }
        
        _curRound++;
        
        if (_curRound > NumRounds){
            EndGame();
        }else {
            _roundOver = true;
        }
    }


    
    void EndGame() {        
        string winner = _scores["Player0"] > _scores["Player1"] ? "Player0" : "Player1";

        _gameOver = true;
        WinText.enabled = true;
        WinText.text = WriteWinText($"{winner} wins!", true);
    }
    
    
    
    public void Die(GameObject go) {
        _players.Remove(go);
        Destroy(go);
        if (_players.Count <= 1) {
            EndRound();
        }
    }

    
    
    private void GoToMenu(){
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
	}
    
    
    
	private void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus){
            GoToMenu();
		}
	}
}









