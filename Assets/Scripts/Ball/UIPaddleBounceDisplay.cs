using TMPro;
using UnityEngine;
using YourGame.Utilities;  // ICustomUpdate + IDisposable si aplica

public class UIPaddleBounceDisplay : ICustomUpdate, IDisposable
{
    private readonly TextMeshProUGUI _bounceText;
    private readonly BallController _ballController;

    public UIPaddleBounceDisplay(TextMeshProUGUI bounceText, BallController ballController)
    {
        _bounceText = bounceText;
        _ballController = ballController;

        GameManager.Instance.Register(this);
    }

    public void Dispose()
    {
        GameManager.Instance.Unregister(this);
    }

    public void CustomUpdate(float deltaTime)
    {
        if (_bounceText == null || _ballController == null)
            return;

        _bounceText.text = $"Paddle Bounces: {_ballController.TotalPaddleBounces}";
    }
}