
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

    private static TimeSpace[] _timeSpaces;

    public static void Init()
    {
        _timeSpaces = new TimeSpace[MAX_TIME_SPACES];
        for (int i = 0; i < MAX_TIME_SPACES; i++)
        {
            _timeSpaces[i].Reset();
        }
    }

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
        for (int i = 0; i < MAX_TIME_SPACES; i++)
        {
            _timeSpaces[i].Tick(delta);
        }
    }
}
