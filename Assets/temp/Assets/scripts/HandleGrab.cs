using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HandleGrab : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {
	
	
	public RectTransform resizeableRect;
	public sides side;


	public enum sides{Top, Right, Bottom, Left};
	private Dictionary<sides, float> align;
	
	private float distance = 50f;
	private Vector3 difference = Vector3.zero;
	private Transform newObj;
	private RectTransform newRect;
	private bool itemClicked = false;
	private bool itemDrag = false;
	private Vector2 clickPosition;
	
	
	
	void Start()
	{
		align = new Dictionary<sides, float>(){
			{sides.Top, resizeableRect.anchorMax.y},
			{sides.Left, resizeableRect.anchorMin.x},
			{sides.Bottom, resizeableRect.anchorMin.y},
			{sides.Right, resizeableRect.anchorMax.x},
		};
		
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		itemClicked = true;
		clickPosition = Input.mousePosition;


		newObj = transform;
		newRect = (RectTransform)transform;
		difference = Camera.main.WorldToScreenPoint(newRect.position) - Input.mousePosition;
		difference = Vector2.zero;
	}
	
	public void OnPointerUp(PointerEventData eventData)
	{
		itemClicked = false;
	}
	
	
	void Update()
	{

	}
	

	public void OnMouseDrag()
	{
		

		if (Vector2.Distance(Input.mousePosition, clickPosition) > 10f)
			itemDrag = true;
		
		if (itemDrag){
			Vector2 newAnchor;
			
			if (side == sides.Left || side == sides.Right)
				newAnchor = new Vector2(Mathf.Clamp((Input.mousePosition.x + difference.x) / Screen.width, 0f, 1.0f), 0.5f);
			else
				newAnchor = new Vector2(0.5f, Mathf.Clamp((Input.mousePosition.y + difference.y) / Screen.height, 0f, 1.0f));

			//newRect.pivot = new Vector2(0.5f, 0.5f);
			////newRect.anchorMin = newAnchor;
			//newRect.anchorMax = newAnchor;
			//newObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			switch (side){
			case sides.Left:
				resizeableRect.anchorMin = new Vector2(newAnchor.x, resizeableRect.anchorMin.y);
				break;
			case sides.Right:
				resizeableRect.anchorMax = new Vector2(newAnchor.x, resizeableRect.anchorMax.y);
				break;
			case sides.Top:
				resizeableRect.anchorMax = new Vector2(resizeableRect.anchorMax.x, newAnchor.y);
				break;
			case sides.Bottom:
				resizeableRect.anchorMin = new Vector2(resizeableRect.anchorMin.x, newAnchor.y);
				break;
			}


		}
			

		
	}
	
}
