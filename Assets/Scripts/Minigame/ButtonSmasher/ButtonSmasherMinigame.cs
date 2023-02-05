using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSmasherMinigame : HackingBase
{
    [Header("UI Components")]
    public TextMeshProUGUI textBox;
    public Image timerBar;

    private float _timerBarWidth = 650;

    [Space(5),Header("Variables")]
    public float minTime;
    public float maxTime;
    public int minButtonPresses;
    public int maxButtonPresses;

    private float _mashingTime;
    private float _timer;
    private int _numOfMashes;
    private int _curPresses;

    private string _lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris dignissim tellus cursus erat vestibulum sagittis. Donec leo neque, lacinia sit amet dignissim sed, tincidunt vel dolor. Etiam in libero ut est euismod finibus quis in purus. Donec pharetra justo ut sapien pretium, ac varius sapien hendrerit. Sed varius quam non mi consectetur porta. Donec convallis ipsum dolor, at mattis lacus vehicula ut. Morbi quis nibh nisl. Cras et magna vitae nunc euismod aliquet. Mauris pharetra ipsum quam, a facilisis ipsum varius ut. Morbi aliquam aliquet est, quis blandit diam efficitur et. Integer at dui est. Sed molestie auctor nisl, sed elementum dolor accumsan et. Vivamus sodales lobortis est sed ultrices. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nam justo arcu, sagittis quis consectetur ut, condimentum tincidunt lacus. Etiam lorem tortor, ultrices pharetra quam in, suscipit lobortis sem. Nam vehicula diam id sem laoreet, sed molestie diam laoreet. Suspendisse ac dictum sapien. Donec varius erat ut tincidunt accumsan. Aliquam et justo orci. Pellentesque venenatis congue accumsan. Duis vel nulla urna. Cras at ultrices nulla, vel accumsan tellus. Duis diam dui, aliquam eget auctor sed, tempus in nisl. Donec tempor porta eros, non mollis justo dignissim vel. Nulla sagittis leo eu faucibus hendrerit. Nam iaculis nisi sed odio vehicula, eu gravida erat sagittis. Vivamus pharetra pellentesque ipsum vitae accumsan. Integer tristique eros tellus, nec imperdiet nisi vulputate vel. Curabitur quis libero pharetra, interdum purus vitae, tincidunt eros. Aliquam vitae condimentum velit. Maecenas tellus leo, semper at.";
    private string[] _words;

    private RectTransform _barRect;

    private void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer < _mashingTime)
        {
            float newWidth = Mathf.Lerp(_timerBarWidth, 0, (_timer / _mashingTime));
            _timer += Time.deltaTime;
            _barRect.sizeDelta = new Vector2(newWidth, _barRect.rect.height);

            _mashingTime -= Time.deltaTime;
            if (Input.anyKeyDown)
            {
                textBox.text += $"{_words[_curPresses]} ";
                _curPresses++;
            }

            if (_curPresses >= _numOfMashes)
            {
                Finish();
            }

        } 
        else
        {

        }
    }

    private void CleanUp()
    {
        _mashingTime = 0;
        _numOfMashes = 0;
        _curPresses = 0;
        _timer = 0;

        var barRect = timerBar.rectTransform.rect;
        barRect.Set(barRect.x, barRect.y, _timerBarWidth, barRect.height);
        textBox.text = "";
    }

    public override void Finish()
    {
        base.Finish();
        CleanUp();
    }
    public override void Cancel() => CleanUp();

    public override void Initialize()
    {
        _mashingTime = Random.Range(minTime, maxTime);
        _numOfMashes = Random.Range(minButtonPresses, maxButtonPresses);

        _words = _lorem.Split();
        _barRect = timerBar.rectTransform;

        _timer = 0;
    }
}
