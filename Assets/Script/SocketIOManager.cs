using Firesplash.UnityAssets.SocketIO;
using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;


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
    private SchemaHistory[] schemaHistory;


    [Serializable]
    class SchemaHistory
    {
        public int _id;
        public int roundId;
        public string userId;
        public string userName;
        public int betAmount;
        public int winAmount;
        public int[] betScores;
        public string date;
    }

    [Serializable]
    class UserId
    {
        public string userId;
    }
    UserId userID;

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
            //Debug.Log("myInfo:data = " + data);
            MyInfo myInfo = JsonUtility.FromJson<MyInfo>(data);
            SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text = myInfo.balance.ToString ();
            txt_ID.GetComponent<TMP_Text>().text = myInfo.username;
            GameManager.instance.playerID = myInfo.userId;
            userID = new UserId();
            userID.userId = myInfo.userId;

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
                GameManager.instance.EnableTglBets();
                //Debug.Log("time-track/ Can Bet!: " + data);
                GameManager.instance.CreatPlayer();
                GameManager.instance.btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = ConstVars.btnBetName;
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
            GameManager.instance.btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = ConstVars.btnBetName;
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
                //Debug.Log("Bet_Info: " + data);
                arrayBetInfos = JsonConvert.DeserializeObject<BetInfos[]>(data, settings);
                ViewInfoPanel();
                //Debug.Log("curInfoTabIdx = " + GameManager.instance.curInfoTabIdx);               

            }
            catch (Exception e)
            {
                Debug.Log("Data read error null = " + e);
            }

        });

        sioCom.Instance.On("my-history", (string data) =>
        {
            //Debug.Log("Schemahistory = " + data);
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            schemaHistory = JsonConvert.DeserializeObject<SchemaHistory[]>(data, settings);

            if (GameManager.instance.curInfoTabIdx != 2) return;

            for (int i = 0; i < schemaHistory.Length; i++)
            {
                GameObject scr_Item = GameObject.Instantiate(scr_HistoryItem, scr_HistoryItem.transform.position, Quaternion.identity);
                scr_Item.transform.SetParent(scr_History.content.transform, false);
                //scr_Item.GetComponent<RectTransform>().localScale = GameManager.instance.panelInfoScale;
                scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text
                = schemaHistory[i].roundId.ToString();
                //Debug.Log("roundID =" + schemaHistory[i].roundId.ToString());
                scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text
                = schemaHistory[i].betAmount.ToString();
                //Debug.Log("betAmount =" + schemaHistory[i].betAmount.ToString());
                string combination = "";
                foreach (int item in schemaHistory[i].betScores)
                {
                    string strItem = "";
                    switch (item)
                    {
                        case 25:
                            {
                                strItem = "GOALPOST,";
                                break;
                            }
                        case 26:
                            {
                                strItem = "MISS/SAVE,";
                                break;
                            }
                        case 27:
                            {
                                strItem = "TOP,";
                                break;
                            }
                        case 28:
                            {
                                strItem = "MIDDLE,";
                                break;
                            }
                        case 29:
                            {
                                strItem = "BOTTOM,";
                                break;
                            }
                        case 30:
                            {
                                strItem = "BLUE,";
                                break;
                            }
                        case 31:
                            {
                                strItem = "YELLOW,";
                                break;
                            }
                    }
                    combination += strItem;

                }
                scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text
                = combination;
                //Debug.Log("combination =" + combination);
                scr_Item.GetComponent<ScrollRect>().content.transform.GetChild(3).gameObject.GetComponent<TMP_Text>().text
                = schemaHistory[i].winAmount.ToString();
                Debug.Log("winAmount =" + schemaHistory[i].winAmount.ToString());

            }

            RectTransform contentRectTransform = scr_History.content.GetComponent<RectTransform>();            
            float newContentHeight = scr_HistoryItem.GetComponent<RectTransform>().rect.height * schemaHistory.Length;
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, newContentHeight);
        });  

        sioCom.Instance.On("leaderboard", (string data) =>
        {            
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            //Debug.Log("leaderboard: " + data);
            leaderboards = JsonConvert.DeserializeObject<Leaderboard[]>(data, settings);
            
        });
    }

    void ClearOldHistory()
    {
        Array.Clear(schemaHistory, 0, schemaHistory.Length);
        schemaHistory = null;
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
                        //Debug.Log("CurrentBets: = " + arrayBetInfos[i].ToString());
                    }
                    //ClearOldHistory();
                    //GameManager.instance.Panel_Btn_Current.SetActive(true);
                    break;
                }
            case 2:
                {
                    sioCom.Instance.Emit("my-history", JsonUtility.ToJson(userID), false);
                    
                    foreach (Transform child in scr_History.content)
                    {
                        Destroy(child.gameObject);
                    }
                    
                    //if (schemaHistory == null) return;                    
                    
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
                    //ClearOldHistory();
                    break;
                }
        }
        scr_CurrentBet.content.anchoredPosition = new Vector2 (0f, 0f);
        scr_History.content.anchoredPosition = new Vector2(0f, 0f);
        scr_LeaderBoard.content.anchoredPosition = new Vector2(0f, 0f);
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
    }

}