using System;
using JetBrains.Annotations;
using Screens.MainMenu;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStorage : MainMenuElement.IAuthenticatedState
{
    public event Action<bool> IsAuthenticatedChanged;

    [CanBeNull]
    public string Id
    {
        get => PlayerPrefs.GetString("Id").NullIfEmpty();
        set
        {
            if (value != null)
            {
                IsAuthenticatedChanged?.Invoke(true);
                PlayerPrefs.SetString("Id", value);
            }
            else
            {
                IsAuthenticatedChanged?.Invoke(false);
                PlayerPrefs.DeleteKey("Id");
            }
        }
    }

    public bool IsAuthenticated => Id != null;

    public void Exit() => Id = null;
}
