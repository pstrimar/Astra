using UnityEngine;

public class ObstacleDropLever : Lever, IUseable
{
    [SerializeField] ObstacleSpawner[] spawners;

    public override void Use()
    {
        base.Use();
        foreach (ObstacleSpawner spawner in spawners)
        {
            spawner.SpawnObstacle();
        }
    }
}
