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

    private string[] _lorem = new string[] { 
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris dignissim tellus cursus erat vestibulum sagittis. Donec leo neque, lacinia sit amet dignissim sed, tincidunt vel dolor. Etiam in libero ut est euismod finibus quis in purus. Donec pharetra justo ut sapien pretium, ac varius sapien hendrerit. Sed varius quam non mi consectetur porta. Donec convallis ipsum dolor, at mattis lacus vehicula ut. Morbi quis nibh nisl. Cras et magna vitae nunc euismod aliquet. Mauris pharetra ipsum quam, a facilisis ipsum varius ut. Morbi aliquam aliquet est, quis blandit diam efficitur et. Integer at dui est. Sed molestie auctor nisl, sed elementum dolor accumsan et. Vivamus sodales lobortis est sed ultrices. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nam justo arcu, sagittis quis consectetur ut, condimentum tincidunt lacus. Etiam lorem tortor, ultrices pharetra quam in, suscipit lobortis sem. Nam vehicula diam id sem laoreet, sed molestie diam laoreet. Suspendisse ac dictum sapien. Donec varius erat ut tincidunt accumsan. Aliquam et justo orci. Pellentesque venenatis congue accumsan. Duis vel nulla urna. Cras at ultrices nulla, vel accumsan tellus. Duis diam dui, aliquam eget auctor sed, tempus in nisl. Donec tempor porta eros, non mollis justo dignissim vel. Nulla sagittis leo eu faucibus hendrerit. Nam iaculis nisi sed odio vehicula, eu gravida erat sagittis. Vivamus pharetra pellentesque ipsum vitae accumsan. Integer tristique eros tellus, nec imperdiet nisi vulputate vel. Curabitur quis libero pharetra, interdum purus vitae, tincidunt eros. Aliquam vitae condimentum velit. Maecenas tellus leo, semper at.",
    "I’m here all weekend - come on ya fuckin punk ass swimme. I’ll be waiting on ya - I’ll be waiting on your punk ass - wait matter of fact give me your address I’ll come to wherever you are and give you a chance to make good on your promises since I know you won’t actually come here me Navy SEAL lol what BUDS class were you in bitch? See you’re talking to an Army Ranger - RSC 13-2 - I’ve ACTUALLY been on clandestine missions - I’ve ACTUALLY been in gunfights - and on the 1% chance that you’re ACTUALLY a buds graduate I’ll tell you RQRF in the korangal - we were saving baby seals on a daily basis because they have no fucking idea what to do when bullets start flying the other direction - so no - I’m not worried about you - the USMC is still using gulf war hand me downs so you’re saying your equipment is dated and sporting extensive wear and tear? Annnndddd no need to involve your top secret lies I mean spies whoops - cuzzzzz I just told you and the internet where I live - you can come here or give me your address and I’ll come there - either way",
    "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.",
    "Today when I walked into my economics class I saw something I dread every time I close my eyes. Someone had brought their new gaming laptop to class. The Forklift he used to bring it was still running idle at the back. I started sweating as I sat down and gazed over at the 700lb beast that was his laptop. He had already reinforced his desk with steel support beams and was in the process of finding an outlet for a power cable thicker than Amy Schumer's thigh. I start shaking. I keep telling myself I'm going to be alright and that there's nothing to worry about. He somehow finds a fucking outlet. Tears are running down my cheeks as I send my last texts to my family saying I love them. The teacher starts the lecture, and the student turns his laptop on. The colored lights on his RGB Backlit keyboard flare to life like a nuclear flash, and a deep humming fills my ears and shakes my very soul. The entire city power grid goes dark. The classroom begins to shake as the massive fans begin to spin. In mere seconds my world has gone from vibrant life, to a dark, earth shattering void where my body is getting torn apart by the 150mph gale force winds and the 500 decibel groan of the cooling fans. As my body finally surrenders, I weep, as my school and my city go under. I fucking hate gaming laptops.",
    "Wowwwww, you meow like a cat! That means you are one, right? Shut the fuck up. If you really want to be put on a leash and treated like a domestic animal then that’s called a fetish, not “quirky” or “cute”. What part of you seriously thinks that any part of acting like a feline establishes a reputation of appreciation? Is it your lack of any defining aspect of personality that urges you to resort to shitty representations of cats to create an illusion of meaning in your worthless life? Wearing “cat ears” in the shape of headbands further notes the complete absence of human attribution to your false sense of personality, such as intelligence or charisma in any form or shape. Where do you think this mindset’s gonna lead you? You think you’re funny, random, quirky even? What makes you think that acting like a fucking cat will make a goddamn hyena laugh? I, personally, feel extremely sympathetic towards you as your only escape from the worthless thing you call your existence is to pretend to be an animal. But it’s not a worthy choice to assert this horrifying fact as a dominant trait, mainly because personality traits require an initial personality to lay their foundation on. You’re not worthy of anybody’s time, so go fuck off, “cat-girl”. ",
    "I am a concerned mother with a 13 year old child and I am here to seek help regarding my son. Last week when we went to the supermarket, my son pointed to a red trash can and started jumping around screaming “THAT’S AMONG US! THAT TRASH CAN IS SUS! RED IS THE IMPOSTOR!” As soon as he did that, the manager told us to leave. I told him that my son is just excited about something, and apologised. But the manager still told us to leave so I picked up the red trash can that my son was going crazy over and threw it on the managers head. Then my son shouted “DEAD BODY REPORTED.” Can someone please tell me what on earth is wrong with him?",
    };
    
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

        _words = _lorem[Random.Range(0, _lorem.Length)].Split();
        _barRect = timerBar.rectTransform;

        _timer = 0;
    }
}
