using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using JetBrains.Annotations;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI[] options;
    public TextMeshProUGUI mainText;

    private string _curStringDefault = "";
    public int curOption = 0;
    public int currentPage;

    public float initialMoveTime = 0.5f;
    public float moveTime = 0.1f;
    private float _moveTimer = 0;

    private int _pressed = 0;
    public Color highlightColor;

    private string _mainText = "Macrohard Walls [Version 64.5177.34.77395.7675863.51]\n(c) Macrohard Freelancing. All rights are not reserved.\n\n{0}";
    private string[] _mainMenuText = {
        "C:\\Users\\INTO_THE_ROOT>",
        "C:\\Users\\INTO_THE_ROOT\\Run>",
        "C:\\Users\\INTO_THE_ROOT\\Customize>",
        "C:\\Users\\INTO_THE_ROOT\\Credits>",
    };

    // Start is called before the first frame update
    void Start()
    {
        mainText.text = string.Format(_mainText, _mainMenuText[1]);
        curOption = 0;
        SetHighlight(options[curOption]);
    }

    // Update is called once per frame
    void Update()
    {

        if (_pressed != 0)
        {
            _moveTimer -= Time.deltaTime;
            if (_moveTimer <= 0)
            {
                ChangeOption(_pressed);
                _moveTimer = moveTime;
            }
        }


        if (Input.GetKeyDown(KeyCode.Return))
        {
            SelectOption();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _pressed = -1;
            _moveTimer = initialMoveTime;
            ChangeOption(_pressed);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            ResetMovement();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _pressed = 1;
            _moveTimer = initialMoveTime;
            ChangeOption(_pressed);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            ResetMovement();
        }
    }

    void ChangeOption(int dir)
    {
        ResetHighlight(options[curOption]);
        if (curOption == 0 && dir == -1)
        {
            curOption = options.Length - 1;
        }
        else if (curOption == options.Length-1 && dir == 1)
        {
            curOption = 0;
        } 
        else
        {
            curOption += dir;
        }
        SetHighlight(options[curOption]);
    }

    void SetHighlight(TextMeshProUGUI text)
    {
        _curStringDefault = text.text;
        string modifiedString = $" - {_curStringDefault.ToUpper()} -";

        text.SetText(modifiedString);
        text.color = highlightColor;
    }
    void ResetHighlight(TextMeshProUGUI text)
    {
        text.SetText(_curStringDefault);
        text.color = Color.white;
    }

    void SelectOption()
    {
        Debug.Log($"Selection option number: {curOption}");
        //TODO: Check which option the player has chosen and do stuff relating to that
        // So Far: 0 = start game | 1 = customization | 2 = credits
    }

    void ResetMovement()
    {
        _pressed = 0;
        _moveTimer = 0;
    }
}
