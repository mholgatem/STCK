using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/* 
*** TOUCH HANDLER CLASS ***
Aggregates all of the Input.touches and creates a new TouchInstance for each,
then stores them in TouchHandler.touches[]

[ TouchInstance ] - contains all of the extended properties such as 'action',
'tapCount', 'distanceTraveled', etc...

[ TouchExtension ] - extends Input.touches with the getExtended method,
returns TouchInstance or TouchInstance[]

[ TouchCreator ] - is used solely for converting mouse clicks to touch events

 */





[DisallowMultipleComponent]
[ScriptOrder(-1000)]
[RequireComponent (typeof(UnityEngine.EventSystems.StandaloneInputModule))]
public class TouchHandler : MonoBehaviour{

	public bool displayStats = false;

	[Tooltip("Time allowed between taps before reset")]
	public float tapTimeout = 0.15f;
	public static float _tapTimeout;

	[Range(0.01f, 1f)]
	public float longPressTime = 0.5f;
	public static float _longPressTime;

	public UnitTypes measureUnits;
	public static UnitTypes _measureUnits;

	[Tooltip("Speed (measureUnits/Second) a touch can travel before swipe action")]
	public float swipeThreshold;
	public static float _swipeThreshold;

	[Tooltip("Distance a touch can move before drag action")]
	public float dragThreshold;
	public static float _dragThreshold;

	public static float _pinchDistance;
	public static float _pinchRatio;

	public enum actions{None, Down, LongPress, Tap, Swipe, Drag}; 
	public enum directions{None, Up, Down, Left, Right};
	public enum UnitTypes{Millimeter, Centimeter, Inch, Pixel};

	public static Touch[] touches = new Touch[0];
	public static List<TouchInstance> _touchCache = new List<TouchInstance>();


	// Use mouse to simulate touches
	public bool simulateTouchWithMouse = false;
	public TouchCreator tc = new TouchCreator();
	private Touch fakeTouch;


	void Start () {
		screenPixelsPerCm = Screen.dpi; //initialize
		_measureUnits = measureUnits;
		_tapTimeout = tapTimeout;
		_longPressTime = longPressTime;
		_dragThreshold = UnitsToPixels(dragThreshold);
		_swipeThreshold = swipeThreshold;
		fakeTouch = tc.CreateEmpty();

	}

	public void assignTouches(){

		if (!simulateTouchWithMouse){
			touches = (Touch[])Input.touches.Clone ();
		}else{
			//simulate touch with mouse
			if(Input.GetMouseButtonDown(0)){
				fakeTouch = tc.Begin ();
				touches = new Touch[]{fakeTouch};
			}else if (Input.GetMouseButton(0)){
				fakeTouch = tc.Update ();
				touches[0] = fakeTouch;
			}else if (Input.GetMouseButtonUp(0)){
				fakeTouch = tc.End();
				touches[0] = fakeTouch;
			}else{
				fakeTouch = tc.CreateEmpty ();
				touches = new Touch[]{};
			}
		}
	}

	void Update () {

		assignTouches();

		//compare _touchCache to latest touches
		foreach(Touch t in touches){
			if (!_touchCache.Exists ( x => x.fingerId == t.fingerId )){
				_touchCache.Add (new TouchInstance(t));
				_touchCache.OrderBy(x => x.fingerId);
			}
		}

		//update TouchInstances in reverse 
		//so they can remove themselves when finished
		if (_touchCache.Count > 0){
			for(int i = _touchCache.Count - 1; i >= 0; i--){
				_touchCache[i].Update ();
			}
		}

	}
	
	void OnGUI()
	{
#if UNITY_EDITOR
		if (displayStats){
			Color fontColor = Color.white;
			int w = Screen.width, h = Screen.height;
			
			GUIStyle style = new GUIStyle();
			Rect rect = new Rect(5, 5, w, h / 50);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h / 50;
			style.normal.textColor = fontColor;

			string text = string.Format("Is Pinching:\t{0}", Pinch.IsActive());
			text += string.Format("\nPinch Dist.:\t{0:0.0}px", Pinch.GetDistance());
			text += string.Format("\nPinch Ratio:\t{0:0%}", Pinch.GetRatio());
			text += "\n----------------------------------";
			int y = h/10;
			GUI.Label(rect, text, style);
			for (int i = 0; i < _touchCache.Count; i++){
				rect = new Rect(5, (h / 50) * i * 9 + y, w, h / 50);
				style.alignment = TextAnchor.UpperLeft;
				style.fontSize = h / 50;
				style.normal.textColor = fontColor;
				text = string.Format("Touch ID:\t{0}", _touchCache[i].fingerId);
				if (_touchCache.Count > 0){
					text += string.Format("\nPhase:\t{0}", _touchCache[i].phase);
					text += string.Format("\nAction:\t{0}", _touchCache[i].action);
					text += string.Format("\nTaps:\t{0}", _touchCache[i].tapCount);
					text += string.Format("\nDistance:\t{0}", _touchCache[i].distanceTraveled);
					text += string.Format("\nSpeed:\t{0}", _touchCache[i].speed);
					text += string.Format("\nTime:\t{0:0.00} s.", _touchCache[i].totalPressTime);
					text += "\n----------------------------------";
				}
				GUI.Label(rect, text, style);
			}
		}
#endif
	}

