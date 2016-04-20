using UnityEngine;
using System.Collections;

public class Examples : MonoBehaviour {
	
	void Update () {

		//Pinching
		if (Input.touches.IsPinching()){
			Camera.main.fieldOfView = Input.touches.GetPinchRatio() * 60f;
		
		//Two-Finger Swipe
		}else if (Input.touches.GetDoubleSwipeDirection() != TouchHandler.directions.None)
		{
			if(Input.touches.GetDoubleSwipeDirection() == TouchHandler.directions.Right)
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
		foreach(TouchInstance t in Input.touches.CheckRayHit3D()){
			if (t.raycastHit.rigidbody != null){
				switch (t.action){
				case TouchHandler.actions.Drag:
					t.raycastHit.rigidbody.transform.position = new Vector3(t.raycastHit.point.x, t.raycastHit.rigidbody.transform.position.y, t.raycastHit.point.z);
					//use this to avoid accidentally swiping
					t.overrideAction = TouchHandler.actions.Drag;
					break;
				case TouchHandler.actions.Tap:
					t.raycastHit.rigidbody.transform.Rotate(Vector3.up * Time.deltaTime * 90);
					break;
				case TouchHandler.actions.LongPress:
					t.raycastHit.rigidbody.transform.Rotate(Vector3.down * Time.deltaTime * 90);
					break;
				case TouchHandler.actions.Swipe:
					t.raycastHit.rigidbody.AddForce(t.velocity * 5);
					t.raycastHit.rigidbody.AddRelativeTorque(t.velocity * 5);
					break;
				}
			}
		}
	}

}
