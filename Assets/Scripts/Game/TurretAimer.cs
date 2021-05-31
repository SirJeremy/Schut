using UnityEngine;

public class TurretAimer : MonoBehaviour
{
    #region Variables
    [Tooltip("The target that the turret aims at")]
    public Transform target;
    [SerializeField] [Tooltip("The head of turret that swivels.")]
    private Transform turretBase;
    [SerializeField] [Tooltip("The barrel of the turret that moves up and down. Barrel should be a child to base")]
    private Transform turretBarrel;

    [Tooltip("Horizontal turn speed in degrees per second")]
    public float baseTurnSpeed = 180;
    [Tooltip("Vertical turn speed in degrees per second")]
    public float barrelTurnSpeed = 90;
    #endregion

    #region MonoBehaviours
    void FixedUpdate()
    {
        if (target != null)
            AimTurret();
    }
    #endregion

    #region Methods
    //aims turret to target with max turn speed
    private void AimTurret() {
        //aim base
        if(turretBase != null) {
            RotateToDirection(turretBase,
                CalculateRotationDirection(turretBase.up, turretBase.forward, target.position - turretBase.position, baseTurnSpeed));
        }
        
        //aim barrel
        if(turretBarrel != null) {
            RotateToDirection(turretBarrel,
                CalculateRotationDirection(turretBarrel.right, turretBarrel.forward, target.position - turretBarrel.position, barrelTurnSpeed));
        }
    }
    //calculates the rotation dir from the forwardDir to targetDir locked on rotation axis
    private Vector3 CalculateRotationDirection(Vector3 rotationAxis, Vector3 forwardDir, Vector3 targetDir, float turnSpeed) {
        targetDir = Vector3.ProjectOnPlane(targetDir, rotationAxis);
        return Vector3.RotateTowards(forwardDir, targetDir, turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0);
    }
    //does the rotates to dir in world space
    private void RotateToDirection(Transform transform, Vector3 rotationDir) {
        transform.rotation = Quaternion.LookRotation(rotationDir, transform.up);
    }
    #endregion
}
