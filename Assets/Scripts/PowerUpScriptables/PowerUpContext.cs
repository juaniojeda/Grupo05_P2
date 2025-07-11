using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PowerUpContext
{
    public readonly GameManager gameManager;
    public readonly Transform paddle;

    public PowerUpContext(GameManager gm, Transform paddle)
    {
        this.gameManager = gm;
        this.paddle = paddle;
    }
}