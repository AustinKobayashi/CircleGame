using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

public class Effectsv2 : MonoBehaviour {
    public float SloMoDuration;
    public float SloMoAmount;
    public float ZoomSpeed;
    public float ZoomAmount;
    public float MinZoomVelocity;
    public float ScreenShakeDuration;
    public float DecreaseFactor = 1.0f;
    public float MaxShakeForce = 40;
    [HideInInspector]
    public bool SlowDown;
    private Transform _camTransform;
    private Vector3 _origCamPos;
    private float _origFixedTime;
    private float _origCameraZoom;
    private float _shakeDuration = 0f;
    private float _shakeForce;

    void Awake() {
        _camTransform = Camera.main.transform;
        _origCamPos = _camTransform.position;
        _origFixedTime = Time.fixedDeltaTime;
        _origCameraZoom = Camera.main.orthographicSize;
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
            await Task.Delay(10);
        }
        _shakeDuration = 0f;
        ResetCamera();
    }
    public void cameraZoom(GameObject pos1, GameObject pos2){
        var hits = Physics2D.RaycastAll(pos1.transform.position, pos1.GetComponent<Rigidbody2D>().velocity.normalized * 10, Mathf.Infinity, LayerMask.GetMask("SloMo"));
        if (hits.Length <= 1) return;
        Time.timeScale = SloMoAmount;
         if (!SlowDown){
            ZoomIn(pos1, pos2);
        }
        SlowMoCountDown();
    }
    public async void ResetCamera() {
        Time.timeScale = 1;
        if (SlowDown) await ZoomOut();
        if (!SlowDown) _camTransform.position = _origCamPos;
    }
    async void ZoomIn(GameObject player1, GameObject player2){
        SlowDown = true;
        var startTime = Time.time;
        var counter = 0;
        var origCameraSize = Camera.main.orthographicSize;
        var origCameraLocation = _camTransform.position;
        while (SlowDown && Camera.main.orthographicSize > ZoomAmount){
            float t = ((startTime + counter) - startTime) * (ZoomSpeed * SloMoAmount);
            Camera.main.orthographicSize = Mathf.SmoothStep(origCameraSize, ZoomAmount, t);
            Vector3 camTarget = GetCameraVector(player1, player2);
            _camTransform.position = Vector3.Lerp(origCameraLocation, camTarget, t);
            counter++;
            await Task.Delay(10);
        }
    }
    async Task ZoomOut(){
        SlowDown = false;
        var startTime = Time.time;
        var counter = 0;
        var origCameraSize = Camera.main.orthographicSize;
        var origCameraLocation = _camTransform.position;
        while (!SlowDown && Camera.main.orthographicSize < _origCameraZoom){
            float t = ((startTime + counter) - startTime) * (ZoomSpeed * SloMoAmount);
            Camera.main.orthographicSize = Mathf.SmoothStep(origCameraSize, _origCameraZoom, t);
            _camTransform.position = Vector3.Lerp(origCameraLocation, _origCamPos, t);
            counter++;
            await Task.Delay(10);
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
    public int getMaxSlowMo(){
        return (int)(SloMoDuration * 1000);
    }
    private async void SlowMoCountDown(){
		await Task.Delay(getMaxSlowMo());
        if (SlowDown)
            ResetCamera();
	}
}