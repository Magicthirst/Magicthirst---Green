using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStorage
{
    [CanBeNull]
    public string Id
    {
        get => PlayerPrefs.GetString("Id").NullIfEmpty();
        set => PlayerPrefs.SetString("Id", value);
    }

    public void Exit() => Id = null;
}
