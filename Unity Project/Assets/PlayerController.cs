using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    GameManager gm;
    Transform selfTransform;

    Vector2 touchStartPosition;
    Vector2 touchEndPosition;

    [Header("DEBUG VARIABLES")]
    public Material green;
    public Material red;

    // Player stats
    [Header("Player Stats")]
    public int health = 150;
    public int attackDamage = 15;

    float meleeRange = 2.12f;
    float midRange = 3.51f;
    float longRange = 6.0f;


    //Player Animations
    Animator selfAnimator;


    // Swipe and Touch options
    [Header("Inputs Options")]
    [Tooltip("Min swipe length to detect a swipe, otherwise it's a touch (1=screenHeight)")]
    [Range(0,1.0f)]
    public float minSwipeInputLength = 0.1f;

    [Tooltip("Max time before not considering input as touch (in seconds)")]
    [Range(0, 1.0f)]
    public float maxTouchInputTime = 0.5f;

    [Tooltip("Dot product between enemyDirection and swipeDirection (in dregrees)")]
    [Range(0, 20f)]
    public float maxSwipeConeAngleTolerance = 10.0f;

    [Tooltip("from which angle difference two ennemies are considered as close as swipe direction (in degrees)")]
    [Range(0, 5f)]
    public float angleTolerence = 2.0f;

    float timerTouchInput = 0;

    public EnemyBehaviour enemySelected;

	void Start () {
        gm = GameManager.instance;
        selfAnimator = GetComponentInChildren<Animator>();
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
                
                if (Vector2.Distance(touchStartPosition, touchEndPosition) / Screen.height > minSwipeInputLength) 
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
            float angleToSwipe = Mathf.Acos(dot) * Mathf.Rad2Deg;

            // if enemy is in cone
            if (angleToSwipe <= maxSwipeConeAngleTolerance)
            {
                
                // if enemy tested is already our selection, ignore it
                if (gm.enemyList[i] == enemySelected) continue;

                // if is 1st enemy tested, then stock it
                if (enemyToSelect == null) 
                {
                    enemyToSelect = gm.enemyList[i];
                    distToEnemyToSelect = Vector2.Distance(enemyPositionOnScreen, playerPositionOnScreen)/Screen.height;
                    angleToEnemyToSelect = 1 - dot;
                    continue;
                }
               
                else 
                {
                    // if threat bigger, replace and stock it
                    if (gm.enemyList[i].threatLevel > enemyToSelect.threatLevel) 
                    {
                        enemyToSelect = gm.enemyList[i];
                        distToEnemyToSelect = Vector2.Distance(enemyPositionOnScreen, playerPositionOnScreen) / Screen.height;
                        angleToEnemyToSelect = 1 - dot;
                        continue;
                    }

                    // if threat are equals, test distance and swipe angle
                    else if(gm.enemyList[i].threatLevel == enemyToSelect.threatLevel)
                    {
                        
                        // if closer to swipe angle than previous (minus tolerance), stock it
                        if (angleToSwipe <= angleToEnemyToSelect - angleTolerence)
                        {
                            enemyToSelect = gm.enemyList[i];
                            distToEnemyToSelect = Vector2.Distance(enemyPositionOnScreen, playerPositionOnScreen) / Screen.height;
                            angleToEnemyToSelect = 1 - dot;
                            continue;
                        }
                        
                        // else if two ennemies are the closest to swipe direction (more or less tolerance), check distance
                        else if (angleToSwipe < angleToEnemyToSelect + angleTolerence && angleToSwipe > angleToEnemyToSelect - angleTolerence)
                        {
                            // take the nearest to player
                            if (Vector2.Distance(enemyPositionOnScreen, playerPositionOnScreen) / Screen.height < distToEnemyToSelect)
                            {
                                enemyToSelect = gm.enemyList[i];
                                distToEnemyToSelect = Vector2.Distance(enemyPositionOnScreen, playerPositionOnScreen) / Screen.height;
                                angleToEnemyToSelect = 1 - dot;
                                continue;
                            }
                            else continue;
                        }
                      
                    }
                }
            }
            // if not in cone
            else continue;
        }

        // if an ennemy was found, then select it
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
