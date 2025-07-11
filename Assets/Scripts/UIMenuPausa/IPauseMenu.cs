// IPauseMenu.cs
using UnityEngine;
using YourGame.Utilities;  // para ICustomUpdate

public interface IPauseMenu : ICustomUpdate
{
    void TogglePause();
    bool IsPaused { get; }
}
