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
	public RaycastHit objectTouched;
	
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
	private TouchHandler.directions getSwipeDirection(){
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

	//ACTION == NONE OR DOWN
	private bool noneOrDown(){
		return (action == TouchHandler.actions.None || action == TouchHandler.actions.Down);
	}

	//SET ACTION
	private void setAction(){

		if (hasMoved){
			//SWIPE
			if (speed > TouchHandler._swipeThreshold){
				action = TouchHandler.actions.Swipe;
				swipeDirection = getSwipeDirection();
			}else{
				swipeStartPos = currentPos;
			}
			//DRAG
			if (action != TouchHandler.actions.Swipe){
				action = TouchHandler.actions.Drag;
			}

		}else{
			//TAP
			if (_touch.phase.isDone() && 
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

	}

	//RETURN CORRECT TOUCH
	public Touch getTouchById(int fingerId){
		foreach (Touch t in TouchHandler.touches){
			if (t.fingerId == fingerId)
				return t;
		}
		return _touch;

	}

	//CHECK IF USER IS TOUCHING SOMETHING
	public void CheckRayHit(){
		Ray ray = Camera.main.ScreenPointToRay (currentPos);
		Physics.Raycast(ray, out objectTouched);
	}

	//UPDATE CALLED BY TouchHandler
	public void Update(){

		//refresh _touch
		_touch = getTouchById (fingerId);
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
		if (_touch.phase.isDone()){
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
			setAction();
		}
		
		
	}
	
}

