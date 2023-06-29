using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerActions : MonoBehaviour
{   
    // keeper info to do animate when player shoot ball.
    [SerializeField]
    private GameObject KeeperPerfab;
    [HideInInspector]
    public GameObject Keeper;
    [HideInInspector]
    public Animator ani_Keeper;
    [HideInInspector]
    public Vector3 vKeeperPos;
    [HideInInspector]
    public Quaternion keeperRot;

    // player ball info
    private GameObject BallOnHand;
    [SerializeField]
    private GameObject preBall;

    private GameObject instanceBall;
    private Rigidbody rigidbodyBall;
    private Vector3 vBallFirstPos = new Vector3(0f, 0.12f, 51.7f); 
    private const float fGoalDoorZPos = 59.4f;
    private const float fForce = 1f;

    // delay time from flying to destory
    private const float fViewBallOnHand = 0.18f;

    [HideInInspector]
    public Vector3 BallDir;
    public static PlayerActions instance;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!instanceBall)
        {
            instanceBall = GameObject.Instantiate(preBall, vBallFirstPos, Quaternion.identity);
            //SphereCollider ballCollider = instanceBall.GetComponent<SphereCollider>();
            //ClothSphereColliderPair clothSphereColliderPair = new ClothSphereColliderPair();

            //clothSphereColliderPair.first = ballCollider;
            //clothSphereColliderPair.colliderDistance = 0.1f;
            //clothSphereColliderPair.interactionDistance = 0.2f;

            // Assign the ClothSphereColliderPair to the cloth
            //GameManager.instance.goalNet_1.sphereColliders = new ClothSphereColliderPair[] { clothSphereColliderPair };            
        }

        Keeper = GameObject.Instantiate(KeeperPerfab, KeeperPerfab.transform.position, KeeperPerfab.transform.rotation);
        ani_Keeper = Keeper.GetComponent<Animator>();
        keeperRot = Keeper.transform.localRotation;
        vKeeperPos =Keeper.transform.position;

        BallOnHand = GameObject.FindGameObjectWithTag("HandBall");
        BallOnHand.SetActive(false);

        //Debug.Log("Player is created!");

        Canvas.ForceUpdateCanvases();
    }

    private void OnDestroy()
    {
        GameObject.Destroy(Keeper);
        if (instanceBall)
            GameObject.Destroy(instanceBall);
        //Debug.Log("old Keeper and ball is destoried!");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShootBallEvent()
    {
        if ( instanceBall )
        {
            //Debug.Log("OnShootBallEvent: Triger!");

            rigidbodyBall = instanceBall.GetComponent<Rigidbody>();
            BallDir = new Vector3(GameManager.instance.arrayShootBallPos[GameManager.instance.nGoalKind -1].x,
                GameManager.instance.arrayShootBallPos[GameManager.instance.nGoalKind - 1].y, fGoalDoorZPos) - vBallFirstPos;
            //Debug.Log("GoalPos = " + GameManager.instance.arrayShootBallPos[GameManager.instance.nGoalKind - 1]);

            Vector3 dir = BallDir * fForce;
            rigidbodyBall.AddForce(dir.x, dir.y, dir.z, ForceMode.Impulse);
            int nKeeperKind = 0;
            if (GameManager.instance.bCatch)
            {
                int playerPosState = GameManager.instance.nGoalKind % 8;
                
                if (1 <= playerPosState && playerPosState <= 3)
                {
                    nKeeperKind = 11;
                }
                else if (4 <= playerPosState && playerPosState <= 5)
                {
                    nKeeperKind = 12;
                }
                else if (6 <= playerPosState || playerPosState == 0)
                {
                    nKeeperKind = 13;
                }
            }
            else
            {
                nKeeperKind = Random.Range (1, 11);
            }    

            switch (nKeeperKind)
            {
                case 1:
                    {
                        ani_Keeper.SetTrigger("Left_Low");
                        break;
                    }
                case 2:
                    {
                        ani_Keeper.SetTrigger("Left_Middle");
                        break;
                    }
                case 3:
                    {
                        ani_Keeper.SetTrigger("Left_Up");
                        break;
                    }
                case 4:
                    {
                        ani_Keeper.SetTrigger("Uper");
                        break;
                    }
                case 5:
                    {
                        ani_Keeper.SetTrigger("Down");
                        break;
                    }
                case 6:
                    {
                        ani_Keeper.SetTrigger("Right_Low");
                        break;
                    }
                case 7:
                    {
                        ani_Keeper.SetTrigger("Right_Middle");
                        break;
                    }
                case 8:
                    {
                        ani_Keeper.SetTrigger("Right_Up");
                        break;
                    }
                case 9:
                    {
                        ani_Keeper.SetTrigger("Left_Low0");
                        break;
                    }
                case 10:
                    {
                        ani_Keeper.SetTrigger("Right_Low0");
                        break;
                    }
                case 11:
                    {
                        ani_Keeper.SetTrigger("S_Right");
                        break;
                    }
                case 12:
                    {
                        ani_Keeper.SetTrigger("S_Left");
                        break;
                    }
                case 13:
                    {
                        ani_Keeper.SetTrigger("S_HighMiddle");
                        break;
                    }
            }
            //Debug.Log("nCatch State = "+ GameManager.instance.bCatch + ", Keeper State = " + nKeeperKind);

            if (GameManager.instance.bCatch)
            {
                StartCoroutine(ViewBallOnHand());
                GameObject.Destroy(instanceBall, fViewBallOnHand);
            }    
             
            StartCoroutine(GameManager.instance.ViewScoreBoard());
        }            
    }   

    IEnumerator ViewBallOnHand()
    {
        yield return new WaitForSeconds(fViewBallOnHand);
        BallOnHand.SetActive(true);
    }    
    
}
