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

	public int IFrameAmount;
	
	public bool Player1;

	public float Health;
	
	private float _rotateTimer = 0;
	private float _angle = 0;
	private int _power;
	private float _powerTimer;
	private bool _attacking;
	private int _iFramesCount;
	
	
	
	void Awake () {
		_arrowSpriteRenderer = Arrow.GetComponent<SpriteRenderer>();
		_rigid = GetComponent<Rigidbody2D>();
	}
	
	

	void Update () {
		if (_iFramesCount > 0) { _iFramesCount--; }
		
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
		    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z)) {
			    _power = 0;
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
		    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.M)) {
			    _power = 0;
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
    }

	

	void Rotate() {
		Arrow.RotateAround(transform.position, Vector3.forward, RotationAmount);
		_arrowSpriteRenderer.color = new Vector4((float)_power / (float)MaxPower, 0f, 0f, 1f);
		_angle += RotationAmount;
		if (_angle >= 360) {
			_angle = 0;
		}
	}
	
	
	

	void Shoot() {
		_attacking = true;
		_powerTimer = 0;
		_rigid.AddForce(Speed * _power * (new Vector2(Mathf.Cos(Mathf.Deg2Rad * _angle), Mathf.Sin(Mathf.Deg2Rad * _angle)).normalized), ForceMode2D.Impulse);
		_power = 0;
	}


	
	void OnCollisionEnter2D(Collision2D other) {
		if (!_attacking)
			return;
		
		if (other.gameObject.CompareTag("Player")) {
			if (other.gameObject.GetComponent<Player>().Invincible()) {
				Debug.Log(other.gameObject.name  + " is invincible to hit from " + gameObject.name);
				return;
			}

			Debug.Log(_rigid.velocity.magnitude);
			int damage = Mathf.RoundToInt(_rigid.velocity.magnitude * 2f / Speed);
			other.gameObject.GetComponent<Player>().TakeDamage(damage);
			Debug.Log(gameObject.name + " damaged " + other.gameObject.name + " for " + damage);
		}
	}

	
	
	public void TakeDamage(int amount) {
		Health -= amount;
		
		_iFramesCount = IFrameAmount;
		
		if (Health <= 0) {
			Destroy(gameObject);
		}
	}
	


	public bool Invincible() {
		return _iFramesCount > 0;
	}
}