	//HELPERS
	private const float inchesToCentimeters = 2.54f;
	private static float _screenPixelsPerCm = 0f;
	public static float screenPixelsPerCm
	{
		get
		{
			return _screenPixelsPerCm;
		}
		
		set
		{
			
			float setDpi = value;
			float fallbackDpi = 72f;
			#if UNITY_ANDROID
			// Android MDPI setting fallback
			// http://developer.android.com/guide/practices/screens_support.html
			fallbackDpi = 160f;
			#elif (UNITY_WP8 || UNITY_WP8_1 || UNITY_WSA || UNITY_WSA_8_0)
			// Windows phone is harder to track down
			// http://www.windowscentral.com/higher-resolution-support-windows-phone-7-dpi-262
			fallbackDpi = 92f;
			#elif UNITY_IOS
			// iPhone 4-6 range
			fallbackDpi = 326f;
			#endif
			
			_screenPixelsPerCm = setDpi == 0f ? fallbackDpi / inchesToCentimeters : setDpi / inchesToCentimeters;
		}
	}

	/// <summary>
	/// Convert Units to pixels based on screen dpi
	/// </summary>
	/// <returns>float</returns>
	/// <param name="units"> The number of units to convert</param>
	public static float UnitsToPixels(float units){
		float conversion = 1f;
		
		switch (_measureUnits)
		{
		case UnitTypes.Centimeter:
			conversion = _screenPixelsPerCm;
			break;
		case UnitTypes.Millimeter:
			conversion = _screenPixelsPerCm / 10f;
			break;
		case UnitTypes.Inch:
			conversion = _screenPixelsPerCm * 2.54f;
			break;
		case UnitTypes.Pixel:
			conversion = 1f;
			break;
		}
		
		return units * conversion;
	}
	
	/// <summary>
	/// Convert pixels to Units based on screen dpi
	/// </summary>
	/// <returns>float</returns>
	public static float PixelsPerUnit(){
	
		switch (_measureUnits)
		{
		case UnitTypes.Centimeter:
			return _screenPixelsPerCm;
		case UnitTypes.Millimeter:
			return _screenPixelsPerCm * 10f;
		case UnitTypes.Inch:
			return _screenPixelsPerCm / 2.54f;
		case UnitTypes.Pixel:
		default:
			return 1f;
		}	
	}

	//  RAYCAST FOR TOUCHED OBJECTS
	/// <summary>
	/// <para>Returns TouchInstance[].</para>
	/// <para>Same method as: Input.touches.GetRayHit3D();</para>
	/// </summary>
	/// <returns>The ray hit3 d.</returns>
	public static TouchInstance[] GetRayHit3D(){
		foreach(TouchInstance t in _touchCache){
			t.CheckRayHit();
		}
		return _touchCache.ToArray();
	}

	//  TWO FINGER SWIPE
	/// <summary>
	/// <para>Get direction of two finger swipe.</para>
	/// <para>Returns directions.None if not applicable.</para>
	/// </summary>
	public static class DoubleSwipe{
		public static TouchHandler.directions Direction(){
			if (_touchCache.Count == 2 &&
			    _touchCache[0].action == TouchHandler.actions.Swipe &&
			    _touchCache[1].action == TouchHandler.actions.Swipe){
				return _touchCache[0].swipeDirection;
			}

			return TouchHandler.directions.None;
		}
	}

	//  PINCH CLASS
	/// <summary>
	/// <para>Pinch Class, contains methods:</para>
	/// <para>isActive(), GetDistance(), GetRatio()</para>
	/// </summary>
	public static class Pinch{

		/// <summary>
		/// Determines if user is pinching. Requires exactly 2 touches.
		/// </summary>
		/// <returns><c>true</c> if is active the specified pinchDelay; otherwise, <c>false</c>.</returns>
		/// <param name="pinchDelay">time to allow the user to perform some other action</param>
		public static bool IsActive(float pinchDelay = .2f){
			if (_touchCache.Count == 2){
				return (_touchCache[1].action == TouchHandler.actions.Drag ||
				        _touchCache[0].action == TouchHandler.actions.Drag) &&
						(_touchCache[0].totalPressTime > pinchDelay &&
					 	_touchCache[1].totalPressTime > pinchDelay);
			}
			return false;
		}

		/// <summary>
		/// <para>Gets Distance of pinch in pixels.</para>
		/// <para><c>Positive: </c>Touches moving away from each other.</para>
		/// <para><c>Negative: </c>Touches moving toward each other.</para>
		/// </summary>
		/// <returns>float</returns>
		public static float GetDistance(){
			if (IsActive()){
				return Vector2.Distance(_touchCache[0].currentPos, _touchCache[1].currentPos) - 
						Vector2.Distance (_touchCache[0].startPos, _touchCache[1].startPos);
			}
			return 0;
		}

		/// <summary>
		/// <para>Gets % that pinch has changed since start.</para>
		/// <para> </para>
		/// <para><c>1.0: </c> No change.</para>
		/// <para><c>Greater 1: </c>Touches moving away from each other.</para>
		/// <para><c>Less 1: </c>Touches moving toward each other.</para>
		/// </summary>
		/// <returns>float</returns>
		public static float GetRatio(){
			if (IsActive()){
				return Vector2.Distance(_touchCache[0].currentPos, _touchCache[1].currentPos) / 
						Vector2.Distance (_touchCache[0].startPos, _touchCache[1].startPos);
			}
			return 1;
		}
	}
	
}

