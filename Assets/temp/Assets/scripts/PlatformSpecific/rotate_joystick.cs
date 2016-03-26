using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class rotate_joystick : MonoBehaviour {


	public float smooth = 10F;
	public float tiltAngle = 30.0F;
	private string hAxis;
	private string vAxis;
	private Vector3 axis = Vector2.zero;

	// Use this for initialization
	void Start () {
		string parentName = transform.parent.parent.name;
		hAxis = parentName + "-H";
		vAxis = parentName + "-V";
	}
	
	// Update is called once per frame
	void Update () {

		float x = CrossPlatformInputManager.GetAxis(hAxis);
		float y = CrossPlatformInputManager.GetAxis(vAxis);
		axis.x = 270.0F + (Mathf.Max(Mathf.Abs (x),Mathf.Abs (y)) * tiltAngle);

		float theta = Mathf.Atan2(y,x);
		
		axis.y = (90 - ((theta * 180) / Mathf.PI)) % 360;


		//axis.z = CrossPlatformInputManager.GetAxis("Horizontal")* tiltAngle;
		Quaternion target = Quaternion.Euler(axis.x, axis.y, axis.z);
		transform.localRotation = Quaternion.Slerp(transform.rotation, target, smooth);//, Time.deltaTime * smooth);
	}
}
