using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamagable {

	#region Variables
	[SerializeField]
	private float health = 5;
	[SerializeField] [Tooltip("How close the player has to be in order to begin facing it")]
	private float faceRange = 20;
	[SerializeField] [Tooltip("How quickly the face will turn in degrees per secon")]
	private float turnSpeed = 20;
	[SerializeField]
	private Transform head;
	[SerializeField]
	private Transform playerCamera;
	#endregion

	#region MonoBehaviours
	void Start() {
		playerCamera = FindCamera();
	}

    void Update()
    {
		SpinHead();
	}
	#endregion

	#region IDamagable
	public float Health => health;

	public void Damage(float value) {
		health -= value;
		if (health <= 0)
			Kill();
	}

	public void Kill() {
		Destroy(gameObject);
	}
	#endregion

	#region Methods
	private Transform FindCamera() {
		//better ways of doing this but works this
		return Camera.main.transform;
	}
	private void SpinHead() {
		if(head != null && playerCamera != null) {
			Quaternion desiredRot;
			if ((playerCamera.position - head.position).sqrMagnitude <= faceRange * faceRange)
				desiredRot = Quaternion.LookRotation(playerCamera.position - head.position, transform.up);
			else //idle cw spin
				desiredRot = Quaternion.LookRotation(head.right, transform.up);
			head.rotation = Quaternion.RotateTowards(head.rotation, desiredRot, turnSpeed * Time.deltaTime);
		}
	}
	#endregion
}
