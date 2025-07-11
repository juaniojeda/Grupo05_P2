using TMPro;
using UnityEngine;
using YourGame.Utilities;

public class UIBrickCounterDisplay : ICustomUpdate, IDisposable
{
    private readonly TextMeshProUGUI _brickCounterText;

    public UIBrickCounterDisplay(TextMeshProUGUI brickCounterText)
    {
        if (brickCounterText == null)
            Debug.LogError("UIBrickCounterDisplay: falta asignar brickCounterText en el Inspector.");
        _brickCounterText = brickCounterText;
        GameManager.Instance.Register(this);
    }

    public void Dispose()
    {
        GameManager.Instance.Unregister(this);
    }

    public void CustomUpdate(float deltaTime)
    {
        if (_brickCounterText == null) return;
        int bricksLeft = CountActiveBricks();
        _brickCounterText.text = $"Bricks Left: {bricksLeft}";
    }

    private int CountActiveBricks()
    {
        int count = 0;
        var bricks = GameObject.FindGameObjectsWithTag("Brick");
        foreach (var b in bricks)
            if (b.activeInHierarchy)
                count++;
        return count;
    }
}

