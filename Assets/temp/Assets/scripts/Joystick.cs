using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical
		}

		public int MovementRange = 50;
		public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use

		private string horizontalAxisName, verticalAxisName; // The name given to the horiz/vert axis for the cross platform input

		private Camera cam;
		private Vector3 m_StartPos, m_ResetPos;
		private Vector2 pointerStartPos;
		private bool m_UseX, m_UseY; // Toggle for using the x/y axis
		CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
		CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

		void Start()
		{
			cam = Camera.main.GetComponent<Camera>();
			MovementRange *= (int)transform.parent.parent.transform.localScale.x;
			m_StartPos = transform.position;
			m_ResetPos = transform.localPosition;
			CreateVirtualAxes();
		}

		void UpdateVirtualAxes(Vector2 value)
		{
			
			value.x -= pointerStartPos.x;
			value.y -= pointerStartPos.y;
			value /= (MovementRange);
			
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(value.x);
			}
			
			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(value.y);
			}
		}
		
		void CreateVirtualAxes()
		{
			// set axes to use
			m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
			m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);
			
			// create new axes based on axes to use
			if (m_UseX)
			{
				horizontalAxisName = transform.parent.parent.name + "-H";
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.UnRegisterVirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}
			if (m_UseY)
			{
				verticalAxisName = transform.parent.parent.name + "-V";
				m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
				CrossPlatformInputManager.UnRegisterVirtualAxis(verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
			}
		}
		
		
		public void OnDrag(PointerEventData data)
		{
			Vector3 newPos = data.position - pointerStartPos;
			
			if (m_UseX)
			{
				int delta = (int)(newPos.x);
				delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
				newPos.x = pointerStartPos.x + delta;
			}
			
			if (m_UseY)
			{
				int delta = (int)(newPos.y);
				delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
				newPos.y = pointerStartPos.y + delta;
			}
			
			transform.position = cam.ScreenToWorldPoint(newPos);
			UpdateVirtualAxes(newPos);
		}
		
		
		public void OnPointerUp(PointerEventData data)
		{
			
			transform.localPosition = m_ResetPos;
			UpdateVirtualAxes(pointerStartPos);
		}
		
		
		public void OnPointerDown(PointerEventData data) 
		{
			pointerStartPos = cam.WorldToScreenPoint(transform.position);
		}
		
		void OnDisable()
		{
			// remove the joysticks from the cross platform input
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Remove();
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis.Remove();
			}
		}
	}
}