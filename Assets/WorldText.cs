using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldText : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void Setup(Vector3 position, bool visible, string text)
    {
        if (visible)
        {
            this.text.text = text;
            transform.position = position;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
