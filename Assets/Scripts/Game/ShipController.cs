using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
	#region Variables
	[Header("Speed")]
	[Tooltip("How quickly it speeds up")]
	public float speed = 5;
	[Tooltip("Max speed it can reach")]
	public float maxSpeed = 10;
	[Range(0, 1)] [Tooltip("How much the speed reduces per second as a percentage")]
	public float speedDecayPercent = 0.2f;
	[Tooltip("How much the speed reduces per per second when not trying to move")]
	public float speedDecayFlat = 2f;

	[Header("Rotation")]
	public float mouseSensitivityHorizontal = 1;
	public float mouseSensitivityVertical = 1;
	[Range(0, 5)]
	[Tooltip("How quickly it will rotate to remove roll per second in a lerp")]
	public float correctiveRotationSpeedLerp = 0.8f;
	[Tooltip("How quickly it will rotate to remove roll in degrees per second")]
	public float correctiveRotationSpeedFlat = 30;
	[Tooltip("How quickly it will reduce angular velocity in degrees per second")]
	public float antiAngularVelocitySpeed = 10;

	[Header("References")]
	[SerializeField]
	private Targeter targeter;
	[SerializeField]
	private TurretController[] turrets = new TurretController[1]; 

	private Rigidbody rb;
	#endregion

	#region MonoBehaviours
	void Start()
    {
		rb = GetComponent<Rigidbody>();
		EnableTargeter(false);
    }

    void FixedUpdate()
    {
		Move();
		RemoveRoll();
		if (Input.GetButton("Ready Weapons")) {
			ReadyWeapons(true);
		}
		else {
			ReadyWeapons(false);
			Rotate();
		}
	}

	#endregion

	#region Methods
	private void Move() {
		//not using raw input for floaty spaceship feel
		Vector3 movementInput = new Vector3(Input.GetAxis("Right"), Input.GetAxis("Up"), Input.GetAxis("Forward"));
		movementInput = transform.rotation * movementInput;

		//if magnitude is greater than 1, nomalize it
		if (movementInput.sqrMagnitude > 1)
			movementInput = movementInput.normalized;

		//add speed and flat decay to counterbalance flat decay for more accurate speed value
		rb.velocity += movementInput * (speed + speedDecayFlat) * Time.fixedDeltaTime;
		rb.velocity -= rb.velocity * speedDecayPercent * Time.fixedDeltaTime;

		//make sure flat decay only reduces velocity
		Vector3 flatDecay = rb.velocity.normalized * speedDecayFlat * Time.fixedDeltaTime;
		if (rb.velocity.sqrMagnitude < flatDecay.sqrMagnitude)
			rb.velocity = Vector3.zero;
		else
			rb.velocity -= flatDecay;

		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
	}
	private void Rotate() {
		transform.Rotate(Vector3.right, -Input.GetAxisRaw("Mouse Y") * mouseSensitivityVertical * Time.fixedDeltaTime, Space.Self);
		transform.Rotate(Vector3.up, Input.GetAxisRaw("Mouse X") * mouseSensitivityHorizontal * Time.fixedDeltaTime, Space.World);

		//wanted to add a more floaty turning, but it was taking too much time
		//there is also some issue with turing past vertical poles but its good enough for what this is

		//remove force so it eventually stops, also good for collisions
		rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, Vector3.zero, Mathf.Deg2Rad * antiAngularVelocitySpeed * Time.fixedDeltaTime);
	}

	private void RemoveRoll() {
		//keep look roataion but remove roll at corrective speed
		Quaternion correctedRotation = Quaternion.LookRotation(transform.forward);
		transform.rotation = Quaternion.Lerp(transform.rotation, correctedRotation, correctiveRotationSpeedLerp * Time.fixedDeltaTime);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, correctedRotation, correctiveRotationSpeedFlat * Time.fixedDeltaTime);
	}

	private void ReadyWeapons(bool enable) {
		if(!GameManager.Instance.IsPaused)
			Cursor.lockState = enable ? CursorLockMode.Confined : CursorLockMode.Locked;
		EnableTargeter(enable);
		EnableTurrets(enable);
	}
	private void EnableTargeter(bool enable) {
		if(targeter != null) {
			targeter.enableInputUpdate = enable;
		}
	}
	private void EnableTurrets(bool enable) {
		foreach(TurretController turret in turrets) {
			if (turret != null)
				turret.enableFiring = enable;
		}
	}
	#endregion
}
