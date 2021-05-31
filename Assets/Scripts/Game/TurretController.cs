using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
	#region Variables
	[SerializeField] [Tooltip("Damage done per second the laser is firing")]
	private float damage = 5;
	[SerializeField] [Tooltip("Max range of the laser, past that and it won't do any damage")]
	private float maxRange = 100;

	[Tooltip("When enabled, it will do its fire animation and fire")]
	public bool enableFiring = true;

	[SerializeField]
	private LineRenderer lineRenderer;
	[SerializeField]
	private Transform tipOfBarrel;

	[SerializeField] [Tooltip("When at max value, the laser will fire")]
	private CurveProgressValue cpv;

	private bool isFiring = false;
	private int targetLayerMask = LayerManager.Terrain | LayerManager.Targets;
	#endregion

	#region MonoBehaviours
	void Start()
    {
		InitializeLineRenderer();
	}

    void FixedUpdate()
    {
		AdjustLaserLength();
		if (enableFiring && Input.GetButton("Fire"))
			isFiring = true;
		if (isFiring)
			Fire();
	}
	#endregion

	#region Methods
	private void InitializeLineRenderer() {
		if (lineRenderer == null)
			return;
		lineRenderer.useWorldSpace = false;
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition(0, Vector3.zero);
		lineRenderer.SetPosition(1, Vector3.forward * maxRange);
		lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, 1);
		cpv.SetToMinProgress();
		lineRenderer.widthMultiplier = cpv.Value;
	}

	private Vector3 FindTarget(Vector3 origin, Vector3 direction) {
		if (Physics.Raycast(origin, direction, out RaycastHit hit, maxRange, targetLayerMask)) {
			return hit.point;
		}
		return direction * maxRange + origin;
	}
	private void AdjustLaserLength() {
		if (tipOfBarrel == null || lineRenderer == null)
			return;
		lineRenderer.SetPosition(1, tipOfBarrel.InverseTransformPoint(FindTarget(tipOfBarrel.position, tipOfBarrel.forward)));		
	}

	private void Fire() {
		if (lineRenderer == null && tipOfBarrel == null) {
			isFiring = false;
			return;
		}

		lineRenderer.widthMultiplier = cpv.GetValueAndIncrementProgress(Time.fixedDeltaTime);

		//this does have the problem of possibly skipping the actual shoot portion on low frames but eh...
		if (cpv.Value == cpv.MaxOutputValue && 
			Physics.Raycast(tipOfBarrel.position, tipOfBarrel.forward, out RaycastHit hit, maxRange, targetLayerMask)) {
			if (hit.collider.gameObject.TryGetComponent(out IDamagable damagable)) {
				damagable.Damage(damage * Time.fixedDeltaTime);
			}
		}
		if(cpv.IsMaxProgress) {
			cpv.SetToMinProgress();
			isFiring = false;
		}
	}
	
	#endregion
}
