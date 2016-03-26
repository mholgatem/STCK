using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class modifyControls : MonoBehaviour {

	public GameObject controlToModify;
	public ToggleGroup colorGroup;
	public ToggleGroup imageGroup;
	public GameObject textGrid;
	public GameObject colorGrid;
	public GameObject iconGrid;
	
	public GameObject inputOption;
	public GameObject dpadOption;
	public GameObject textOption;
	public GameObject colorOption;
	public GameObject imageOption;
	public GameObject keyDropdown;
	public Slider scaleSlider;
	public Text controlTitle;

	public List<GameObject> editorOptions;


	// Use this for initialization
	void Start () {
		editorOptions.Add (textGrid);
		editorOptions.Add (colorGrid);
		editorOptions.Add (iconGrid);
		editorOptions.Add (inputOption);
		editorOptions.Add (dpadOption);
		editorOptions.Add (textOption);
		editorOptions.Add (colorOption);
		editorOptions.Add (imageOption);
		editorOptions.Add (keyDropdown);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Color getColor(GameObject obj){

		Renderer[] controlChildren = getMaterialObject (obj);
		foreach(Renderer child in controlChildren)
		{
			return child.material.color;
		}
		if (obj.tag == "UI_Button")
		{
			Image[] icons = obj.transform.GetComponentsInChildren<Image>();
			foreach(Image icon in icons)
			{
				switch (icon.name)
				{
				case "Button_hitbox":
					return icon.color;
				default:
					break;
				}
			}
		}
		return Color.white;
	}

	public void setKey(int selectionNumber){
		if (selectionNumber == 0){
			inputOption.SetActive (true);
		}
		else
			inputOption.GetComponent<InputField>().text = keyDropdown.GetComponent<Dropdown>().options[selectionNumber].text;

	}

	public void setColor(bool isSelected){

		if (isSelected)
		{
			Color currentColor;
			Toggle active = colorGroup.GetActive();
			currentColor = active.colors.normalColor;
			Renderer[] controlChildren = getMaterialObject (controlToModify);
			foreach(Renderer child in controlChildren)
			{
				child.material.color = currentColor;
			}
			Image[] hitBoxes = controlToModify.transform.GetComponentsInChildren<Image>();
			foreach(Image hitBox in hitBoxes)
			{
				if (hitBox.name == "hitbox" || hitBox.name == "Button_hitbox")
					hitBox.color = currentColor;
			}
			if (controlToModify.tag == "UI_Button")
			{
				Image[] icons = controlToModify.transform.GetComponentsInChildren<Image>();
				foreach(Image icon in icons)
				{
					switch (icon.name)
					{
					case "Button_hitbox":
						icon.color = currentColor;
						break;
					default:
						icon.color = Color.white;
						break;
					}
				}
			}
		}
	}

	public void setIcon(bool isSelected){
		
		if (isSelected)
		{
			Sprite currentImage;
			Toggle active = imageGroup.GetActive ();
			Sprite background = active.GetComponent<Image>().sprite;
			Image[] icons = controlToModify.transform.GetComponentsInChildren<Image>();
			foreach(Image icon in icons)
			{
				switch (icon.name)
				{
				case "Button_hitbox":
					icon.sprite = active.image.sprite;
					break;
				default:
					icon.sprite = background;
					break;
				}
			}
		}
		
	}

	public void setCommand(string newCommand)
	{
		controlToModify.GetComponent<propertyContainer>().command = newCommand;
	}

	public void setDpadCommand(GameObject inputName)
	{
		string tempText = inputName.GetComponent<InputField>().text;
		switch (inputName.name)
		{
		case "up":
			controlToModify.GetComponent<propertyContainer>().dpad.up = tempText;
			break;
		case "down":
			controlToModify.GetComponent<propertyContainer>().dpad.down = tempText;
			break;
		case "left":
			controlToModify.GetComponent<propertyContainer>().dpad.left = tempText;
			break;
		case "right":
			controlToModify.GetComponent<propertyContainer>().dpad.right = tempText;
			break;
		}

	}

	public Renderer[] getMaterialObject(GameObject control)
	{
		switch (control.tag)
		{
		case "Dpad":
			return control.transform.GetComponentsInChildren<Renderer>();
		case "Button":
			return control.transform.GetComponentsInChildren<Renderer>();
		case "UI_Button":
			return control.transform.GetComponentsInChildren<Renderer>();
		case "Joystick":
			return control.transform.GetChild(0).FindChild("joystick").GetComponents<Renderer>();
		case "Wheel":
			return control.transform.GetComponentsInChildren<Renderer>();
		default:
			return control.transform.GetComponentsInChildren<Renderer>();
		}	
	}

	public void setText(string caption){

		controlToModify.transform.GetComponentInChildren<Text>().text = caption;
		
	}


	public void setScale(float size)
	{
		controlToModify.transform.localScale = Vector3.one * (size/2);
	}

	public void setDropdownValue(string tempCommand, Dropdown list){

		if (string.IsNullOrEmpty(tempCommand))
			list.value = 0;
		else
			list.value = 1;
		for (int i = 0; i < list.options.Count; i++){
			if (tempCommand == list.options[i].text){
				list.value = i;
				break;
			}
		}
	}

	public void clearOptions(){
		foreach(GameObject item in editorOptions)
		{
			item.SetActive(false);
		}

	}

	public void setOptions()
	{

		controlTitle.text = controlToModify.name;
		scaleSlider.value = controlToModify.transform.localScale.x * 2;

		clearOptions();

		switch (controlToModify.tag)
		{
			case "Dpad":
				{
					Debug.LogWarning("Dpad");
					string tempCommand = controlToModify.GetComponent<propertyContainer>().dpad.up;
					setDropdownValue(tempCommand, dpadOption.transform.GetChild(1).FindChild("DropUp").GetComponent<Dropdown>());
					tempCommand = controlToModify.GetComponent<propertyContainer>().dpad.down;
					setDropdownValue(tempCommand, dpadOption.transform.GetChild(1).FindChild("DropDown").GetComponent<Dropdown>());
					tempCommand = controlToModify.GetComponent<propertyContainer>().dpad.left;
					setDropdownValue(tempCommand, dpadOption.transform.GetChild(1).FindChild("DropLeft").GetComponent<Dropdown>());
					tempCommand = controlToModify.GetComponent<propertyContainer>().dpad.right;
					setDropdownValue(tempCommand, dpadOption.transform.GetChild(1).FindChild("DropRight").GetComponent<Dropdown>());
					dpadOption.SetActive (true);
					colorOption.SetActive (true);
					break;
				}

			case "Button":
				{
					//inputOption.SetActive (true);
					
					string tempCommand = controlToModify.GetComponent<propertyContainer>().command;
					setDropdownValue(tempCommand, keyDropdown.GetComponent<Dropdown>());
					keyDropdown.SetActive(true);
					colorOption.SetActive (true);
					textOption.SetActive (true);
					textGrid.SetActive(true);
					if (controlToModify.GetComponentInChildren<Text>().text.Length > 0)
					{
						string tempText = controlToModify.GetComponentInChildren<Text>().text;
						textGrid.transform.GetChild(0).GetComponentInChildren<Toggle>().isOn = true;
						textGrid.transform.GetChild (0).GetComponentInChildren<InputField>().text = tempText;
					}else{
						textGrid.transform.GetChild (0).GetComponentInChildren<Toggle>().isOn = false;
					}
					textGrid.SetActive(false);
					break;
				}

			case "UI_Button":
				{
					//inputOption.SetActive (true);
					
					string tempCommand = controlToModify.GetComponent<propertyContainer>().command;
					setDropdownValue(tempCommand, keyDropdown.GetComponent<Dropdown>());
					keyDropdown.SetActive(true);
					colorOption.SetActive (true);
					textOption.SetActive (true);
					imageOption.SetActive (true);
					textGrid.SetActive(true);
					if (controlToModify.GetComponentInChildren<Text>().text.Length > 0)
					{
						string tempText = controlToModify.GetComponentInChildren<Text>().text;
						textGrid.transform.GetChild(0).GetComponentInChildren<Toggle>().isOn = true;
						textGrid.transform.GetChild (0).GetComponentInChildren<InputField>().text = tempText;
					}else{
						textGrid.transform.GetChild(0).GetComponentInChildren<Toggle>().isOn = false;
					}
					textGrid.SetActive(false);
					break;
				}

			case "Joystick":
				{
					colorOption.SetActive (true);
					break;

				}
			case "Wheel":
				{
					colorOption.SetActive (true);
					break;
				}
		}
	}

	public void forceClose()
	{
		transform.GetChild (0).gameObject.SetActive(false);
	}

	public void hideMenu()
	{
		if (!colorGroup.isActiveAndEnabled && !imageGroup.isActiveAndEnabled && !textGrid.activeInHierarchy) 
			transform.GetChild (0).gameObject.SetActive(false);
		else
			setOptions();
	}
}
