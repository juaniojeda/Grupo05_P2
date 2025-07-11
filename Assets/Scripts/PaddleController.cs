using UnityEngine;
using YourGame.Utilities;

public class PaddleController : ICustomUpdate
{
    private Transform _transform;
    private float _speed;
    private float _halfWidth;
    private float _xLimit;

  
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public PaddleController(Transform transform, PaddleData data)
    {
        _transform = transform;
        ResetWithData(data);
    }

 
    public void ResetWithData(PaddleData data)
    {
        Speed = data.speed;            // ahora usamos la propiedad
        _halfWidth = data.width * 0.5f;
        _xLimit = data.xLimit;
        // si ajustas escala, aquí:
        // _transform.localScale = new Vector3(data.width, data.height, 1f);
    }

    public void CustomUpdate(float deltaTime)
    {

        float move = Input.GetAxis("Horizontal") * Speed * deltaTime;
        Vector3 pos = _transform.position + Vector3.right * move;
        pos.x = Mathf.Clamp(pos.x, -_xLimit + _halfWidth, _xLimit - _halfWidth);
        _transform.position = pos;
    }
}