using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour {

	public Transform Arrows;
	public List<GameObject> arrowRefs = new List<GameObject>();
	public GameObject _arrowPrefab;
	private Rigidbody2D _rigid;
    public ParticleSystem deathExplosion;

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
	private Effectsv2 effects;
	private bool _zoomed;
	
	
	public void SetEffects(Effectsv2 effects){
		this.effects = effects;
	}
	
	
	
	public void SetInitialAngle(float angle){
		_angle = angle;
	}
	
	
	
	public void SetGameManager(GameManager gameManager) {
		_gameManager = gameManager;
	}
	
	
	
	// TODO: refactor to hide/enable rather than instantiate/destroy
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
		} else {
			_text = texts[1];
			_text.color = Color.blue;
		}
		RotateToAngle(Arrows, transform.position, _angle);		
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
		HandleMovement();
	}

	
	
	private void HandleMovement() {
		if (Player1) {
			Input.touches
			.Where(touch => !TouchedRight(touch))
			.ToList()
			.ForEach(HandleTouch);
		} else {
			Input.touches
			.Where(touch => TouchedRight(touch))
			.ToList()
			.ForEach(HandleTouch);
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
		return touch.position.x > Screen.width / 2f;
	}

	
	
	private void HandleTouch(Touch touch) {
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
		if (_rigid.velocity == Vector2.zero){
			_zoomed = false;
			_attacking = false;
			return;
		}
		var hits = Physics2D.RaycastAll(transform.position, _rigid.velocity.normalized, _rigid.velocity.magnitude*0.05f, LayerMask.GetMask("SloMo"));
		Debug.DrawRay(transform.position,_rigid.velocity.normalized * (_rigid.velocity.magnitude * 0.05f), Color.red, 2);
		if (hits.Length <= 1) return;
		var hit = hits.ToList().Find(findhit => findhit.collider.name != name);
		if (hit && !_zoomed){
			_zoomed = true;
			if (!effects.SlowDown)
				effects.cameraZoom(this.gameObject, hit.collider.gameObject);
		}
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
			Speed * 
			_power * 
			(new Vector2(Mathf.Cos(Mathf.Deg2Rad * _angle), Mathf.Sin(Mathf.Deg2Rad * _angle)).normalized), 
			ForceMode2D.Impulse);
		_power = 0;
	}

	

	private bool AmIFaster(Collider2D other) {
		Vector2 myVelocity = _rigid.velocity;
		Vector2 otherVel = other.GetComponent<Rigidbody2D>().velocity;
		return myVelocity.magnitude >= otherVel.magnitude;
	}

	

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			if (!AmIFaster(other)) {
				if (DebugStatements)
					Debug.Log(other.gameObject.name + " is invincible to hit from " + gameObject.name);
				return;
			}
			other.GetComponent<Player>().TakeDamage(1);
		}
	}
	
	public void TakeDamage(int amount) {
		if (Invincible()) return;
		
		Health -= amount;
		_iFramesCount = IFrameAmount;
		if (Health <= 0) {
            deathExplosion.transform.parent = null;
            deathExplosion.Play();
            _gameManager.Die(gameObject);
        }
	}


	
	private bool Invincible() {
		return _iFramesCount > 0;
	}
	
	

	public bool IsAttacking() {
		return _attacking;
	}
}
