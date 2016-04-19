using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    GameManager gm;

    Vector2 touchStartPosition;
    Vector2 touchEndPosition;

    // Min range to detect a swipe, otherwise a touch (1 = screenWidth)
    [Range(0,1.0f)]
    public float minSwipeInputLength = 0.1f;
    
    // Max time before not considering input as touch (in seconds)
    [Range(0, 1.0f)]
    public float maxTouchInputTime = 0.5f;

    float timerTouchInput = 0;

	void Start () {
        gm = GameManager.instance;
	}
	

    void Update()
    {

        if (Input.touchCount != 0)
        {
            Touch input = Input.GetTouch(0);

            if (input.phase == TouchPhase.Began)
            {
                touchStartPosition = input.position;
                timerTouchInput = 0;
            }
           
            else if(input.phase == TouchPhase.Ended)
            {
                touchEndPosition = input.position;
                
                if (Vector2.Distance(touchStartPosition, touchEndPosition) / Screen.width > minSwipeInputLength) 
                {
                    Vector3 direction = new Vector3(touchEndPosition.x - touchStartPosition.x, 0, touchEndPosition.y - touchStartPosition.y).normalized;
                    SwipeInput(direction);
                }
                else if (timerTouchInput <= maxTouchInputTime)
                {
                    TouchInput(touchEndPosition);
                }
            }

            timerTouchInput += Time.deltaTime;
            
        }
    }

    // If Touch input triggered
    void TouchInput(Vector2 _touchPosition)
    {
        if (_touchPosition.x < Screen.width / 2)
        {
            Dodge();
        }
        else {
            Attack();
        }
    }

    // If Swipe input triggered
    void SwipeInput(Vector3 _direction)
    {
        Debug.Log(_direction);
    }

    // Attack function
    void Attack() 
    {
        Debug.Log("Attack");
    }

    // Dodge function
    void Dodge() 
    {
        Debug.Log("Dodge");
    }

}
