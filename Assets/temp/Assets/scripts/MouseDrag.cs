using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
/*
public class MouseDrag : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {


	public Toggle snapToGrid;
	public GameObject controlEditor;

	private float distance = 50f;
	private Transform newObj;
	private GameObject controlPanel;
	private Vector2 resizeStartPos;
	private RectTransform newRect;

	private bool menuOpen = false;
	private bool itemClicked = false;
	private GameObject editorContent;
	private Vector2 clickPosition;
	private Vector3 localScaleStart;

	

	void Start()
	{
		controlPanel = GameObject.Find ("3D controls");
		if (snapToGrid == null)
			snapToGrid = GameObject.Find ("SnapGridToggle").GetComponent<Toggle>();
		if (controlEditor == null)
			controlEditor = GameObject.Find ("controlEditor");
		editorContent = controlEditor.transform.GetChild (0).gameObject;
		
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		clickPosition = Input.mousePosition;
		localScaleStart = transform.localScale;
		itemClicked = true;

		newObj = transform;
		newRect = (RectTransform)transform;

	}

	public void OnPointerUp(PointerEventData eventData)
	{
		//release the object
		newObj = null;
		newRect = null;
		itemClicked = false;

	}


	void Update()
	{

		if (itemClicked && !editorContent.activeInHierarchy)
		{
			if (TouchHandler.touches.Length > 0)
			{
				//Double-Tap to Delete
				if (TouchHandler.touches[0].tapCount > 1){
					Destroy (this.gameObject);
				}

				//Longpress to open item config
				if (TouchHandler.touches[0].action == touchInstance.actions.LongPress){

					TouchHandler.touches[0].overrideAction = touchInstance.actions.None;
					if (tag != "Keyboard"){
						menuOpen = true;
						editorContent.SetActive(true);
						float scaleSize = Mathf.Max (Screen.width,Screen.height) / 1000;

						//controlEditor.transform.localScale = Vector3.one * scaleSize;
						modifyControls _modifyControls = controlEditor.GetComponent<modifyControls>();
						_modifyControls.controlToModify = this.gameObject;
						_modifyControls.setOptions();
						//RectTransform controlRect = (RectTransform)controlEditor.transform;

						//controlRect.anchoredPosition = new Vector2(Mathf.Clamp(Input.mousePosition.x , 0, Screen.width - (controlRect.rect.width)),
						                                           //Mathf.Clamp(Input.mousePosition.y - (controlRect.rect.height / 2), 0, Screen.height - (controlRect.rect.height)));

					}

					if(tag == "Keyboard"){
						createSettingsList.setKeyboardLayoutState(true, this.gameObject);
						//GameObject.Find ("menuToggle").SetActive(false);
						//controlPanel.SetActive(false);

					}
				}
			}

			// If 2 touches -> Resize
			if (Input.touchCount > 1 && TouchHandler.isPinching())
			{

				float scaleSize = Mathf.Clamp(localScaleStart.x * TouchHandler.pinchRatio(), 1f, 10);

				transform.localScale = Vector3.one * scaleSize;
				
			}
		}
		
	}
	
	
	public void OnMouseDrag()
	{

		if (newRect && !TouchHandler.isPinching() && !editorContent.activeInHierarchy)
		{

			Vector2 newAnchor = new Vector2(TouchHandler.touches[0].currentPos.x / Screen.width, TouchHandler.touches[0].currentPos.y / Screen.height);

			if (snapToGrid.isOn)
				newAnchor = new Vector2(Mathf.Round (newAnchor.x * 30f) / 30f,
				                        Mathf.Round(newAnchor.y * 30f) / 30f);
			Vector2 pivotPos = new Vector2(0.5f, 0.5f);
			if (newAnchor.x > 0.66f)
				pivotPos.x = 1f;
			if (newAnchor.x < .33f)
				pivotPos.x = 0f;
			if (newAnchor.y > 0.66f)
				pivotPos.y = 1f;
			if (newAnchor.y < .33f)
				pivotPos.y = 0f;

			newRect.pivot = pivotPos;
			newRect.anchorMin = newAnchor;
			newRect.anchorMax = newAnchor;
			newObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			

		}

	}

}

*/