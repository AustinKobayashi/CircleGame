using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Effects : MonoBehaviour {

    public float ScreenShakeDuration;
    public float ShakeAmount = 0.7f;
    public float DecreaseFactor = 1.0f;
    public float MaxShakeForce = 40;

    public float CloseDistance = 2;
    public float CloseAngle = 5f;
    
    private List<GameObject> _players = new List<GameObject>();
    private Vector3 _originalPos;
    private Transform _camTransform;
    private float _shakeDuration = 0f;
    private float _shakeForce;
    

    void Awake() {
        _camTransform = Camera.main.transform;
        _originalPos = _camTransform.localPosition;
    }

    

    void Update() {
        if (_players.Count == 0)
            return;
        
        Screenshake();

        if (DetectCollision()) {
            Debug.Log("COOOOOOOOLLLLLLLLLLLLLLLLLL");
        }
    }



    public void StartScreenshake(float shakeForce) {
        _shakeDuration = ScreenShakeDuration;
        _shakeForce = shakeForce;
    }
    
    
    
    void Screenshake() {
        if (_shakeDuration > 0) {
            _camTransform.localPosition = _originalPos + Random.insideUnitSphere * ShakeAmount * _shakeForce / MaxShakeForce;
            _shakeDuration -= Time.deltaTime * DecreaseFactor;
        } else {
            _shakeDuration = 0f;
            _camTransform.localPosition = _originalPos;
        }
    }


    
    bool DetectCollision() {
        List<Tuple<GameObject, GameObject>> closeGameObjects = ClosePlayers();
        
        foreach (Tuple<GameObject, GameObject> goTup in closeGameObjects) {

            GameObject go1 = goTup.Item1;
            GameObject go2 = goTup.Item2;

            if (go1.GetComponent<Player>().IsAttacking() || go2.GetComponent<Player>().IsAttacking()) {
                Vector2 deltaVec = go1.transform.position - go2.transform.position;
                
                Vector2 velVec1 = go1.GetComponent<Rigidbody2D>().velocity;
                Vector2 velVec2 = go1.GetComponent<Rigidbody2D>().velocity;

                if (Vector2.Angle(deltaVec, velVec1) <= CloseAngle || Vector2.Angle(-deltaVec, velVec1) <= CloseAngle)
                    return true;
                
                if (Vector2.Angle(deltaVec, velVec2) <= CloseAngle || Vector2.Angle(-deltaVec, velVec2) <= CloseAngle)
                    return true;
            }
        }
        
        return false;
    }

    
    

    List<Tuple<GameObject, GameObject>> ClosePlayers() {
        List<Tuple<GameObject, GameObject>> closeGameObjects = new List<Tuple<GameObject, GameObject>>();
        
        for (int i = 0; i < _players.Count; i++) {
            for (int j = 0; j < _players.Count; j++) {
                if (i != j) {
                    if (Vector2.Distance(_players[i].transform.position, _players[j].transform.position) <=
                        CloseDistance) {

                        Tuple<GameObject, GameObject> goTup = new Tuple<GameObject, GameObject>(_players[i], _players[j]);
                        closeGameObjects.Add(goTup);
                    }
                }
            }
        }

        return closeGameObjects;
    }
    
    
    
    public void SetPlayers(List<GameObject> players) {
        _players = players;
    }
}
