using System.Collections.Generic;
using UnityEngine;

public class ErBasicCarController : MonoBehaviour
{
  [SerializeField]
  public WheelCollider FrontRight;
  public WheelCollider FrontLeft;
  public WheelCollider BackRight;
  public WheelCollider BackLeft;

  #region Wheel accessors

  private IEnumerable<WheelCollider> Wheels
  {
    get
    {
      yield return BackRight;
      yield return BackLeft;
      yield return FrontRight;
      yield return FrontLeft;
    }
  }

  private IEnumerable<WheelCollider> MotorWheels
  {
    get
    {
      yield return BackRight;
      yield return BackLeft;
    }
  }

  private IEnumerable<WheelCollider> TurningWheels
  {
    get
    {
      yield return FrontRight;
      yield return FrontLeft;
    }
  }

  #endregion

  public float MaxAcceleration = 500.0f;
  public float MaxBrakingForce = 300.0f;
  public float MaxTurningAngle = 15.0f;

  public float CurrentAcceleration = 0.0f;
  public float CurrentBrakingForce = 0.0f;
  public float CurrentTurningAngle = 0.0f;

  // Start is called before the first frame update
  void Start()
  {

  }

  private void FixedUpdate()
  {
    CurrentAcceleration = Input.GetKey(KeyCode.W) ? Mathf.Clamp((CurrentAcceleration + 1) * 1.5f, 0, MaxAcceleration) : 0;
    foreach (var wheel in MotorWheels) wheel.motorTorque = CurrentAcceleration;

    CurrentBrakingForce = Input.GetKey(KeyCode.Space) ? MaxBrakingForce : 0;
    foreach (var wheel in Wheels) wheel.brakeTorque = CurrentBrakingForce;

    CurrentTurningAngle = Input.GetKey(KeyCode.D)
      ? MaxTurningAngle
      : Input.GetKey(KeyCode.A)
          ? -MaxTurningAngle
          : 0;
    foreach (var wheel in TurningWheels) wheel.steerAngle = CurrentTurningAngle;
  }

  // Update is called once per frame
  void Update()
  {

  }
}
