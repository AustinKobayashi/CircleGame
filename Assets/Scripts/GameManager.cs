using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private bool _resetPossible;
    private bool _gameOver;
    private bool _cooldown;
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

    private Effectsv2 _effects2;
    
    void Awake() {
        _effects = GetComponent<Effects>();
        _effects2 = GetComponent<Effectsv2>();
        _winText = GameObject.FindGameObjectWithTag("winText").GetComponent<UnityEngine.UI.Text>();
        SpawnPlayers();
    }


    void Update() {
        if (!_cooldown && _resetPossible 
                && (Input.GetKey(KeyCode.Space) || Input.touchCount > 0)){
            Reset();
        }
        if (!_cooldown && _gameOver 
                && (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)){
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
            player.GetComponent<Player>().setEffects(_effects2);
            
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
        _effects2.resetCamera();
        _winText.enabled = false;
        _resetPossible = false;
        foreach (GameObject player in _players) {
            Destroy(player);
        }
        // _effects.StopSlowDown();
        _players.Clear();
        SpawnPlayers();
    }

    private string writeWinText(string text, bool end = false) {
        string scores = _scores.Aggregate("", (acc, score) => acc += $"{score.Key}: {score.Value}".PadRight(20)).TrimEnd();
        string resettext = end ? "Tap or Press Space to end game" : "Tap or Press Space to restart";
        return $"{text}\n{scores}\n{resettext}";
    }


    private async void startCoolDown() {
        _cooldown = true;
        await Task.Delay(1000);
        _cooldown = false;
    }
    void EndRound() {
        _effects2.StartScreenshake();
        startCoolDown();
        if (_players.Count < 1){    
            _winText.enabled = true;
            _winText.text = writeWinText("Tie");
        }else if (_players.Count == 1) {
            _scores[_players[0].name]++;
            if (NumRounds - _scores["Player0"] < NumRounds / 2.0 ||
                    NumRounds - _scores["Player1"] < NumRounds / 2.0){
                EndGame();
                return;
            }
            _winText.enabled = true;
            _winText.text = writeWinText($"{_players[0].name} wins round {_curRound}!");
        }
        _curRound++;
        if (_curRound > NumRounds){
            EndGame();
        }else {
            _resetPossible = true;
        }
    }


    void EndGame() {        
        string winner = _scores["Player0"] > _scores["Player1"] ? "Player0" : "Player1";

        _gameOver = true;
        _winText.enabled = true;
        _winText.text = writeWinText($"{winner} wins!", true);
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
    private void GoToMenu(){
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
	}
	private void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus){
            GoToMenu();
		}
	}
}









