using System.Collections.Generic;
using UnityEngine;

public class ErBasicCarController : MonoBehaviour
{
  [SerializeField]
  public WheelCollider FrCollider;

  [SerializeField]
  public WheelCollider FlCollider;

  [SerializeField]
  public WheelCollider BrCollider;

  [SerializeField]
  public WheelCollider BlCollider;

  [SerializeField]
  public Transform FrModel;

  [SerializeField]
  public Transform FlModel;

  [SerializeField]
  public Transform BrModel;

  [SerializeField]
  public Transform BlModel;

  #region Wheel accessors

  private IEnumerable<WheelCollider> MotorWheels
  {
    get
    {
      yield return BrCollider;
      yield return BlCollider;
    }
  }

  private IEnumerable<WheelCollider> TurningWheels
  {
    get
    {
      yield return FrCollider;
      yield return FlCollider;
    }
  }

  private IEnumerable<(WheelCollider Collider, Transform Model)> Wheels
  {
    get
    {
      yield return (BrCollider, BrModel);
      yield return (BlCollider, BlModel);
      yield return (FrCollider, FrModel);
      yield return (FlCollider, FlModel);
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

    CurrentTurningAngle = Input.GetKey(KeyCode.D)
      ? MaxTurningAngle
      : Input.GetKey(KeyCode.A)
          ? -MaxTurningAngle
          : 0;
    foreach (var wheel in TurningWheels) wheel.steerAngle = CurrentTurningAngle;

    CurrentBrakingForce = Input.GetKey(KeyCode.Space) ? MaxBrakingForce : 0;
    foreach (var (collider, model) in Wheels)
    {
      collider.brakeTorque = CurrentBrakingForce;
      UpdateWheelPosition(collider, model);
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  private static void UpdateWheelPosition(WheelCollider collider, Transform model)
  {
    collider.GetWorldPose(out var position, out var rotation);
    model.SetPositionAndRotation(position, rotation);
  }
}
