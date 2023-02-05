using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler
{
    private Vector2 _deltaValue = Vector2.zero;
    public void OnDrag(PointerEventData data)
    {
        if (BorderlessWindow.framed) { return; }

        _deltaValue += data.delta;
        if (data.dragging)
        {
#if !UNITY_EDITOR
            BorderlessWindow.MoveWindowPos(_deltaValue, Screen.width, Screen.height);
#endif
        }
    }
}
