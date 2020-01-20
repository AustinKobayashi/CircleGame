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

    private bool _startSlowdown = false;
    private float _slowDownTimer = 0;

    void Awake() {
        _camTransform = Camera.main.transform;
        _originalPos = _camTransform.localPosition;
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

    public void cameraZoom(Vector2 pos1, Vector2 pos2){

    }
}