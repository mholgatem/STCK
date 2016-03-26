using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static class ExtensionMethods
{
	//Even though they are used like normal methods, extension
	//methods must be declared static. Notice that the first
	//parameter has the 'this' keyword followed by a Transform
	//variable. This variable denotes which class the extension
	//method becomes a part of.
	public static Toggle GetActive(this ToggleGroup aGroup)
	{
		return aGroup.ActiveToggles().FirstOrDefault();
	}


}

public class WorldRaycaster : GraphicRaycaster {
	
	[SerializeField]
	private int SortOrder = 0;
	
	public override int sortOrderPriority {
		get {
			return SortOrder;
		}
	}
}