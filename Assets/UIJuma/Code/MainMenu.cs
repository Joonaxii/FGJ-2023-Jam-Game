using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;

public enum Menus
{
    None = 0,
    Main = 1,
    Run = 2,
    Customize = 3,
    Credits = 4,
}
[System.Serializable]
public class MenuButton 
{
    public TextMeshProUGUI buttonText;
    public Menus destination;
    public bool interactable = true;
}

public class MainMenu : MonoBehaviour
{
    [Header("UI Components")]
    public MenuButton[] mainMenuOptions;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI descriptionBox;
    [Space(10), Header("Current variables")]
    private string _curStringDefault = "";
    public Menus curMenu = Menus.Main;
    public int curOption = 0;
    public int currentPage;

    public Image windowMax;
    public Sprite[] windowResizeSprites;

    public CanvasGroup menuCanvas;

    public float initialMoveTime = 0.5f;
    public float moveTime = 0.1f;
    private float _moveTimer = 0;

    private bool _canMove = true;
    private int _pressed = 0;
    public Color highlightColor;

    private string _mainText = "Macrohard Walls [Version 64.5177.34.77395.7675863.51]\n(c) Macrohard Freelancing. All rights are not reserved.\n\n{0}";
    private string[] _mainMenuText = {
        "C:\\Users\\INTO_THE_ROOT>",
        "C:\\Users\\INTO_THE_ROOT\\Run>",
        "C:\\Users\\INTO_THE_ROOT\\Customize>",
        "C:\\Users\\INTO_THE_ROOT\\Credits>",
    };

    [Space(10), Header("Hacker text")]
    public float minHackTextTime = 0.1f;
    public float maxHackTextTime = 0.5f;
    private float _hackTextTimer;
    [Range(1,42)] public int minHackerTexts = 1;
    [Range(1,42)] public int maxHackerTexts = 5;

    WeightedType<string>[] flavourTexts =
    {
        new ("Hacking into the mainframe...",10),
        new ("Opening the backdoor...",10),
        new ("Enhancing...",10),
        new ("Brute-forcing passwords...",10),
        new ("Cracking the firewall...",10),
        new ("Scanning event viewer...",8),
        new ("Infecting the back end...",8),
        new ("Hitting a breakpoint...",8),
        new ("Starting VPN (Connecting to GB)...",8),
        new ("Mining data...",8),
        new ("Spreading misinformation...",8),
        new ("Downloading more RAM...",6),
        new ("Enabling Blast Processing...",6),
        new ("Enabling tracking...",6),
        new ("Embedding link (<color=#ffff33>https://www.youtube.com/watch?v=dQw4w9WgXcQ</color>)...",6),
        new ("Spinning up the VM...",6),
        new ("Clearing cache...",6),
        new ("Leaving breadcrumbs...",6),
        new ("Reading comments...",6),
        new ("Framing the framework...",6),
        new ("Compiling shaders...",6),
        new ("Contacting tech support...",6),
        new ("Routing the router...",6),
        new ("Debugging code...",6),
        new ("Unpacking big data...",6),
        new ("Packing small data...",6),
        new ("Opening the database...",6),
        new ("Device disconnected...",4),
        new ("Updating Macrohard Walls...",4),
        new ("Throttling the network...",4),
        new ("Booting up Source-SDK...",4),
        new ("Playing metalpipefalling.mp3...",4),
        new ("Device connected...",4),
        new ("NullPointerException @404...",4),
        new ("Building a pyramid scheme...",4),
        new ("SyntaxError @177013...",4),
        new ("DDOSing ASAN...",4),
        new ("Running tree command...",4),
        new ("Accepting cookies...",4),
        new ("Collecting tokens...",4),
        new ("Preparing pump 'n' dump...",4),
        new ("Feeding the mouse...",4),
        new ("Rebooting...",4),
        new ("hehe haha, 80085...",2),
        new ("Searching for Imposters...",2),
        new ("Sorting passwords (w/ BogoSort)...",2),
        new ("Unchaining the Blockchain...",2),
        new ("Moderating r/hackers...",2),
        new ("Pulling the rug...",2),
        new ("Alexa, play Despacito...",1),
        new ("Screenshotting NFTs...",1),
        new ("Installing Bonzi Buddy...",1),
        new ("Attempting to hack: Edna (Attempt #220)...",1),
    };

    // Start is called before the first frame update
    void Start()
    {
        descriptionBox.enabled = false;
        _hackTextTimer = 0;
        _canMove = true;
        mainText.text = string.Format(_mainText, _mainMenuText[0]);
        curOption = 0;
        SetHighlight(mainMenuOptions[curOption].buttonText);

        windowMax.sprite = windowResizeSprites[GameManager.IsMaximized ? 1 : 0];
    }

