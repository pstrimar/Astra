using UnityEngine;

[ExecuteInEditMode()]
public class ParallaxLayer : MonoBehaviour {
	[Range(-2f, 1f)]
	public float relativeMove = 1f;
	public Transform cam;

	private void Update() 
	{
		// Move transform x position relative to camera x position
		transform.position = new Vector2(cam.position.x * relativeMove, transform.position.y);
	}
}
