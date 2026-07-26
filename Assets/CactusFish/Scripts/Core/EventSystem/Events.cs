public class OpenUI : IEvent
{
    public string UIName { get; set; }
}

public class CloseUI : IEvent
{
    public string UIName { get; set; }
}

public class GamePause : IEvent { }

public class GameResume : IEvent { }

public class GameStart : IEvent { }

public class GameOver : IEvent { }

public class GameRestart : IEvent { }
public class SceneLoadProgressEvent : IEvent
{
    public float Progress;
}
public class SceneLoadCompleteEvent : IEvent
{
    public string SceneName;
}
