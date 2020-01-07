using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour {

	public Transform Arrow;

	private SpriteRenderer _arrowSpriteRenderer;
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
	
	private float _rotateTimer = 0;
	private float _angle = 0;
	private int _power;
	private float _powerTimer;
	private bool _attacking;
	private int _iFramesCount;
	private UnityEngine.UI.Text _text;

	private GameManager _gameManager;

	void Start() {
		_arrowSpriteRenderer = Arrow.GetComponent<SpriteRenderer>();
		_rigid = GetComponent<Rigidbody2D>();
		var texts = FindObjectsOfType<UnityEngine.UI.Text>();
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
	}



	void FixedUpdate() {
		if (_rigid.velocity == Vector2.zero)
			_attacking = false;


		if (Player1) {			
			if (Input.GetKeyDown(KeyCode.Z)) {
				_power = 1;
			}

			if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Z)) {
				_powerTimer += Time.deltaTime;
			}

			if (_powerTimer >= PowerStageTimer) {
				_power++;
				_powerTimer = 0;
			}

			_power = Mathf.Min(_power, MaxPower);

			if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Z)) {
				Shoot();
			}
		}
		else {
			if (Input.GetKeyDown(KeyCode.M)) {
				_power = 1;
			}

			if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.M)) {
				_powerTimer += Time.deltaTime;
			}

			if (_powerTimer >= PowerStageTimer) {
				_power++;
				_powerTimer = 0;
			}

			_power = Mathf.Min(_power, MaxPower);

			if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.M)) {
				Shoot();
			}
		}

		_arrowSpriteRenderer.color = new Vector4((float) _power / (float) MaxPower, 0f, 0f, 1f);
	}



	void Rotate() {
		_angle += RotationAmount;
		if (_angle >= 360) {
			_angle = 0;
		}

		RotateToAngle(Arrow, transform.position, _angle);
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



	// TODO: slow collisions will deal 0 damage. Do we want it to do at least 1?
	private void OnTriggerEnter2D(Collider2D other) {

		if (!_attacking)
			return;

		if (other.gameObject.CompareTag("Player")) {
			if (other.gameObject.GetComponent<Player>().Invincible()) {
				if (DebugStatements)
					Debug.Log(other.gameObject.name + " is invincible to hit from " + gameObject.name);
				return;
			}

			_gameManager.DamageCalculation(gameObject, other.gameObject, Speed);
//			int damage = Mathf.RoundToInt(other.relativeVelocity.magnitude * DamageModifier / Speed);
//			other.gameObject.GetComponent<Player>().TakeDamage(damage);
//			if (DebugStatements)
//				Debug.Log(gameObject.name + " damaged " + other.gameObject.name + " for " + damage);
		}
	}



	public void TakeDamage(int amount) {
		Health -= amount;

		_iFramesCount = IFrameAmount;

		if (Health <= 0) {
			_gameManager.Die(gameObject);
			Destroy(gameObject);
		}
	}



	public bool Invincible() {
		return _iFramesCount > 0;
	}


	public void SetGameManager(GameManager gameManager) {
		_gameManager = gameManager;
	}
}