    public void ToggleWindowSize()
    {
        GameManager.Instance.ToggleWindowSize();
        windowMax.sprite = windowResizeSprites[GameManager.IsMaximized ? 1 : 0];
    }

    public void OpenMainMenu()
    {
        menuCanvas.alpha = 1.0f;
        descriptionBox.enabled = false;
        _hackTextTimer = 0;
        _canMove = true;
        curOption = 0;
        SetHighlight(mainMenuOptions[curOption].buttonText);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_canMove) return;
        var Inputs = GameManager.Instance.Inputs;

        if (_pressed != 0)
        {
            _moveTimer -= Time.deltaTime;
            if (_moveTimer <= 0)
            {
                ChangeOption(_pressed);
                _moveTimer = moveTime;
            }
        }

        if (Inputs.IsDown(InputHandler.InputType.Confirm))
        {
            SelectOption();
        }

        if (Inputs.IsDown(InputHandler.InputType.MoveUp))
        {
            _pressed = -1;
            _moveTimer = initialMoveTime;
            ChangeOption(_pressed);
        }
        if (Inputs.IsUp(InputHandler.InputType.MoveUp))
        {
            ResetMovement();
        }

        if (Inputs.IsDown(InputHandler.InputType.MoveDown))
        {
            _pressed = 1;
            _moveTimer = initialMoveTime;
            ChangeOption(_pressed);
        }
        if (Inputs.IsUp(InputHandler.InputType.MoveDown))
        {
            ResetMovement();
        }
    }

    void ChangeOption(int dir)
    {
        ResetHighlight(mainMenuOptions[curOption].buttonText);
        if (curOption == 0 && dir == -1)
        {
            curOption = mainMenuOptions.Length - 1;
        }
        else if (curOption == mainMenuOptions.Length-1 && dir == 1)
        {
            curOption = 0;
        } 
        else
        {
            curOption += dir;
        }
        SetHighlight(mainMenuOptions[curOption].buttonText);
    }
    void ResetMovement()
    {
        _pressed = 0;
        _moveTimer = 0;
    }

    #region Highlight functions
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
    #endregion

    void SelectOption() => ChangeMenu(mainMenuOptions[curOption].destination);

    void ChangeMenu(Menus nextMenu)
    {
        curMenu = nextMenu;
        mainText.text = string.Format(_mainText, _mainMenuText[(int)nextMenu-1]);

        switch (nextMenu)
        {
            case Menus.Main:
                // TODO: Open main menu and enable main menu options
                OpenMenu(Menus.Main);
                break;
            case Menus.Run:
                _canMove = false;
                OpenMenu(Menus.Run);
                StartCoroutine(HackerTexts());
                break;
            case Menus.Customize:
                break;
            case Menus.Credits:
                break;
        }
    }

    void OpenMenu(Menus newMenu)
    {
        CloseMenu(curMenu);
        // TODO: Set new stuff to appear 

    }
    void CloseMenu(Menus menu)
    {
        // TODO: Close the current menu
        foreach (var textButton in mainMenuOptions)
        {
            textButton.interactable = false;
            textButton.buttonText.enabled = false;
        }
    }

    IEnumerator HackerTexts()
    {
        RNG.SetSeed((int)DateTime.Now.ToBinary());
        List<WeightedType<string>> localTexts = new(flavourTexts);
        int numOfTexts = RNG.Range(minHackerTexts, maxHackerTexts+1);
        descriptionBox.text = "";
        descriptionBox.enabled = true;

        for (int i = 0; i < numOfTexts; i++)
        {
            int ind = RNG.SelectRandomIndex(localTexts);
            string text = localTexts[ind].value;
            localTexts.RemoveAt(ind);

            descriptionBox.text += $"{text}\n";

            // TODO: ADD TIMER
            _hackTextTimer = RNG.Range(minHackTextTime, maxHackTextTime);
            yield return new WaitForSeconds(_hackTextTimer);
        }
        descriptionBox.text += "<color=#FFFF33><b>You are in.</color></b>";

        GameManager.Instance.InitGame();
        float fadeDuration = 1.5f;
        float fadeTime = 0;

        yield return new WaitForSeconds(1.0f);
        while (fadeTime < fadeDuration)
        {
            float n = fadeTime / fadeDuration;
            menuCanvas.alpha = 1.0f - n;

            fadeTime += GTime.GetDeltaTime(0);
            yield return null;
        }
        menuCanvas.alpha = 0;
        yield return StartCoroutine(GameManager.Instance.StartGame());
    }
}
