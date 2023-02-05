using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler
{
    private Vector2 _deltaValue = Vector2.zero;
    [SerializeField] private RectTransform _local;

    public void OnDrag(PointerEventData data)
    {
        if (_local != null)
        {
            _local.position += (Vector3)data.delta;
            return;
        }

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
