using System.Collections.Generic;
using System.Linq;
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
  public float MaxBrakingForce = 5000.0f;
  public float MaxTurningAngle = 15.0f;

  public float CurrentAcceleration = 0.0f;
  public float CurrentBrakingForce = 0.0f;
  public float CurrentTurningAngle = 0.0f;

  public AudioClip IdleSound;

  public AudioClip AccelerationSound;

  private AudioSource IdleAS;

  private AudioSource AccelerationAS;

  private const int PreviousRpmsCapacity = 10;
  private Queue<float> PreviousRpms = new Queue<float>(PreviousRpmsCapacity);

  // Start is called before the first frame update
  void Start()
  {
    IdleAS = gameObject.AddComponent<AudioSource>();
    AccelerationAS = gameObject.AddComponent<AudioSource>();

    IdleAS.clip = IdleSound;
    AccelerationAS.clip = AccelerationSound;

    IdleAS.volume = 0.2f;
    IdleAS.loop = true;
    AccelerationAS.loop = true;

    IdleAS.Play();
    AccelerationAS.Play(); AccelerationAS.Pause();
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
    // Calculate current pitch by RPM
    var rpm = GetWheelRpm();

    if (rpm > 0.1)
    {
      // Speeding
      IdleAS.Pause();
      AccelerationAS.UnPause();
    }
    else
    {
      // Idle
      IdleAS.UnPause();
      AccelerationAS.Pause();
    }

    AccelerationAS.pitch = rpm;
  }

  private static void UpdateWheelPosition(WheelCollider collider, Transform model)
  {
    collider.GetWorldPose(out var position, out var rotation);
    model.SetPositionAndRotation(position, rotation);
  }

  private float GetWheelRpm()
  {
    var rpm = 0.0f;
    rpm = Mathf.Clamp(MotorWheels.Average(x => x.rpm) * 0.005f, -2.0f, 2.0f);

    if (PreviousRpms.Count == PreviousRpmsCapacity)
      PreviousRpms.Dequeue();
    PreviousRpms.Enqueue(rpm);

    return PreviousRpms.Average();
  }
}
