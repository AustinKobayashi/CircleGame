using UnityEngine;
using System.Threading.Tasks;

public class Effectsv2 : MonoBehaviour {
    public float ScreenShakeDuration;
    public float ShakeAmount = 0.7f;
    public float DecreaseFactor = 1.0f;
    public float MaxShakeForce = 40;

    public float CloseDistance = 2;
    public float CloseAngle = 5f;

    public float SlowDownPercentage;
    public float SlowDownStep;
    public float SlowDownDuration;
    private Vector3 _originalPos;
    private Transform _camTransform;
    private float _shakeDuration = 0f;
    private float _shakeForce;
    private bool _slowDown;
    private bool _startSlowdown = false;
    private float _slowDownTimer = 0;
    public float ZoomTime;

    void Awake() {
        _camTransform = Camera.main.transform;
        _originalPos = _camTransform.localPosition;
    }
    private void Update() {
        if (_slowDown){

        }
    }
    public void StartScreenshake() {
        _shakeDuration = ScreenShakeDuration;
        _shakeForce = 5;
        Screenshake();
    }
    async void Screenshake() {
        while(_shakeDuration > 0){
            _camTransform.localPosition = _originalPos + Random.insideUnitSphere * _shakeForce / MaxShakeForce;
            _shakeDuration -= Time.deltaTime * DecreaseFactor;
            await Task.Delay(1);
        }
        _shakeDuration = 0f;
        _camTransform.localPosition = _originalPos;
    }

    async void ZoomIn(){
        _slowDown = true;
        var startTime = Time.time;
        while (_slowDown && Camera.main.orthographicSize > 3){
            float t = (Time.time - startTime) / ZoomTime;
            Camera.main.orthographicSize = Mathf.SmoothStep(5, 3, t);
            await Task.Delay(1);
        }
    }
    async void ZoomOut(){
        _slowDown = false;
        var startTime = Time.time;
        while (!_slowDown && Camera.main.orthographicSize < 5){
            float t = (Time.time - startTime) / ZoomTime;
            Camera.main.orthographicSize = Mathf.SmoothStep(3, 5, t);
            await Task.Delay(1);
        }
    }
    public void cameraZoom(Vector2 pos1, Vector2 pos2){
        Vector2 normalized = (pos1 + pos2).normalized;
        Vector3 cameraPos = new Vector3(normalized.x, normalized.y, -10);
        _camTransform.position = cameraPos;
        Time.timeScale = 0.5f;
        if (!_slowDown){
            ZoomIn();
        }
    }
    public void resetCamera() {
        if (_slowDown) ZoomOut();
        Time.timeScale = 1;
        _camTransform.localPosition = _originalPos;
    }
}