using UnityEngine;
using System.Collections;

public class Examples : MonoBehaviour {
	
	void Update () {

		//Pinching
		if (Input.touches.IsPinching()){
			Camera.main.fieldOfView = Input.touches.PinchRatio() * 60f;
		
		//Two-Finger Swipe
		}else if (Input.touches.DoubleSwipeDirection() != TouchHandler.directions.None)
		{
			if(Input.touches.DoubleSwipeDirection() == TouchHandler.directions.Right)
			{
				Camera.main.transform.Rotate(Vector3.up * Time.deltaTime * 90);
			}else{
				Camera.main.transform.Rotate(Vector3.down * Time.deltaTime * 90);
			}
		
		//All Other Actions
		}else{
			CheckActions();
		}
	}
	
	void CheckActions(){
		foreach(TouchInstance t in Input.touches.GetRayHit3D()){
			if (t.objectTouched.rigidbody != null){
				switch (t.action){
				case TouchHandler.actions.Drag:
					t.objectTouched.rigidbody.transform.position = new Vector3(t.objectTouched.point.x, t.objectTouched.rigidbody.transform.position.y, t.objectTouched.point.z);
					//use this to avoid accidentally swiping
					t.overrideAction = TouchHandler.actions.Drag;
					break;
				case TouchHandler.actions.Tap:
					t.objectTouched.rigidbody.transform.Rotate(Vector3.up * Time.deltaTime * 90);
					break;
				case TouchHandler.actions.LongPress:
					t.objectTouched.rigidbody.transform.Rotate(Vector3.down * Time.deltaTime * 90);
					break;
				case TouchHandler.actions.Swipe:
					t.objectTouched.rigidbody.AddForce(t.velocity * 5);
					t.objectTouched.rigidbody.AddRelativeTorque(t.velocity * 5);
					break;
				}
			}
		}
	}

}
