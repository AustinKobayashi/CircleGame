using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public Transform Arrow;
	
	private SpriteRenderer _arrowSpriteRenderer;
	private Rigidbody2D _rigid;

	public float Speed;
	
	public float RotateFrequency;
	public float RotationAmount;
	public float PowerIncreaseRate;
	
	private float _rotateTimer = 0;
	
	private float _power;

	
	private float _angle = 0;
	
	
	void Awake () {
		_arrowSpriteRenderer = Arrow.GetComponent<SpriteRenderer>();
		_rigid = GetComponent<Rigidbody2D>();
	}
	
	

	void Update () {
		_rotateTimer += Time.deltaTime;
		
		if (_rotateTimer >= RotateFrequency) {
			Rotate();
			_rotateTimer = 0;
		}
	}


	
    void FixedUpdate() {
	    if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) {
		    _power += PowerIncreaseRate;
	    }

	    if (Input.touchCount > 0) {
		    _power += PowerIncreaseRate;
	    }

	    _power = Mathf.Min(_power, 1);
	    
	    if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetKeyUp(KeyCode.Z)) {
		    Shoot();
		    _power = 0;
	    }
	}


	void Rotate() {
		Arrow.RotateAround(transform.position, Vector3.forward, RotationAmount);
		Vector4 color = _arrowSpriteRenderer.color;
		_arrowSpriteRenderer.color = new Vector4(_power, 0f, 0f, 1f);
		_angle += RotationAmount;
		if (_angle >= 360) {
			_angle = 0;
		}
	}
	
	
	

	void Shoot() {
		_rigid.AddForce(Speed * _power * new Vector2(Mathf.Cos(Mathf.Deg2Rad * _angle), Mathf.Sin(Mathf.Deg2Rad * _angle)), ForceMode2D.Impulse);
	}
}
