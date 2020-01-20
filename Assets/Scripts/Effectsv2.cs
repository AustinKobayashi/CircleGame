using UnityEngine;
using System.Threading.Tasks;

public class Effectsv2 : MonoBehaviour {
    public float SloMoDuration;
    public float ZoomSpeed;
    public float SloMoAmount;
    public float ScreenShakeDuration;
    public float DecreaseFactor = 1.0f;
    public float MaxShakeForce = 40;

    private Vector3 _originalPos;
    private Transform _camTransform;
    private float _shakeDuration = 0f;
    private float _shakeForce;
    [HideInInspector]
    public bool SlowDown;
    private float _origFixedTime;


    public int getMaxSlowMo(){
        return (int)(SloMoDuration * 1000);
    }
    void Awake() {
        _camTransform = Camera.main.transform;
        _originalPos = _camTransform.localPosition;
        _origFixedTime = Time.fixedDeltaTime;
    }
    private void Update() {
        Time.fixedDeltaTime = this._origFixedTime * Time.timeScale;
    }
    public void StartScreenshake() {
        _shakeDuration = ScreenShakeDuration;
        _shakeForce = 1;
        Screenshake();
    }
    async void Screenshake() {
        while(_shakeDuration > 0){
            var currPos = _camTransform.position;
            _camTransform.localPosition = currPos + Random.insideUnitSphere * _shakeForce / MaxShakeForce;
            _shakeDuration -= Time.deltaTime * DecreaseFactor;
            await Task.Delay(18);
        }
        _shakeDuration = 0f;
        ResetCamera();
    }

    async void ZoomIn(GameObject player1, GameObject player2){
        Debug.Log("Zooming in");
        SlowDown = true;
        var startTime = Time.time;
        var counter = 0;
        var origCameraSize = Camera.main.orthographicSize;
        var origCameraLocation = Camera.main.transform.position;
        while (SlowDown && Camera.main.orthographicSize > 3){
            float t = ((startTime + counter) - startTime) * (ZoomSpeed * SloMoAmount);
            Camera.main.orthographicSize = Mathf.SmoothStep(origCameraSize, 3, t);
            Vector3 camTarget = GetCameraVector(player1, player2);
            Camera.main.transform.position = Vector3.Lerp(origCameraLocation, camTarget, t);
            counter++;
            await Task.Delay(1);
        }
    }
    async void ZoomOut(){
        SlowDown = false;
        var startTime = Time.time;
        var counter = 0;
        var origCameraSize = Camera.main.orthographicSize;
        var origCameraLocation = Camera.main.transform.position;
        while (!SlowDown && Camera.main.orthographicSize < 5){
            float t = ((startTime + counter) - startTime) * (ZoomSpeed * SloMoAmount);
            Camera.main.orthographicSize = Mathf.SmoothStep(origCameraSize, 5, t);
            Camera.main.transform.position = Vector3.Lerp(origCameraLocation, _originalPos, t);
            counter++;
            await Task.Delay(1);
        }
        return;
    }

    private Vector3 GetCameraVector(GameObject player1, GameObject player2){
        Vector3 normalized;
        if (player1 == null){
            normalized = player1.transform.position;
        }else if (player2 == null){
            normalized = player1.transform.position;
        }else {
            var pos1 = player1.transform.position;
            var pos2 = player2.transform.position;
            normalized = (pos1 + pos2) / 2;
        }
        return new Vector3(normalized.x, normalized.y, -10);
    }
    private async void SlowMoCountDown(){
		await Task.Delay(getMaxSlowMo());
        if (SlowDown)
            ResetCamera();
	}
    public void cameraZoom(GameObject pos1, GameObject pos2){
        Time.timeScale = SloMoAmount;
         if (!SlowDown){
            ZoomIn(pos1, pos2);
        }
        SlowMoCountDown();
    }
    public void ResetCamera() {
        Time.timeScale = 1;
        if (SlowDown) ZoomOut();
    }
}