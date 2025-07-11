using UnityEngine;

public class BrickAtlasLogic
{
    private readonly Vector2 _tiling;
    private readonly Vector2 _offset;
    private readonly string _stPropName;

    public BrickAtlasLogic(float columns, float rows, float index, Renderer exampleRenderer)
    {
        // Calcula índice
        float total = columns * rows;
        int idx = Mathf.FloorToInt(Mathf.Clamp(index, 0, total - 1));

        int col = idx % (int)columns;
        int row = idx / (int)columns;

        // Tiling = tamaño de cada celda
        _tiling = new Vector2(1f / columns, 1f / rows);
        // Offset en UV (invertimos Y porque las V suben de abajo a arriba)
        _offset = new Vector2(col / (float)columns, 1f - (row + 1f) / rows);

        // Detecta si tu shader usa "_BaseMap_ST" o "_MainTex_ST"
        var mat = exampleRenderer.sharedMaterial;
        _stPropName =
            mat.HasProperty("_BaseMap_ST") ? "_BaseMap_ST" :
            mat.HasProperty("_MainTex_ST") ? "_MainTex_ST" :
            "_MainTex_ST";
    }

    public void Apply(Renderer renderer)
    {
        if (renderer == null) return;
        var block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetVector(_stPropName, new Vector4(
            _tiling.x, _tiling.y,
            _offset.x, _offset.y
        ));
        renderer.SetPropertyBlock(block);
    }
}


