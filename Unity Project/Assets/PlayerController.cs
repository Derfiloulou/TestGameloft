using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    GameManager gm;
    Transform selfTransform;

    Vector2 touchStartPosition;
    Vector2 touchEndPosition;

    [Header("DEBUG VARIABLES")]
    Material green;
    Material red;

    [Header("Inputs Options")]
    [Space(10)]
    [Tooltip("Min swipe length to detect a swipe, otherwise it's a touch (1=screenWidth)")]
    [Range(0,1.0f)]
    public float minSwipeInputLength = 0.1f;

    [Tooltip("Max time before not considering input as touch (in seconds)")]
    [Range(0, 1.0f)]
    public float maxTouchInputTime = 0.5f;

    [Tooltip("Dot product between enemyDirection and swipeDirection (1-dotProduct)")]
    [Range(0, 0.3f)]
    public float maxSwipeConeAngleTolerance = 0.05f;

    [Tooltip("Priority of distance (-1), or close to swipe (1) on selection.")]
    [Range(-1, 1f)]
    public float priorityOnSelection = 0f;

    float timerTouchInput = 0;

    EnemyBehaviour enemySelected;

	void Start () {
        gm = GameManager.instance;
        selfTransform = GetComponent<Transform>();
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
                    SwipeInput((touchEndPosition - touchStartPosition).normalized);
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
    void SwipeInput(Vector2 _swipeDirection)
    {
        Vector2 playerPositionOnScreen = gm.cam.WorldToScreenPoint(selfTransform.position);
        EnemyBehaviour enemyToSelect = null;
        float distToEnemyToSelect = -1;
        float angleToEnemyToSelect = -1;

        for(int i=0; i<gm.enemyList.Length; i++)
        {
           
            Vector2 enemyPositionOnScreen = gm.cam.WorldToScreenPoint(gm.enemyList[i].GetComponent<Transform>().position);
            Vector2 enemyDirectionOnScreen = (enemyPositionOnScreen - playerPositionOnScreen).normalized;
            float dot = Vector2.Dot(_swipeDirection, enemyDirectionOnScreen);

            // if enemy is in cone
            if (dot >= 1 - maxSwipeConeAngleTolerance)
            {

                // if enemy tested is already our selection, ignore it
                if (gm.enemyList[i] == enemySelected) continue;

                // if is 1st enemy tested, then stock it
                if (enemyToSelect == null) {
                    enemyToSelect = gm.enemyList[i];
                    distToEnemyToSelect = Vector2.Distance(enemyPositionOnScreen, playerPositionOnScreen)/Screen.height;
                    angleToEnemyToSelect = 1 - dot;
                }
               
                else 
                {
                    // if threat bigger, replace and stock it
                    if (gm.enemyList[i].threatLevel > enemyToSelect.threatLevel) {
                        enemyToSelect = gm.enemyList[i];
                        distToEnemyToSelect = Vector2.Distance(enemyPositionOnScreen, playerPositionOnScreen) / Screen.height;
                        angleToEnemyToSelect = 1 - dot;
                    }
                    // if threat are equals, test distance and swipe angle
                    else if(gm.enemyList[i].threatLevel == enemyToSelect.threatLevel){
                        ///Vector2.Distance(enemyPositionOnScreen, playerPositionOnScreen);
                    }
                }
            }
            else continue;
        }

        if (enemyToSelect != null) 
        {
            enemySelected.GetComponent<Renderer>().material = red;
            enemySelected = enemyToSelect;
            enemyToSelect.GetComponent<Renderer>().material = green;
        }

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
