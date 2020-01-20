using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public Transform Arrows;
	public List<GameObject> arrowRefs = new List<GameObject>();
	public GameObject _arrowPrefab;
	private Rigidbody2D _rigid;

	public float Speed;

	public float RotateFrequency;
	public float RotationAmount;
	public float PowerStageTimer;
	public int MaxPower;
	public float DamageModifier;

	public int IFrameAmount;

	public bool Player1;

	public float Health;

	public bool DebugStatements;
	public float InitialArrowDistance;
	public float ArrowSeperationDistance;
	public float ArrowSizeIncrease;
	private float _rotateTimer = 0;
	private float _angle = 0;
	private int _power;
	private float _powerTimer;
	private bool _attacking;
	private int _iFramesCount;
	private UnityEngine.UI.Text _text;

	private GameManager _gameManager;
	Effectsv2 effects;

	public void setEffects(Effectsv2 effects){
		this.effects = effects;
	}

	void CalculateArrow() {
		arrowRefs
			.FindAll(o => Char.GetNumericValue(o.name[o.name.Length - 1]) > _power)
			.ForEach(x => {
				arrowRefs.Remove(x);
				Destroy(x);
			});
		for (int i = 0; i <= _power; i++) {
			if (arrowRefs.Count > i){
				continue;
			}
			float pos = InitialArrowDistance + (ArrowSeperationDistance * i);
			var arrowObject = Instantiate(_arrowPrefab, new Vector3(0, 0, 0), Arrows.rotation);
			arrowObject.name = $"Arrow:{i}";
			arrowObject.transform.SetParent(Arrows);
			arrowObject.transform.localPosition = new Vector3(pos, 0, 0);
			arrowObject.transform.localScale *= 1 + (i * ArrowSizeIncrease);
			arrowRefs.Add(arrowObject);
		}
	}
	
	void Start() {
		_rigid = GetComponent<Rigidbody2D>();
		var texts = GameObject
						.FindGameObjectsWithTag("scoreText")
						.Select(obj => obj.GetComponent<UnityEngine.UI.Text>())
						.ToList();
		if (name == "Player0"){
			_text = texts[0];
			_text.color = Color.red;
		}else{
			_text = texts[1];
			_text.color = Color.blue;
		}
		
	}



	void Update() {
		if (_iFramesCount > 0) {
			_iFramesCount--;
		}
		_text.text = $"{name}: {Health}";
		_rotateTimer += Time.deltaTime;

		if (_rotateTimer >= RotateFrequency) {
			Rotate();
			_rotateTimer = 0;
		}
		CalculateArrow();
		handleMovement();
	}

	private void handleMovement() {
		if (Player1) {
			Input.touches
			.Where(touch => !TouchedRight(touch))
			.ToList()
			.ForEach(handleTouch);
		}else {
			Input.touches
			.Where(touch => TouchedRight(touch))
			.ToList()
			.ForEach(handleTouch);
		}
		if (Player1) {			
			if (Input.GetKeyDown(KeyCode.Z)) {
				_power = 1;
			}

			if (Input.GetKey(KeyCode.Z)) {
				_powerTimer += Time.deltaTime;
			}

			if (Input.GetKeyUp(KeyCode.Z)) {
				Shoot();
			}
		}
		else {
			if (Input.GetKeyDown(KeyCode.M)) {
				_power = 1;
			}

			if (Input.GetKey(KeyCode.M)) {
				_powerTimer += Time.deltaTime;
			}

			if (Input.GetKeyUp(KeyCode.M)) {
				Shoot();
			}
		}
		if (_powerTimer >= PowerStageTimer) {
				_power++;
				_powerTimer = 0;
			}

		_power = Mathf.Min(_power, MaxPower);
	}
	private bool TouchedRight(Touch touch) {
		Debug.Log("Touch: " + touch.position.x);
		return touch.position.x > Screen.width / 2;
	}

	private void handleTouch(Touch touch) {
			if (touch.phase == TouchPhase.Began) {
					_power = 1;
				}
			if (touch.phase == TouchPhase.Stationary 
					|| touch.phase == TouchPhase.Moved){
				_powerTimer += Time.deltaTime;
			}
			if (touch.phase == TouchPhase.Ended){
				Shoot();
			}
	}

	void FixedUpdate() {
		if (_rigid.velocity == Vector2.zero)
			_attacking = false;
	}



	void Rotate() {
		_angle += RotationAmount;
		if (_angle >= 360) {
			_angle = 0;
		}

		RotateToAngle(Arrows, transform.position, _angle);
	}


	// (1, 0) relative to (0, 0) is the default angle of 0
	void RotateToAngle(Transform transform, Vector3 origin, float angle) {
		Vector3 angleVec = (transform.position - origin).normalized;
		Vector3 originVec = Vector3.right;
		float curRotation = Mathf.Acos(Vector3.Dot(angleVec, originVec)) * Mathf.Rad2Deg;
		if (angleVec.y < 0) {
			curRotation = 360 - curRotation;
		}

		float angleDelta = angle - curRotation;
		transform.RotateAround(origin, Vector3.forward, angleDelta);
	}




	void Shoot() {
		_attacking = true;
		_powerTimer = 0;
		_rigid.AddForce(
			Speed * _power * (new Vector2(Mathf.Cos(Mathf.Deg2Rad * _angle), Mathf.Sin(Mathf.Deg2Rad * _angle))
				.normalized), ForceMode2D.Impulse);
		_power = 0;
	}


	private bool amIFaster(Collider2D other) {
		Vector2 myVelocity = _rigid.velocity;
		Vector2 otherVel = other.GetComponent<Rigidbody2D>().velocity;
		return myVelocity.magnitude >= otherVel.magnitude;
	}
	// TODO: slow collisions will deal 0 damage. Do we want it to do at least 1?
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			if (!amIFaster(other)) {
				if (DebugStatements)
					Debug.Log(other.gameObject.name + " is invincible to hit from " + gameObject.name);
				return;
			}
			other.GetComponent<Player>().TakeDamage(1);
		}
		if (other.gameObject.CompareTag("SloMo")){
			effects.cameraZoom(transform.position, other.transform.position);
		}
	}
	private void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.CompareTag("SloMo")){
			effects.cameraZoom(transform.position, other.transform.position);
		}
	}
	private void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag("SloMo")){
			effects.resetCamera();
		}
	}

	public void TakeDamage(int amount) {
		Health -= amount;
		_iFramesCount = IFrameAmount;
		if (Health <= 0) {
			_gameManager.Die(gameObject);
		}
	}



	public bool Invincible() {
		return _iFramesCount > 0;
	}


	public void SetGameManager(GameManager gameManager) {
		_gameManager = gameManager;
	}



	public bool IsAttacking() {
		return _attacking;
	}
}
