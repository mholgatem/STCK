using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class rotate_wheel : MonoBehaviour {
	
	
	public float smooth = 10F;
	public float tiltAngle = 45.0F;
	private string hAxis,vAxis;
	private Vector3 axis = Vector2.zero;
	
	// Use this for initialization
	void Start () {
		hAxis = transform.name + "-H";
		vAxis = transform.name + "-V";

	}
	
	// Update is called once per frame
	void Update () {
		
		float x = CrossPlatformInputManager.GetAxis(hAxis);
		float y = CrossPlatformInputManager.GetAxis(vAxis);
		//axis.x = (Mathf.Max(Mathf.Abs (x),Mathf.Abs (y)) * tiltAngle);
		
		float theta = Mathf.Atan2(y,x);
		
		axis.z = tiltAngle * -x;
		
		
		//axis.z = CrossPlatformInputManager.GetAxis("Horizontal")* tiltAngle;
		Quaternion target = Quaternion.Euler(axis.x, axis.y, axis.z);
		transform.localRotation = Quaternion.Slerp(transform.rotation, target, smooth);//, Time.deltaTime * smooth);
	}
}
