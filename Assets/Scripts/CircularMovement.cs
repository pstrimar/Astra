using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    [Range(0, 5)]
    public float RotateSpeed = 1f;
    [Range(0, 5)]
    public float Radius = 1f;

    private Vector2 _center;
    private float _angle;

    private void Start()
    {
        _center = transform.position;
    }

    private void Update()
    {

        _angle += RotateSpeed * Time.deltaTime;

        var offset = new Vector2(Mathf.Sin(_angle), Mathf.Cos(_angle)) * Radius;

        transform.position = _center + offset;
    }
}
