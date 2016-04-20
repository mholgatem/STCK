using UnityEngine;
using System.Collections.Generic;

/* 
*** TOUCH INSTANCE CLASS ***
Property container for each touch object
 */



public class TouchInstance {
	
	public Touch _touch;
	public int fingerId;
	public int tapCount = 0;
	public float startTime;
	public float currentPressTime; //press time of current tap
	public float totalPressTime; //sum of press time over multiple taps
	public float distanceTraveled; //distance in drag units
	public float magnitude;
	public float speed;
	public Vector2 velocity;

	public bool hasMoved = false;
	public Vector2 startPos;
	public Vector2 currentPos;
	public TouchPhase phase;
	public RaycastHit raycastHit;
	public RaycastHit2D raycastHit2D;
	
	private float tapTimer;
	private Vector2 swipeStartPos;
	
	public TouchHandler.actions action;
	public TouchHandler.actions overrideAction;
	public TouchHandler.directions swipeDirection;
	
	//INITIALIZE NEW INSTANCE
	public TouchInstance(){}
	public TouchInstance(Touch t){
		_touch = t;
		fingerId = t.fingerId;
		startPos = _touch.position;
		swipeStartPos = _touch.position;
		startTime = Time.time;
		action = TouchHandler.actions.Down;
		overrideAction = TouchHandler.actions.None;
	}

	//SWIPE DIRECTION
	private TouchHandler.directions GetSwipeDirection(){
		Vector2 direction = _touch.position - swipeStartPos;
	
		if (Mathf.Abs (direction.x) > Mathf.Abs(direction.y)){
			if (direction.x < 0)
				return TouchHandler.directions.Left;
			else
				return TouchHandler.directions.Right;
		}else if (Mathf.Abs (direction.y) > Mathf.Abs(direction.x)){
			if (direction.y < 0)
				return TouchHandler.directions.Down;
			else
				return TouchHandler.directions.Up;
		}
		return TouchHandler.directions.None;
	}

	//SET ACTION
	private void SetAction(){

		if (hasMoved){
			//SWIPE
			if (speed > TouchHandler._swipeThreshold){
				action = TouchHandler.actions.Swipe;
				swipeDirection = GetSwipeDirection();
			}else{
				swipeStartPos = currentPos;
			}
			//DRAG
			if (action != TouchHandler.actions.Swipe){
				action = TouchHandler.actions.Drag;
			}

		}else{
			//TAP
			if (_touch.phase.IsDone() && 
			    currentPressTime < TouchHandler._longPressTime){
				action = TouchHandler.actions.Tap;
			}
			//LONG-PRESS
			if (currentPressTime > TouchHandler._longPressTime){
				action = TouchHandler.actions.LongPress;
			}

		}
	}

	//JUST (RE)TAPPED
	public void AddTap(){
		tapCount++;
		tapTimer = 0f;
		currentPressTime = 0f;
		distanceTraveled = 0f;
	
		startPos = _touch.position;
		swipeStartPos = _touch.position;
		overrideAction = TouchHandler.actions.None;
		hasMoved = false;

	}


	// RETURN CORRECT TOUCH
	public Touch GetTouchById(int fingerId){
		foreach (Touch t in TouchHandler.touches){
			if (t.fingerId == fingerId)
				return t;
		}
		return _touch;

	}

	// CHECK IF USER IS TOUCHING SOMETHING 3D
	public void CheckRayHit(){
		Ray ray = Camera.main.ScreenPointToRay (currentPos);
		Physics.Raycast(ray, out raycastHit);
	}

	// CHECK IF USER IS TOUCHING SOMETHING 2D
	public void CheckRayHit2D(){
		Vector2 ray = Camera.main.ScreenToWorldPoint (currentPos);
		raycastHit2D = Physics2D.Raycast(ray, Vector2.zero);
	}

	// UPDATE CALLED BY TouchHandler
	public void Update(){

		//refresh _touch
		_touch = GetTouchById (fingerId);
		phase = _touch.phase;

		//add a new tap
		if (phase == TouchPhase.Began){ 
			AddTap ();}

		//update properties
		currentPos = _touch.position;
		magnitude = _touch.deltaPosition.magnitude;
		speed = magnitude.PixelsToTouchUnits() / _touch.deltaTime;
		velocity = _touch.deltaPosition;


		//Moved > dragThreshold
		if (Vector2.Distance(startPos, currentPos) > TouchHandler._dragThreshold){
			hasMoved = true;
			distanceTraveled += magnitude / TouchHandler._dragThreshold;
		}

		//start checking for timeout
		// if touch is done
		if (_touch.phase.IsDone()){
			tapTimer += Time.deltaTime;
			if (tapTimer > TouchHandler._tapTimeout){
				TouchHandler._touchCache.Remove(this);
			}
		}else{
			totalPressTime += Time.deltaTime;
			currentPressTime += Time.deltaTime;
		}

		//ACTIONS
		if (overrideAction != TouchHandler.actions.None){
			action = overrideAction;
		}else{ 
			SetAction();
		}
		
		
	}
	
}

