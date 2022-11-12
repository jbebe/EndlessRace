using System;
using UnityEngine;

namespace EndlessRace
{
  public class CarMovement : MonoBehaviour
  {
    public Camera Camera;
    private float MaxSpeed = 70.0f;
    private float MinSpeed = 2.0f;
    private float SpeedMult = 1.01f;
    private float Speed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      UpdateMotion();
      UpdateCamera();
    }

    private void UpdateCamera()
    {
      var offset = ((0.7f * (-gameObject.transform.forward)) + (0.3f * gameObject.transform.up)).normalized * 5.0f;
      Camera.transform.position = gameObject.transform.position + offset;
      Camera.transform.LookAt(gameObject.transform);
    }

    private void UpdateMotion()
    {
      if (Input.GetKeyDown(KeyCode.W))
      {
        if (Speed == 0.0f) Speed = MinSpeed;
      }
      
      if (Input.GetKey(KeyCode.W))
      {
        Speed *= SpeedMult;
        if (Speed > MaxSpeed) Speed = MaxSpeed;
      }
      else
      {
        Speed /= SpeedMult;
        if (Speed < MinSpeed) Speed = 0.0f;
      }

      var rotationStep = 0.2f;
      if (Input.GetKey(KeyCode.A))
      {
        gameObject.transform.Rotate(Vector3.up, -rotationStep);
      }
      if (Input.GetKey(KeyCode.D))
      {
        gameObject.transform.Rotate(Vector3.up, rotationStep);
      }

      if (Speed != 0.0f)
      {
        gameObject.transform.position += (Speed * Time.deltaTime) * transform.forward;
      }
    }
  }
}
