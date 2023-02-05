using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class HackingManager
{
    public bool IsHacking { get; private set; }

    public GameObject hackingWindow;
    public Animation anim;

    public WeightedType<HackingBase>[] hackingMinigames;

    private HackingBase _hacking;
    private IEnumerator _hack;

    private Action<bool> _cb;

    public void Reset()
    {
        IsHacking = false;
        hackingWindow.SetActive(false);
        _hacking?.Cancel();
        _cb = null;
        _hacking = null;
    }

    public void BeginHacking(Action<bool> callback)
    {
        _cb = null;
        if (hackingMinigames.Length < 1)
        {
            callback?.Invoke(true);
            return;
        }

        _cb = callback;
        _hacking = RNG.SelectRandom(hackingMinigames);
        if(_hack != null)
        {
            GameManager.Instance.StopCoroutine(_hack);
        }
        GameManager.Instance.StartCoroutine(_hack = HackBegin()); 
    }

    private void DisableAll()
    {
        for (int i = 0; i < hackingMinigames.Length; i++)
        {
            hackingMinigames[i].value.gameObject.SetActive(false);
        }
    }

    public void CompleteHacking()
    {
        _cb?.Invoke(true);
        _cb = null;
        if (_hack != null)
        {
            GameManager.Instance.StopCoroutine(_hack);
        }
        GameManager.Instance.StartCoroutine(_hack = HackEnd(true));
    }

    public void CancelHack()
    {
        _cb?.Invoke(false);
        _cb = null;
        if (_hack != null)
        {
            GameManager.Instance.StopCoroutine(_hack);
        }
        GameManager.Instance.StartCoroutine(_hack = HackEnd(false));
    }

    public void ClearHacking(bool finish)
    {
        if (_hacking)
        {
            if(!finish)
            {
                _hacking.Cancel();
            }

            _hacking.gameObject.SetActive(false);
            _hacking = null;
        }
    }

    public IEnumerator HackBegin()
    {
        DisableAll();

        hackingWindow.transform.localPosition = new Vector3(250, -25, 0);

        IsHacking = true;
        hackingWindow.SetActive(true);
        anim.Play("Open", PlayMode.StopAll);

        _hacking.gameObject.SetActive(true);
        while (anim.isPlaying)
        {
            yield return null;
        }

        Debug.Log($"Starting minigame {_hacking}");
        _hacking.Initialize();
    }

    public IEnumerator HackEnd(bool finish)
    {
        DisableAll();

        hackingWindow.SetActive(true);
        anim.Play("Close", PlayMode.StopAll);

        while (anim.isPlaying)
        {
            yield return null;
        }

        IsHacking = false;
        hackingWindow.SetActive(false);
        ClearHacking(finish);
        DisableAll();
    }
}