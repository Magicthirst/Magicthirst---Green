using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStorage
{
    [CanBeNull]
    public string Id
    {
        get => PlayerPrefs.GetString("Id").NullIfEmpty();
        set
        {
            if (value != null)
            {
                PlayerPrefs.SetString("Id", value);
            }
            else
            {
                PlayerPrefs.DeleteKey("Id");
            }
        }
    }

    public void Exit() => Id = null;
}
