using UnityEngine;

public class ErPlayerBehavior : MonoBehaviour
{
  [SerializeField]
  public GameObject Camera;

  void Start()
  {
    var placeholderCamera = transform.GetChild(0);
    Camera.transform.SetPositionAndRotation(placeholderCamera.position, placeholderCamera.rotation);
  }

  void Update()
  {

  }
}
