using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tiling : MonoBehaviour
{
    [SerializeField] int offsetX = 2;               // Offset so that we don't get any weird errors

    // These are used for checking if we need to instantiate stuff
    [SerializeField] bool hasARightBuddy = false;   
    [SerializeField] bool hasALeftBuddy = false;

    [SerializeField] bool reverseScale = false;     // Used if the object is not tileable

    private float repeatWidth = 0f;                 // The width of our element
    private float centerPos = 0f;                   // The center position of our element
    private Camera cam;
    private Transform myTransform;

    private void Awake() 
    {
        cam = Camera.main;
        myTransform = transform;
    }

    void Start()
    {
        repeatWidth = GetComponent<BoxCollider2D>().bounds.size.x;
        centerPos = GetComponent<BoxCollider2D>().bounds.center.x;
    }

    void Update()
    {
        // Does it still need buddies? If not, do nothing
        if (hasALeftBuddy == false || hasARightBuddy == false)
        {
            // Calculate half the width of what the camera can see in world coordinates
            float camHalfWidth = cam.orthographicSize * Screen.width/Screen.height;

            // Calculate the x position where the camera can see the edge of the element
            float edgeVisiblePositionRight = (centerPos + repeatWidth / 2) - camHalfWidth;
            float edgeVisiblePositionLeft = (centerPos - repeatWidth / 2) + camHalfWidth;

            // Checking if we can see the edge of the element and then calling MakeNewBuddy if we can
            if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasARightBuddy) 
            {
                MakeNewBuddy(1);
                hasARightBuddy = true;
            }
            else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasALeftBuddy)
            {
                MakeNewBuddy(-1);
                hasALeftBuddy = true;
            }
        }
    }

    // A function that creates a buddy on the side required
    void MakeNewBuddy(int rightOrLeft) 
    {               
        // Calculating the new position for our new buddy
        Vector3 newPosition = new Vector3(myTransform.position.x + repeatWidth * rightOrLeft, myTransform.position.y, myTransform.position.z);
        // Instantiating our new buddy and storing him in a variable
        Transform newBuddy = Instantiate(myTransform, newPosition, myTransform.rotation) as Transform;

        // If not tileable, let's reverse the x size of our object to get rid of ugly seams
        if (reverseScale) 
        {
            newBuddy.localScale = new Vector3(newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
        }
        
        newBuddy.parent = myTransform.parent;

        if (rightOrLeft > 0) 
        {
            newBuddy.GetComponent<Tiling>().hasALeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<Tiling>().hasARightBuddy = true;
        }
    }
}
