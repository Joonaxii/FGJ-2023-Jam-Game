
using Unity.VisualScripting;

public static class GTime
{
    public const int MAX_TIME_SPACES = 32;

    public struct TimeSpace
    {
        public float time;
        public float deltaTime;
        public float timeScale;

        public void Reset()
        {
            time = 0;
            deltaTime = 0;
            timeScale = 1;
        }

        public void Tick(float delta)
        {
            deltaTime = delta * timeScale;
            time += deltaTime;
        }
    }

    private static float _globalTimescale;
    private static TimeSpace[] _timeSpaces;

    public static void Init()
    {
        _timeSpaces = new TimeSpace[MAX_TIME_SPACES];

        _globalTimescale = 1.0f;
        for (int i = 0; i < MAX_TIME_SPACES; i++)
        {
            _timeSpaces[i].Reset();
        }
    }

    public static float SetGlobalTimeScale(float scale) => _globalTimescale = scale;
    public static float GetGlobalTimeScale() => _globalTimescale;

    public static void Reset()
    {
        for (int i = 0; i < MAX_TIME_SPACES; i++)
        {
            _timeSpaces[i].Reset();
        }
    }

    public static float GetTime(int timeSpace)
    {
        return timeSpace >= MAX_TIME_SPACES ||timeSpace < 0 ? 0 : _timeSpaces[timeSpace].time;
    }

    public static float GetDeltaTime(int timeSpace)
    {
        return timeSpace >= MAX_TIME_SPACES || timeSpace < 0 ? 0 : _timeSpaces[timeSpace].deltaTime;
    }

    public static void Tick(float delta) 
    {
        delta *= _globalTimescale;
        for (int i = 0; i < MAX_TIME_SPACES; i++)
        {
            _timeSpaces[i].Tick(delta);
        }
    }
}
