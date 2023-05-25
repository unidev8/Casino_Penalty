using Firesplash.UnityAssets.SocketIO;
using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class SocketIOManager : MonoBehaviour
{
    public SocketIOCommunicator sioCom;
    public static SocketIOManager instance;

    // server values
    [HideInInspector]
    public int score;
    [HideInInspector]
    public int save;
    [HideInInspector]
    public int startTime;


    [SerializeField]
    private GameObject txt_ID;
    public GameObject txt_FUN;

    [SerializeField]
    private ScrollRect scr_CurrentBet;
    [SerializeField]
    private GameObject scr_CurrentBetItem;

    [SerializeField]
    private ScrollRect scr_History;
    [SerializeField]
    private GameObject scr_HistoryItem;

    [SerializeField]
    private ScrollRect scr_LeaderBoard;
    [SerializeField]
    private GameObject scr_LeaderBoardItem;

    [SerializeField]
    private ScrollRect scr_Result;
    [SerializeField]
    private GameObject scr_ResultItem;
    [SerializeField]
    private GameObject pan_Lowbalance;
    [SerializeField]
    private GameObject btn_Url;
    

    struct MyInfo
    {
        public int balance;
        public string userId;
        public string username;
        public bool status;
        public string url;
    } 

    [Serializable]
    struct PlayerData
    {
        public string token;
    }

    [Serializable]
    struct CurTime
    {
        public int curTime;
    }

    [Serializable]
    struct RoundResult
    {
        public int save;
        public int score;
    }

    [Serializable]
    struct UserBalance
    {
        public int balance;
        public int winAmount;
    }

    [Serializable]
    class Leaderboard
    {
        public string userId;
        public string username;
        public int  totalBet;
        public int totalWin;
    }

    [Serializable]
    struct Balance
    {
        public int balance;
    }

    [HideInInspector]
    public BetInfos[] arrayBetInfos;
    private Leaderboard[] leaderboards;

    void Start()
    {
        sioCom.Instance.On("connect", (string data) => {
            Debug.Log("SocketIO connected");
            
            PlayerData playerData = new PlayerData();
            playerData.token = GameManager.instance.tokenValue;
            //playerData.myToken = playerData.myToken.Substring(playerData.myToken.Length - 8);

            sioCom.Instance.Emit("enterRoom", JsonUtility.ToJson(playerData), false);         
            
        });

        sioCom.Instance.On("myInfo", (string data) =>
        {
            Debug.Log("myInfo:data = " + data);
            MyInfo myInfo = JsonUtility.FromJson<MyInfo>(data);
            SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text = myInfo.balance.ToString ();
            txt_ID.GetComponent<TMP_Text>().text = myInfo.username;
            GameManager.instance.playerID = myInfo.userId;

            if (myInfo.balance == 0)
            {
                pan_Lowbalance.SetActive(true);
                btn_Url.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = myInfo.url;
                return;
            }
        });

        //sioCom.Instance.Connect();

        // After 1.5s from round_result trigger
        sioCom.Instance.On("user-balance", (string data) => {
            //Debug.Log("user-balance/ New Round!, data = " + data);

            UserBalance userBalance = JsonUtility.FromJson<UserBalance>(data);

            // first time after load game should not be applied new round trigger.
            // canBet is true on loading game;
            if (!GameManager.instance.canBet)// first trigger from starting game
            {
                GameManager.instance.NewRound();
            }

            txt_FUN.GetComponent<TMP_Text>().text = userBalance.balance.ToString();
            GameManager.instance.winAmount = userBalance.winAmount;
        });
 
        sioCom.Instance.On("time-track", (string data) =>
        {
            //Debug.Log("time-track: " + data);
            CurTime curServerTime = new CurTime();
            curServerTime = JsonUtility.FromJson<CurTime>(data);
            startTime = curServerTime.curTime;

            if (startTime == ConstVars.constTimeCount)
            {
                GameManager.instance.canBet = true;
                //Debug.Log("time-track/ Can Bet!: " + data);
                GameManager.instance.CreatPlayer();
                GameManager.instance.btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET";
            }
        });
        // If I click bet button, This trigger is rised
        sioCom.Instance.On("myBetState", (string data) =>
        {
            Balance _balance = JsonUtility.FromJson<Balance>(data);
            txt_FUN.GetComponent<TMP_Text>().text = _balance.balance.ToString();
        });
        // After 2.5s from time_track Trigger
        sioCom.Instance.On("round-result", (string data) => {
            //Debug.Log("round-result/ Do Shoot!: " + data);
            RoundResult roundResult = new RoundResult();
            roundResult = JsonUtility.FromJson<RoundResult>(data);
            score = roundResult.score;
            save = roundResult.save;
            
            GameManager.instance.betCoinsNum = 0;
            GameManager.instance.DoShoot();
            GameManager.instance.txt_info.GetComponent<TMP_Text>().text = "";
            GameManager.instance.btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET";
        });

        sioCom.Instance.On("bet-info", (string data) =>
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                Debug.Log("Bet_Info: " + data);
                arrayBetInfos = JsonConvert.DeserializeObject<BetInfos[]>(data, settings);

                //Debug.Log("curInfoTabIdx = " + GameManager.instance.curInfoTabIdx);               

            }
            catch (Exception e)
            {
                Debug.Log("Data read error null = " + e);
            }

        });

        sioCom.Instance.On("leaderboard", (string data) =>
        {            
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            Debug.Log("leaderboard: " + data);
            leaderboards = JsonConvert.DeserializeObject<Leaderboard[]>(data, settings);
            
        });
    }

    public void ViewInfoPanel()
    {
        switch (GameManager.instance.curInfoTabIdx)
        {
            case 1:
                {
                    // delete all old contents  
                    foreach (Transform child in scr_CurrentBet.content)
                    {
                        Destroy(child.gameObject);
                    }
                    for (int i = 0; i < arrayBetInfos.Length; i++)
                    {
                        //if (scr_CurrentBet.content.childCount <=  )
                        GameObject scr_Item = GameObject.Instantiate(scr_CurrentBetItem, scr_CurrentBetItem.transform.position, Quaternion.identity);
                        scr_Item.transform.SetParent(scr_CurrentBet.content.transform, false);
                        //scr_Item.transform
                        //scr_Item.GetComponent<RectTransform>().localScale = GameManager.instance.panelInfoScale;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].name;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].betAmount;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].combination;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(3).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].winFun;
                        //Debug.Log("CurrentBets: = " + arrayBetInfos[i].ToString());
                    }
                    
                    //GameManager.instance.Panel_Btn_Current.SetActive(true);
                    break;
                }
            case 2:
                {
                    foreach (Transform child in scr_History.content)
                    {
                        Destroy(child.gameObject);
                    }
                    for (int i = 0; i < arrayBetInfos.Length; i++)
                    {
                        GameObject scr_Item = GameObject.Instantiate(scr_HistoryItem, scr_HistoryItem.transform.position, Quaternion.identity);
                        scr_Item.transform.SetParent(scr_History.content.transform, false);
                        //scr_Item.GetComponent<RectTransform>().localScale = GameManager.instance.panelInfoScale;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].roundId;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].betAmount;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].combination;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(3).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].winFun;
                        //Debug.Log("History: = " + arrayBetInfos[i].ToString());
                    }
                    break;
                }
            case 3:
                {
                    foreach (Transform child in scr_Result.content)
                    {
                        Destroy(child.gameObject);
                    }
                    for (int i = 0; i < arrayBetInfos.Length; i++)
                    {
                        GameObject scr_Item = GameObject.Instantiate(scr_ResultItem, scr_ResultItem.transform.position, Quaternion.identity);
                        scr_Item.transform.SetParent(scr_Result.content.transform, false);
                        //scr_Item.GetComponent<RectTransform>().localScale = GameManager.instance.panelInfoScale;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].roundId;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].combination;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text
                        = arrayBetInfos[i].winFun;
                        //Debug.Log("Result: = " + arrayBetInfos[i].ToString());
                    }
                    break;
                }
            case 4:
                {
                    foreach (Transform child in scr_LeaderBoard.content)
                    {
                        Destroy(child.gameObject);
                    }
                    for (int i = 0; i < leaderboards.Length; i++)
                    {
                        GameObject scr_Item = GameObject.Instantiate(scr_LeaderBoardItem, scr_LeaderBoardItem.transform.position, Quaternion.identity);
                        scr_Item.transform.SetParent(scr_LeaderBoard.content.transform, false);
                        //scr_Item.GetComponent<RectTransform>().localScale = GameManager.instance.panelInfoScale;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text
                        = leaderboards[i].userId;
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text
                        = leaderboards[i].totalBet.ToString();
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text
                        = "";
                        scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(3).gameObject.GetComponent<TMP_Text>().text
                        = leaderboards[i].totalWin.ToString();
                        //Debug.Log("LeaderBoard: = " + arrayBetInfos[i].ToString());
                    }
                    break;
                }
        }
        scr_CurrentBet.content.anchoredPosition = new Vector2 (0f, 0f);
        scr_History.content.anchoredPosition = new Vector2(0f, 0f);
        scr_LeaderBoard.content.anchoredPosition = new Vector2(0f, 0f);
        scr_Result.content.anchoredPosition = new Vector2(0f, 0f);        
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        scr_CurrentBet.content.anchoredPosition.Set(0, 0);
        scr_History.content.anchoredPosition.Set(0, 0);
        scr_LeaderBoard.content.anchoredPosition.Set(0, 0);
        scr_Result.content.anchoredPosition.Set(0, 0);
    }


}