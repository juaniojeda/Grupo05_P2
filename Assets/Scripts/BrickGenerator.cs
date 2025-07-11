using UnityEngine;

public class BrickGenerator
{
    private readonly BrickPool pool;
    private readonly BrickGeneratorConfig config;
    private readonly Transform origin;
    private readonly Transform parent;

   
    private GameObject chosenPrefab;

    public BrickGenerator(
        BrickPool pool,
        BrickGeneratorConfig config,
        Transform startTransform,
        Transform parentTransform)
    {
        this.pool = pool;
        this.config = config;
        origin = startTransform;
        parent = parentTransform;
    }

    public void GenerateBricks()
    {
    
        pool.ReturnAllBricks();

        
        chosenPrefab = config.brickPrefabs[
            Random.Range(0, config.brickPrefabs.Length)
        ];

       
        float cellW = 0f, cellH = 0f;
        foreach (var pref in config.brickPrefabs)
        {
            var r = pref.GetComponent<Renderer>();
            if (r == null) continue;
            cellW = Mathf.Max(cellW, r.bounds.size.x);
            cellH = Mathf.Max(cellH, r.bounds.size.y);
        }

       
        for (int y = 0; y < config.rows; y++)
            for (int x = 0; x < config.columns; x++)
            {
                Vector3 worldPos = origin.position + new Vector3(
                    x * (cellW + config.spacing),
                    y * (cellH + config.spacing),
                    0f);

          
                var brick = pool.GetBrick(chosenPrefab, worldPos);

            
                brick.transform.SetParent(parent, worldPositionStays: true);

    
                var p = brick.transform.position;
                brick.transform.position = new Vector3(p.x, p.y, 0f);
                foreach (var rend in brick.GetComponentsInChildren<Renderer>())
                {
                    rend.sortingLayerName = "Bricks";
                    if (rend is SpriteRenderer sr)
                        sr.sortingOrder = 0;
                }
            }
    }
}
