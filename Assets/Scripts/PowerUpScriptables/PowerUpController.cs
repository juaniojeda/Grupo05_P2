using UnityEngine;
using YourGame.Utilities;  // ICustomUpdate

public class PowerUpController : ICustomUpdate
{
    private readonly GameManager _gm;
    private readonly PowerUpData _data;
    private readonly Transform _visual;
    private readonly Transform _paddle;
    private readonly float _halfWidth, _halfHeight;
    private readonly float _atlasCols, _atlasRows, _fixedIdx;
    private readonly bool _randomize;

    public PowerUpController(
        GameManager gm,
        PowerUpData data,
        Vector3 spawnPos,
        Transform paddle,
        float atlasCols,
        float atlasRows,
        bool randomize,
        float fixedIdx)
    {
        _gm = gm;
        _data = data;
        _paddle = paddle;
        _atlasCols = atlasCols;
        _atlasRows = atlasRows;
        _randomize = randomize;
        _fixedIdx = fixedIdx;

        // Obtener media de ancho/alto de la pala
        var paddleRend = paddle.GetComponent<Renderer>();
        _halfWidth = paddleRend.bounds.size.x * 0.5f;
        _halfHeight = paddleRend.bounds.size.y * 0.5f;

        // 1) Crear visual del power-up
        _visual = Object.Instantiate(data.visualPrefab, spawnPos, Quaternion.identity).transform;

        // 2) Aplicar atlas SOLO a este PowerUp
        var rend = _visual.GetComponent<Renderer>();
        if (rend != null)
        {
            float count = _atlasCols * _atlasRows;
            float idx = _randomize ? Random.Range(0f, count) : _fixedIdx;
            new BrickAtlasLogic(_atlasCols, _atlasRows, idx, rend).Apply(rend);
        }

        // 3) Registrarse en el loop de actualizaciones
        _gm.Register(this);
    }

    public void CustomUpdate(float dt)
    {
        // --- Validación temprana: si dejaron de existir referencias, nos damos de baja ---
        if (_visual == null || !_visual.gameObject.activeInHierarchy || _paddle == null)
        {
            Dispose();
            return;
        }

        // Caída del power-up
        _visual.position += Vector3.down * _data.fallSpeed * dt;

        // Destruir si baja demasiado
        if (_visual.position.y < _data.destroyY)
        {
            Dispose();
            return;
        }

        // Detectar colisión con la pala
        Vector3 pu = _visual.position;
        Vector3 p = _paddle.position;
        if (pu.x >= p.x - _halfWidth && pu.x <= p.x + _halfWidth &&
            pu.y >= p.y - _halfHeight && pu.y <= p.y + _halfHeight)
        {
            _data.effect.Activate(new PowerUpContext(_gm, _paddle));
            Dispose();
        }
    }

    private void Dispose()
    {
        _gm.Unregister(this);
        if (_visual != null)
            Object.Destroy(_visual.gameObject);
    }
}
