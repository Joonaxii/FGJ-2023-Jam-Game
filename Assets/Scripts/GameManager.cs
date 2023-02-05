using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool IsMaximized => !Instance._maximize;
    public bool YoureBoned => _youreBoned;

    public static GameManager Instance { get; private set; }

    public InGameUI GameUI => _inGameUI;
    public CameraController CameraController => _cameraCtrl;
    public Player Player => _player;
    public GameBoard Board => _board;
    public InputHandler Inputs => _inputs;
    public HackingManager Hacking => _hackingManager;

    public AudioManager AudioManager => _audioManager;
    public GameStats GetStats => _stats;

    public bool IsScanInProgress => _scanInProgress;

    public Animator virusAnim;
    public ParticleSystem psDeath;

    [SerializeField] private GameStats _stats;
    [SerializeField] private Player _player;
    [SerializeField] private GameBoard _board;

    [SerializeField] private CameraController _cameraCtrl;
    [SerializeField] private AnimationCurve _cameraMoveCurve;

    [SerializeField] private InGameUI _inGameUI;
    [SerializeField] private HackingManager _hackingManager;
    [SerializeField] private MainMenu _mainMenu;

    private InputHandler _inputs;

    private LineRenderer _path;
    [SerializeField] private AudioManager _audioManager;

    private bool _gameStarted;
    private bool _scanInProgress;
    private bool _youreBoned;

    private bool _maximize = true;

    private void Awake()
    {
        _inputs = new InputHandler();
        _path = transform.Find("Path").GetComponent<LineRenderer>();
        Instance = this;

        _youreBoned = false;
        GTime.Init();
        _audioManager.Initialize();
    }

    public void CloseHacking() => _hackingManager.CancelHack();

    IEnumerator Start()
    {
        _mainMenu.OpenMainMenu();
        _hackingManager.Reset();
#if !UNITY_EDITOR
        Screen.SetResolution(1600, 900, false);
        yield return null;
        BorderlessWindow.InitializeOnLoad();
        BorderlessWindow.RestoreWindow();
#endif
        yield return null;
    }

    public void MinimizeWindow()
    {
#if !UNITY_EDITOR
        BorderlessWindow.MinimizeWindow();
#endif
    }
    public void MaximizeWindow()
    {
#if !UNITY_EDITOR
        BorderlessWindow.MaximizeWindow();
#endif
    }
    public void RestoreWindow()
    {
#if !UNITY_EDITOR
        BorderlessWindow.RestoreWindow();
#endif
    }

    public void ToggleWindowSize()
    {
        _maximize = !_maximize;
#if !UNITY_EDITOR
        if(_maximize)
        {
            BorderlessWindow.RestoreWindow();
        }
        else
        {
            BorderlessWindow.MaximizeWindow();
        }
#endif
    }

    public void InitGame()
    {
        _path.positionCount = 0;

        virusAnim.SetBool("IsDead", false);

        _youreBoned = false;
        _hackingManager.Reset();
        GTime.Reset();
        _stats.Reset();

        _board.Generate(31, 31);
        _player.ResetPlayer(_board);

        _cameraCtrl.SetPosition(Vector3.zero, true);
        _cameraCtrl.SetNearFarBlend(1.0f, true);

        _inGameUI.UpdateDetections(_stats.Detections, _stats.MaxDetections);
        _inGameUI.UpdateBitAmount(_stats.Bits, _stats.BitCapacity, _stats.BitsPerSecond, _player.AccumulatedCost);
        _inGameUI.UpdateScanTime(_stats.ScanInterval - _stats.ScanTime, _stats.Corruption);
        _inGameUI.UpdateFullScanTime(_stats.FullScanDuration - _stats.FullScanTime);

        UpdatePath();
    }

    public IEnumerator StartGame()
    {
        float fadeDur = 1.0f;
        _cameraCtrl.SetNearFarBlend(1.0f, true);

        yield return new WaitForSeconds(1.0f);

        float t = 0;
        while (t < fadeDur)
        {
            _cameraCtrl.SetNearFarBlend(1.0f - _cameraMoveCurve.Evaluate(t / fadeDur), true);
            t += GTime.GetDeltaTime(0);
            yield return null;
        }

        _cameraCtrl.SetNearFarBlend(0, true);
        _gameStarted = true;
    }

    public void UpdatePath()
    {
        _path.positionCount = 0;

        var path = _player.GetPath;
        if (path.Count > 0)
        {

            Vector3[] positions = new Vector3[path.Count + 1];
            positions[path.Count] = _board.GridToWorld(_player.Coord);
            positions[path.Count].y = 0.1f;

            for (int i = 0; i < path.Count; i++)
            {
                positions[i] = _board.GridToWorld(path[i]);
                positions[i].y = 0.1f;
            }
            _path.positionCount = path.Count + 1;
            _path.SetPositions(positions);
        }
    }

    public void QuitGame() => Application.Quit();

    public void Update()
    {
        GTime.Tick(Time.deltaTime);

        _audioManager.Update();
        _inputs.Update();

        if (_youreBoned)
        {
            float delta = GTime.GetDeltaTime(0);
            _cameraCtrl.Update(delta);

            return;
        }

        if (_gameStarted)
        {
            float delta = GTime.GetDeltaTime(0);
            _cameraCtrl.Update(delta);

            _stats.Update(delta);
            if (!_scanInProgress)
            {
                switch (_stats.UpdateScan(delta))
                {
                    case 1:
                        StartScan();
                        break;
                    case 2:
                        StartFinalScan();
                        return;
                }
            }

            if (!_hackingManager.IsHacking)
            {
                _player.Update(delta);
            }
        }
    }

    public void StartWin()
    {
        _youreBoned = true;
        if (_hackingManager.IsHacking)
        {
            _hackingManager.CancelHack();
        }

        StopAllCoroutines();
        StartCoroutine(_scan = FinalWin());
    }

    private IEnumerator FinalWin()
    {
        _player.ResetPlayer(_board);
        _board.HideAllTexts();

        _cameraCtrl.SetPosition(new Vector3(0, 1.0f, -3));
        _cameraCtrl.SetNearFarBlend(0.25f, false);

        yield return new WaitForSeconds(1.25f);

        float finalScanDur = 0.4f;

        float[] offsets = new float[_board.Width * _board.Height];
        float maxDist = (_board.Width * GameBoard.PADDING) * 5;
        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3 pos = _board.GridToWorld(new Vector2Int(i % _board.Width, i / _board.Width));
            float idst = pos.magnitude;

            float n = 1.0f - Mathf.Clamp01(idst / maxDist);
            offsets[i] = Mathf.Lerp(0.0f, -2.5f, n * n * n * n);
        }

        GameManager.Instance.AudioManager.PlaySFX("PlayerWin2");
        while (true)
        {
            bool done = true;
            for (int i = 0; i < offsets.Length; i++)
            {
                var f = offsets[i] + GTime.GetDeltaTime(0);
                offsets[i] = f;
                float n = Mathf.Clamp01(f / finalScanDur);

                n = n * n * n;
                _board.LerpBetweenBaseAndHacked(n, i);
                if (f < finalScanDur)
                {
                    done = false;
                }
                else if(n >= 0.5f)
                {
                    var p = new Vector2Int(i % _board.Width, i / _board.Width);
                    if (!_board[p].IsHacked())
                    {
                        _board.SetTileState(p, true);
                        _board[p].unit?.RefreshHackState();
                    }
                }
            }

            if (done) { break; }
            yield return null;
        }


        float fadeTimeXD = 0.5f;
        float t = 0;

        _cameraCtrl.SetNearFarBlend(1.0f, false);
        _cameraCtrl.SetPosition(new Vector3(0, -41, 3));

        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.AudioManager.PlaySFX("SkeletonLaughSpoopy");
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(_mainMenu.FadeCameraBack());
        Restart();
    }



    public void StartFinalScan()
    {
        GameManager.Instance.AudioManager.PlaySFX("VirusFound");

        _youreBoned = true;
        _inGameUI.UpdateFullScanTime(0);
        if (_hackingManager.IsHacking)
        {
            _hackingManager.CancelHack();
        }

        StopAllCoroutines();
        StartCoroutine(_scan = FinalScan());
    }

    private IEnumerator FinalScan()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetFloat("_Palette_Blend", 1.0f);

        Renderer[] rends = virusAnim.GetComponentsInChildren<Renderer>();
        foreach (var item in rends)
        {
            item.SetPropertyBlock(props);
        }

        _player.ResetPlayer(_board);
        _board.HideAllTexts();

        yield return null;
        _cameraCtrl.SetPosition(new Vector3(0, 1.0f, -3));
        _cameraCtrl.SetNearFarBlend(0.25f, false);

        yield return new WaitForSeconds(1.25f);

        float finalScanDur = 0.4f;

        float[] offsets = new float[_board.Width * _board.Height];

        float maxDist = (_board.Width * GameBoard.PADDING) * 5;
        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3 pos = _board.GridToWorld(new Vector2Int(i % _board.Width, i / _board.Width));
            float idst = pos.magnitude;

            float n = 1.0f - Mathf.Clamp01(idst / maxDist);
            offsets[i] = Mathf.Lerp(0.0f, -2.5f, n * n * n * n);
        }

        while (true)
        {
            bool done = true;
            for (int i = 0; i < offsets.Length; i++)
            {
                var f = offsets[i] + GTime.GetDeltaTime(0);
                offsets[i] = f;
                float n = Mathf.Clamp01(f / finalScanDur);

                n = n * n * n;
                _board.LerpBetweenBaseAndScan(n, i);
                if (f < finalScanDur)
                {
                    done = false;
                }
                else if(n >= 0.5f)
                {
                    var p = new Vector2Int(i % _board.Width, i / _board.Width);
                    if (_board[p].IsHacked())
                    {
                        _board.SetTileState(p, false);
                        _board[p].unit?.RefreshHackState();
                    }
                }


            }

            if (done) { break; }
            yield return null;
        }


        float fadeTimeXD = 0.5f;
        float t = 0;

        while (t < fadeTimeXD)
        {
            props.SetFloat("_Palette_Blend", 1.0f - (t / fadeTimeXD));
            foreach (var item in rends)
            {
                item.SetPropertyBlock(props);
            }
            t += GTime.GetDeltaTime(0);
            yield return null;
        }
        props.SetFloat("_Palette_Blend", 0.0f);
        foreach (var item in rends)
        {
            item.SetPropertyBlock(props);
        }
        _cameraCtrl.SetNearFarBlend(1.0f, false);
        _cameraCtrl.SetPosition(new Vector3(0, -41, 3));

        yield return new WaitForSeconds(1.5f);
        virusAnim.SetBool("IsDead", true);
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.AudioManager.PlaySFX("PlayerDeath");
        MonoBehaviour.Instantiate(psDeath, virusAnim.transform.position + new Vector3(0, 1.0f, 0), Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(_mainMenu.FadeCameraBack());
        Restart();
    }

    public void StartScan()
    {
        if (_youreBoned) { return; }
        _scanInProgress = true;
        _inGameUI.UpdateScanTime(0, _stats.Corruption);

        if (_scan != null)
        {
            StopCoroutine(_scan);
        }

        _scan = ScanSequence();
        StartCoroutine(_scan);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator _scan;
    private IEnumerator ScanSequence()
    {

        GameManager.Instance.AudioManager.PlaySFX("ScanStart");

        float fadeDur = 1.5f;
        float t = 0;
        while (t < fadeDur)
        {
            _cameraCtrl.SetNearFarBlend(_cameraMoveCurve.Evaluate(t / fadeDur), false);
            t += GTime.GetDeltaTime(0);
            yield return null;
        }

        _cameraCtrl.SetNearFarBlend(1.0f, false);

        int width = _board.Width;
        for (int i = 0; i < width; i++)
        {
            if (_youreBoned) { yield break; }
            if (i == width - 1)
            {
                yield return _board.BeginScan(i);
            }
            else
            {
                StartCoroutine(_board.BeginScan(i));
            }
            yield return new WaitForSeconds(Mathf.Lerp(0.65f, 0.05f, _stats.Corruption));
        }
        _scanInProgress = false;

        fadeDur = 1.5f;
        t = 0;
        while (t < fadeDur)
        {
            _cameraCtrl.SetNearFarBlend(1.0f - _cameraMoveCurve.Evaluate(t / fadeDur), false);
            t += GTime.GetDeltaTime(0);
            yield return null;
        }

        _cameraCtrl.SetNearFarBlend(0.0f, false);
        _inGameUI.UpdateScanTime(_stats.ScanInterval - _stats.ScanTime, _stats.Corruption);
    }
}
