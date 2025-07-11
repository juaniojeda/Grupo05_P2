using TMPro;
using UnityEngine;
using YourGame.Utilities; // para IDisposable

public class UITimerDisplay : ICustomUpdate, IDisposable
{
    private TextMeshProUGUI _timerText;
    private ITimeProvider _timeProvider;

    public UITimerDisplay(TextMeshProUGUI timerText, ITimeProvider timeProvider)
    {
        _timerText = timerText;
        _timeProvider = timeProvider;

        // Ahora usamos GameManager para el registro
        GameManager.Instance.Register(this);
    }

    public void CustomUpdate(float deltaTime)
    {
        if (_timeProvider == null || _timerText == null)
            return;

        float timeRemaining = _timeProvider.GetTimeRemaining();
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        _timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void Dispose()
    {
        GameManager.Instance.Unregister(this);
    }
}

