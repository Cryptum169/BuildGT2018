using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;


public class PlayerController : MonoBehaviour {
	private KinectSensor _Sensor = null; 
	private BodyFrameReader _Reader = null;
    public float speed;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

		_Sensor = KinectSensor.GetDefault();
		if (_Sensor != null)
		{
			_Reader = _Sensor.BodyFrameSource.OpenReader();

			if (!_Sensor.IsOpen)
			{
				_Sensor.Open();
			}
		}   
	}
	

    // For Physics
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		if (_Sensor == null) {
			rb.AddForce (movement * speed);
		} else {
			
		}
    }
}
