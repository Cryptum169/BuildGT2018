using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;


public class PlayerController : MonoBehaviour {
	private KinectSensor _Sensor = null; 
	private Vector3 move;
	private CoordinateMapper _Mapper = null;
	private BodyFrameReader _Reader = null;
	//private BodyFrame _People = null;
    public float speed;
	private Body[] bodies;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

		_Sensor = KinectSensor.GetDefault();
		_Mapper = _Sensor.CoordinateMapper;
		if (_Sensor != null)
		{
			_Reader = _Sensor.BodyFrameSource.OpenReader();
			if (!_Sensor.IsOpen)
			{
				_Sensor.Open();
			}
		}
		move = new Vector3 (0.0f, 0.0f, 0.0f);
	}
	

    // For Physics
    void Update()
    {
		bool dataReceived = false;
		BodyFrame bodyframe = _Reader.AcquireLatestFrame ();
		if (_Reader != null) {
			if (bodyframe != null) {
				if (bodies == null) {
					bodies = new Body[bodyframe.BodyCount];
				}

				bodyframe.GetAndRefreshBodyData (bodies);
				dataReceived = true;

				//frame.Dispose();
				//frame = null;
			}
		

		if (dataReceived) {
			Body firstBody = bodies[0];
			Windows.Kinect.Joint i = firstBody.Joints [JointType.ShoulderRight];
			Windows.Kinect.Joint j = firstBody.Joints [JointType.ElbowRight];
			Windows.Kinect.Joint k = firstBody.Joints [JointType.HandRight];
			print ("Elbow Z Position: " + j.Position.Z + "\n");
			print ("Hand Z Position: " + k.Position.Z + "\n");
			if (j.Position.Y < k.Position.Y) {
				move.Set (5.0f, 0.0f, 0.0f);
			} else {
				move.Set (-5.0f, 0.0f, 0.0f);
			}
			rb.AddForce (move * speed);
			dataReceived = false;
			//if (j.
			}
		} else {
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");
			Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
			rb.AddForce (movement * speed);
		}
    }


	void OnApplicationQuit()
	{
		if (_Reader != null)
		{
			_Reader.Dispose();
			_Reader = null;
		}

		if (_Sensor != null)
		{
			if (_Sensor.IsOpen)
			{
				_Sensor.Close();
			}

			_Sensor = null;
		}
	}


	private GameObject CreateBodyObject(ulong id)
	{
		GameObject body = new GameObject("Body:" + id);

		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
		{
			GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

			LineRenderer lr = jointObj.AddComponent<LineRenderer>();
			lr.SetVertexCount(2);
			lr.material = BoneMaterial;
			lr.SetWidth(0.05f, 0.05f);

			jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			jointObj.name = jt.ToString();
			jointObj.transform.parent = body.transform;
		}

		return body;
	}

	private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
	{
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
		{
			Kinect.Joint sourceJoint = body.Joints[jt];
			if (jt == Kinect.JointType.SpineBase) {
				// Return Data Value
			}
			Kinect.Joint? targetJoint = null;

			if(_BoneMap.ContainsKey(jt))
			{
				targetJoint = body.Joints[_BoneMap[jt]];
			}

			Transform jointObj = bodyObject.transform.Find(jt.ToString());
			jointObj.localPosition = GetVector3FromJoint(sourceJoint);

			LineRenderer lr = jointObj.GetComponent<LineRenderer>();
			if(targetJoint.HasValue)
			{
				lr.SetPosition(0, jointObj.localPosition);
				lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
				lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
			}
			else
			{
				lr.enabled = false;
			}
		}
	}


}