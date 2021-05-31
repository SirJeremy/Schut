using UnityEngine;

public class Targeter : MonoBehaviour {
	#region Variables
    [Tooltip("Targeter Goal is where the Targeter Current will move towards")]
	public Transform targeterGoal;
	[SerializeField] [Tooltip("Targeter Current is where you are currently aiming")]
	private Transform targeterCurrent;
	[SerializeField] [Tooltip("The target that the turrets aim at")]
	private Transform turretTarget;
	[SerializeField] [Tooltip("Camera that the player see through")]
	private Camera playerCamera;
	//using colider bc easier to manipulate
	[SerializeField] [Tooltip("Box collider that is used as the bounds for Trageter Curret. Should be set up as Z forward")]
	private BoxCollider boundsCollider;

	[Tooltip("The speed of Targeter Current in units per second")]
	public float moveSpeed = 5;
	[Tooltip("The max range of the turrets. If not target is found within range, turrets will aim to max range")]
	public float turretMaxRange = 100;
	[Tooltip("The max range of the targeter.")]
	public float targeterMaxRange = 2;
	[Tooltip("If enabled, the aim of the projector will update to current mouse input. If disabled, it will remember the last position and use that instead")]
	public bool enableInputUpdate = true;

	private Vector3 boundsNormal;

	private Vector3 boundsLastPosition = Vector3.zero;
	private Vector3 boundsLastScale = Vector3.one;
	private Quaternion boundsLastRotation = Quaternion.identity;
	private Vector3 boundsLastColliderCenter = Vector3.zero;
	private Vector3 boundsLastColliderSize = Vector3.one;

	private int targeterLayerMask = LayerManager.ShipScreen;
	private int terrainTargetsLayerMask = LayerManager.Targets | LayerManager.Terrain;
	#endregion

	#region MonoBehaviours
	void Start() {
		if (boundsCollider != null) {
			InitializeLastBoundValues();
			InitializeBounds();
		}
	}

	void LateUpdate() {
		if (GameManager.Instance.IsPaused)
			return;
		InitializeBoundsIfChanged();
		MoveTargeterGoal();
		MoveTargetCurrent();
		MoveTurretTarget();
	}
	void OnDrawGizmos() {
		if (boundsCollider == null)
			return;
		Gizmos.matrix = boundsCollider.transform.localToWorldMatrix;
		Gizmos.color = new Color(0,1,1,.3f);
		Gizmos.DrawCube(boundsCollider.center, boundsCollider.size);
	}
	#endregion

	#region Methods
	private void InitializeLastBoundValues() {
		boundsLastPosition = boundsCollider.transform.position;
		boundsLastScale = boundsCollider.transform.localScale;
		boundsLastRotation = boundsCollider.transform.rotation;
		boundsLastColliderCenter = boundsCollider.center;
		boundsLastColliderSize = boundsCollider.size;
	}
	private void InitializeBounds() {
		Transform boundsTransform = boundsCollider.transform;

		//calc coners of box collider
		Vector3 lowerRightBound = new Vector3(boundsCollider.size.x / 2, -boundsCollider.size.y / 2);
		Vector3 lowerLeftBound = new Vector3(-boundsCollider.size.x / 2, -boundsCollider.size.y / 2);
		Vector3 upperRightBound = new Vector3(boundsCollider.size.x / 2, boundsCollider.size.y / 2);

		//calc normal, used for projecting onto bound (finite plane)
		boundsNormal = Vector3.Cross(lowerLeftBound - lowerRightBound, upperRightBound - lowerRightBound);

		//transform from local to global space;
		boundsNormal = boundsTransform.TransformDirection(boundsNormal);
	}
	private void InitializeBoundsIfChanged() {
		if (boundsCollider != null && BoundsChanged()) {
			InitializeBounds();
			InitializeLastBoundValues();
		}
	}

	private bool BoundsChanged() {
		return
			boundsLastPosition != boundsCollider.transform.position ||
			boundsLastScale != boundsCollider.transform.localScale ||
			boundsLastRotation != boundsCollider.transform.rotation ||
			boundsLastColliderCenter != boundsCollider.center ||
			boundsLastColliderSize != boundsCollider.size;
	}
	

	//returns move direction of the targetCurrent
	private Vector3 CalculaulateMoveDirection() {
		return Vector3.ProjectOnPlane(targeterGoal.position - targeterCurrent.position, boundsNormal);
	}
	//scales moveDir by movespeed and makes sure it doesnt overshoot targetGoal
	private Vector3 ScaleMovementAmount(Vector3 moveDir) {
		Vector3 movement = moveDir.normalized * moveSpeed * Time.deltaTime;
		//clamp by targetgoal (dont overshoot)
		if (movement.sqrMagnitude > moveDir.sqrMagnitude)
			return moveDir;
		return movement;
	}
	private void MoveTargetCurrent() {
		if (boundsCollider == null || targeterGoal == null || targeterCurrent == null)
			return;

		Vector3 moveDir = ScaleMovementAmount(CalculaulateMoveDirection());
		targeterCurrent.position += moveDir;
		//Clamp to box
		targeterCurrent.position = boundsCollider.ClosestPoint(targeterCurrent.position);
	}

	//outs a point in worldspace from given screenPosition. if no hit is found, the point of max range is outed. returns if hit found
	private Vector3 CastPointFromAim(Vector3 screenPosition, int layerMask, float maxRange) {
		Ray ray = playerCamera.ScreenPointToRay(screenPosition);
		if (Physics.Raycast(ray, out RaycastHit hit, maxRange, layerMask))
			return hit.point;
		return ray.direction * maxRange + playerCamera.transform.position;
	}
	private void MoveTargeterGoal() {
		if (enableInputUpdate && targeterGoal != null && playerCamera != null) {
			Vector3 point = CastPointFromAim(Input.mousePosition, targeterLayerMask, targeterMaxRange);
			targeterGoal.position = boundsCollider.ClosestPoint(point);
			//some funkyness with the behaviour, but its good enough without busting out real projection math
			//to fix it you would have targetMaxRange be the length of ray to a plane which dont go through origin (targeter screen)
		}
	}
	private void MoveTurretTarget() {
		if(turretTarget != null && targeterCurrent != null && playerCamera != null) {
			Vector3 targeterCurrentScreenSpacePos = playerCamera.WorldToScreenPoint(targeterCurrent.position);
			turretTarget.position = CastPointFromAim(targeterCurrentScreenSpacePos, terrainTargetsLayerMask, turretMaxRange);
		}
	}
	#endregion
}
