using UnityEngine;
using System.Collections;

public class SlideOutMenu : MonoBehaviour {


	public Vector2 slideTo;

	private RectTransform r;
	private Vector2 size;
	private Vector2 startPos;

	public enum Direction{None, Up, Down, Left, Right}
	public enum State{slideOpen, slideClosed, isOpen, isClosed}
	public Direction slideOpenDirection;
	public Direction slideCloseDirection;
	public State state = State.isClosed;
	[Range (1,10)]public int speed = 8;


	public void SetState(string s)
	{
		switch (s)
		{
			case "open":
				state = State.slideOpen;
				break;
			case "close":
				state = State.slideClosed;
				break;

		}

	}

	void Start(){
		r = GetComponent<RectTransform>();
		size = r.anchorMax - r.anchorMin;
		startPos = r.anchorMin;
	}

	void Update(){
		if (TouchHandler.touches.Length > 0){
			if (TouchHandler.touches[0].GetAction () == TouchHandler.actions.Swipe){
				if (state == State.isClosed && (int)TouchHandler.touches[0].GetSwipeDirection() == (int)slideOpenDirection){
					state = State.slideOpen;
				}
				else if (state == State.isOpen && (int)TouchHandler.touches[0].GetSwipeDirection() == (int)slideCloseDirection){
					state = State.slideClosed;
				}
			}
		}

		Slide();

	}

	void Slide(){
		if (state == State.slideOpen){
			r.anchorMin = Vector2.Lerp(r.anchorMin, slideTo, Time.deltaTime * speed);
			r.anchorMax = r.anchorMin + size;
			if (Vector2.Distance(r.anchorMin, slideTo) < 0.005f){
				state = State.isOpen;
				r.anchorMin = slideTo;
				r.anchorMax = r.anchorMin + size;
			}
		}else if (state == State.slideClosed){
			r.anchorMin = Vector2.Lerp(r.anchorMin, startPos, Time.deltaTime * speed);
			r.anchorMax = r.anchorMin + size;
			if (Vector2.Distance(r.anchorMin, startPos) < 0.005f){
				state = State.isClosed;
				r.anchorMin = startPos;
				r.anchorMax = r.anchorMin + size;
			}
		}
		

	}

}
