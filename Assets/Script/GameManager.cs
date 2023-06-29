using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Web;
using System.Runtime.CompilerServices;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject img_BallAni;
    [SerializeField]
    private ScrollRect scrollRect_History;
    [SerializeField]
    private GameObject img_BlueHistory;
    [SerializeField]
    private GameObject img_MissHistory;
    [SerializeField]
    private GameObject img_YellowHistory;

    [SerializeField]
    private GameObject img_GoalBoard;
    [SerializeField]
    private GameObject tgl_XGoalPost;
    [SerializeField]
    private GameObject tgl_XMiss;
    [SerializeField]
    private GameObject tgl_XTopRow;
    [SerializeField]
    private GameObject tgl_XMiddleRow;
    [SerializeField]
    private GameObject tgl_XDownRow;
    [SerializeField]
    private GameObject tgl_XBlue;
    [SerializeField]
    private GameObject tgl_XYellow;
    [SerializeField]
    private GameObject img_WinCup;
    [SerializeField]
    private GameObject panel_Info;

    // Goal Hit Marks
    [SerializeField]
    private GameObject HitMark_Blue;
    [SerializeField]
    private GameObject HitMark_Yellow;
    [SerializeField]
    private GameObject hitplane;
    private GameObject instHitMark;
    private const float GoalBoardApearTime = 0.88f;
    private const float hitMarkZPos = 59f;

    private bool bGoalBoardTransition = false;
    private float goalBoardTransitionValue = 0f;
    private const int fTransitionSpeed = 2;

    // user interface for interactive
    [SerializeField]
    private GameObject img_BtnBG;
    [SerializeField]
    private GameObject btn_CurrentBet;
    [SerializeField]
    private GameObject btn_History;
    [SerializeField]
    private GameObject btn_Result;
    [SerializeField]
    private GameObject btn_Leaders;
    [SerializeField]
    private GameObject pnl_BtnGropMove;
    [HideInInspector]
    public GameObject Panel_Btn_Leaders;
    [HideInInspector]
    public GameObject Panel_Btn_Histroy;
    [HideInInspector]
    public GameObject Panel_Btn_Current;
    [SerializeField]
    private GameObject Panel_View;
    [SerializeField]
    private GameObject Panel_Info;
    [SerializeField]
    private GameObject Panel_Top;
    [SerializeField]
    private GameObject Panel_AnalyTab;
    [SerializeField]
    private GameObject txt_Time;
    [SerializeField]
    private GameObject img_TimeBG;
    [SerializeField]
    private GameObject HitPlane;
    public GameObject img_ViewBG;
    public GameObject container_tglHitNum;
    public GameObject txt_info;
    public GameObject txt_Value;
    [SerializeField]
    private CanvasScaler canvasScaler;
    [SerializeField]
    private ScrollRect HistoryContainer;
    public GameObject btn_BetWin;

    [HideInInspector]
    public sbyte curInfoTabIdx = 1;
    private bool btnGroupMoved = false;

    // betting informations
    [HideInInspector]
    public Vector2[] arrayShootBallPos = new Vector2[31];
    [HideInInspector]
    public int[] seletedTgglarray;
    [HideInInspector]
    public int[] arrayBetToggle = new int[31];

    [HideInInspector]
    public bool bCatch; // Save

    [HideInInspector]
    public int nGoalKind; // 1 to 24 goal in / 25, 29, 31: Miss(right, up, left)/ 26, 28, 30: GoalPost     
    [HideInInspector]
    public byte goalState;// 0 = Goal in, 1 = Goal Post, 2 = Goal Miss

    // game states
    [HideInInspector]
    public bool canBet = true;  // dont change - this is only mark of start game;
    // 1 = view state of round result, user-balance trigger
    // 2 = betable state, countdwon start triger.
    // 3 = view state of playing - shoot ball, round-result trigger
    [HideInInspector]
    public byte GameState;

    // player informations
    [HideInInspector]
    public string playerID;
    [SerializeField]
    private GameObject[] PlayerList;
    [HideInInspector]
    public GameObject Player;
    private Animator ani_Player;

    private Vector3[] vPlayerPos = new Vector3[3];
    private Vector3[] vplayerRot = new Vector3[3];

    public Cloth goalNet_1;
    public Cloth goalNet_2;
    public Cloth goalNet_3;
    public Cloth goalNet_4;

    private bool isMobile = false;

    [HideInInspector]
    public int winAmount;
    [HideInInspector]
    public Vector3 panelViewScale = Vector3.one;
    [HideInInspector]
    public Vector3 panelInfoScale = Vector3.one;
    [HideInInspector]
    public int betCoinsNum = 0;
    //[HideInInspector]
    //public Dictionary<string, BetInfo> dicBetInfo;

    private int resolutionX;
    private int resolutionY;

    public static GameManager instance;
    public string tokenValue = "";

    private float fInfoTextDelay = 1.2f; // delay time for some erro anf notice on top of screen

    private byte curPlayerIdx = 0;

    // for local test (the time is time to elapsed  )
    //[HideInInspector]
    //public float startTime = 0f;

    [Serializable]
    struct BetData
    {
        public string userId;
        public int[] scores;
        public int betAmount;
    }

    [Serializable]
    struct PlayerIno
    {
        public string userId;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        if (!instance)
            instance = this;

        if (UseJSLib.IsAndroid() || UseJSLib.IsIos())
        {
            isMobile = true;
            //Debug.Log("this platform is Mobile!");
        }
        else
        {
            isMobile = false;
            //Debug.Log("this platform isnot Mobile!");
        }

        resolutionX = Screen.width;
        resolutionY = Screen.height;

        //OperatingSystemFamily sysInfo = SystemInfo.operatingSystemFamily ;
        //Debug.Log("Your System is " + sysInfo.ToString());
        //if (sysInfo.ToString() == "Windows  

    }

    void Start()
    {
        //Application.targetFrameRate = 60;
        //Application.runInBackground = true;        

        //isMobile = true;
        if (isMobile)
        {
            MoblieUI();
            //MobileModeUI(UseJSLib.GetWindowWidth());
        }

        //startTime = Time.time;        
        vPlayerPos[0] = new Vector3(-1.9f, 0, 49.8f);
        vPlayerPos[1] = new Vector3(-0.2f, 0, 49.3f);
        vPlayerPos[2] = new Vector3(1.8f, 0f, 49.8f);
        vplayerRot[0] = new Vector3(0f, 35f, 0f);
        vplayerRot[1] = new Vector3(0f, 0f, 0f);
        vplayerRot[2] = new Vector3(0f, -45f, 0f);

        SetArrayHitPos();
        curInfoTabIdx = 1;

        Player = GameObject.Instantiate(PlayerList[curPlayerIdx], PlayerList[curPlayerIdx].transform.position, Quaternion.identity);
        ani_Player = Player.GetComponent<Animator>();

        string url = UseJSLib.GetSearchParams();
        if (url != "")
            tokenValue = url.Substring(url.IndexOf("=") + 1); //HttpUtility.ParseQueryString(new Uri(url).Query).Get("cert");
        //Debug.Log("_URL = " + url + ", tokenValue = " + tokenValue);
    }

    public void Refund()
    {
        if (!canBet) return;

        //PlayerIno myInfo = new PlayerIno();
        //myInfo.userId = GameManager.instance.playerID;
        //SocketIOManager.instance.sioCom.Instance.Emit("refund", JsonUtility.ToJson(myInfo), false);
        Application.ExternalCall("quitted");
    }

    public void OpenUrl(GameObject thisObj)
    {
        //if (thisObj.transform.GetChild (0).gameObject.GetComponent <TMP_Text>().text != "")
        {
            //StartCoroutine(OpenInSelfTabCoroutine(thisObj));
            Application.OpenURL("https://induswin.com/#/pages/recharge/recharge");// thisObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text);
        }
    }

    IEnumerator OpenInSelfTabCoroutine( GameObject obj)
    {
        yield return new WaitForSeconds(0.5f); // wait for the browser to open the new tab
        string script = "window.location.href =" + obj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text; // JavaScript to reload the current page
        Application.ExternalEval(script); // 
    }

    private void MoblieUI()
    {
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);

        float expandRatioX = Screen.width / Panel_Top.GetComponent<RectTransform>().rect.width;
        float expandRatioY = expandRatioX; //Screen.height / Panel_Top.GetComponent<RectTransform>().rect.height;
        Panel_Top.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
        Panel_Top.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
         
        //Debug.Log("Screen Size = " + Screen.width + "/" + Screen.height);
        //Debug.Log("Panel_View Size = " + Panel_View.GetComponent<RectTransform>().rect.ToString());

        float yPos1 = Panel_Top.GetComponent<RectTransform>().rect.height * expandRatioY;
        expandRatioX = Screen.width / Panel_View.GetComponent<RectTransform>().rect.width;
        expandRatioY = (Screen.height - yPos1) * 0.56f / Panel_View.GetComponent<RectTransform>().rect.height;
        Panel_View.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
        Panel_View.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -yPos1, 0);
        panelViewScale = new Vector3(expandRatioX, expandRatioY, 1f);
        //Debug.Log("After applied: Panel_View Size = " + Panel_View.GetComponent<RectTransform>().rect.ToString());
        //Debug.Log("PanelViewPos = " + Panel_View.transform.position + "RectPos = " + Panel_View.GetComponent<RectTransform>().anchoredPosition);

        float yPos2 = yPos1 + (Screen.height - yPos1) * 0.56f;
        expandRatioX = Screen.width / Panel_Info.GetComponent<RectTransform>().rect.width;
        expandRatioY = (Screen.height - yPos1) * 0.44f / Panel_Info.GetComponent<RectTransform>().rect.height;
        Panel_Info.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
        Panel_Info.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -yPos2, 0);
        //Debug.Log("After applied: Panel_Info pos = " + Panel_Info.GetComponent<RectTransform>().anchoredPosition.ToString());
        panelInfoScale = new Vector3(expandRatioX, expandRatioY, 1f);
    }

    private void MobileTypeUI(int screenWidth)
    {
        //Debug.Log("BrowserScreenWidth = " + screenWidth + ", GameScreenWidth = " + Screen.width + ", GameScreenHeight = " + Screen.height );

        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);

        float expandRatioX1 = Screen.width / ConstVars.designeWidth * 2.272f;// (DeskTop- width(1920f) / (Mobile- width (845f)) of orignial PanelTop
        float expandRatioY1 = Screen.height / ConstVars.designeHeight * 1f;// 80f / 80f
        Panel_Top.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX1, expandRatioY1, 1f);
        Panel_Top.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        //Debug.Log("Panel_Top ScaleSize = " + Panel_Top.GetComponent<RectTransform>().localScale.ToString());

        //Debug.Log("Panel_Top height = " + Panel_Top.GetComponent<RectTransform>().rect.height);
        float yPos1 = Panel_Top.GetComponent<RectTransform>().rect.height * expandRatioY1;
        float expandRatioX2 = Screen.width / ConstVars.designeWidth * 1.778f;// (DeskTop- width(1920f) / (Mobile- width (1080f)) of orignial PanelTop
        float expandRatioY2 = Screen.height / ConstVars.designeHeight * 0.5f;// 540f / 1080f
        Panel_View.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX2, expandRatioY2, 1f);
        Panel_View.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -yPos1, 0);
        panelViewScale = Panel_View.GetComponent<RectTransform>().localScale;
        //Debug.Log("Panel_View ScaleSize = " + Panel_View.GetComponent<RectTransform>().localScale.ToString()+ "Panel_View Pos = " + Panel_View.GetComponent<RectTransform>().anchoredPosition);

        //Debug.Log("Panel_Info Size = " + Panel_View.GetComponent<RectTransform>().rect.ToString());
        float yPos2 = yPos1 + Panel_View.GetComponent<RectTransform>().rect.height * expandRatioY2;
        float expandRatioX3 = Screen.width / ConstVars.designeWidth * 2.272f;// (DeskTop- width(1920f) / (Mobile- width (845f)) of orignial PanelTop
        float expandRatioY3 = Screen.height / ConstVars.designeHeight * 0.5f;// 1080f - 80f - 540f / 880f
        Panel_Info.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX3, expandRatioY3, 1f);
        Panel_Info.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -yPos2, 0);
        panelInfoScale = Panel_Info.GetComponent<RectTransform>().localScale;
        //Debug.Log("Panel_Info ScaleSize = " + Panel_Info.GetComponent<RectTransform>().localScale.ToString() + "Panel_Info Pos = " + yPos2);
    }


    private void DeskTopModeUI(int screenWidth)
    {
        //Debug.Log("BrowserScreenWidth = " + screenWidth + ", GameScreenWidth = " + Screen.width + ", GameScreenHeight = " + Screen.height);

        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);

        float expandRatioX = (float)Screen.width / ConstVars.designeWidth;
        float expandRatioY = (float)Screen.height / ConstVars.designeHeight;
      
        GameObject panel_View = Panel_View;
        panel_View.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
        float ViewPosY = -10f * expandRatioY; // 50 - original Panel_Top PosY
        Panel_View.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, ViewPosY, 0f);
        panelViewScale = Panel_View.GetComponent<RectTransform>().localScale;
        //Debug.Log("Panel_View Scale = " + Panel_View.GetComponent<RectTransform>().localScale.ToString());

        Panel_Top.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
        float TopPosX = 1078f * expandRatioX;// 1078 - original Panel_Top PosX
        float TopPosY = -12f * expandRatioY; // -12 - original Panel_Top PosY
        Panel_Top.GetComponent<RectTransform>().anchoredPosition = new Vector3(TopPosX, TopPosY, 0);
        //Debug.Log("Panel_Top ScaleSize = " + Panel_Top.GetComponent<RectTransform>().localScale.ToString());

        //Debug.Log("Panel_Info Pos = " + Panel_Info.GetComponent<RectTransform>().anchoredPosition.ToString()
        //    + " \n Panel_Info world Pos = " + Panel_Info.transform.position );
        Panel_Info.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
        float InfoPosX = 1078f * expandRatioX; //1078 - original Panel_Info PosX
        float InfoPosY = -202f * expandRatioY; //-202 - original Panel_Top PosX
        Panel_Info.GetComponent<RectTransform>().anchoredPosition = new Vector3(InfoPosX, InfoPosY);
        panelInfoScale = Panel_Info.GetComponent<RectTransform>().localScale;
        //Debug.Log("Panel_Info Scale = " + Panel_Info.GetComponent<RectTransform>().localScale.ToString());

    }

    public void Bet()
    {
        if (!NotYetBet()) return;
        
        BetData betData = new BetData();
        betData.userId = playerID;
        betData.betAmount = int.Parse (txt_Value.GetComponent<TMP_Text>().text);
        int j = 0;
        for (int i = 0; i < arrayBetToggle.Length; i++)
        {
            if (arrayBetToggle[i] != 0)
                j++;
        }
        seletedTgglarray = new int [j];
        j = 0;
        for (int i = 0; i < arrayBetToggle.Length; i++)
        {
            if (arrayBetToggle[i] != 0)
            {
                seletedTgglarray[j] = arrayBetToggle[i];
                j++;
            }                
        }
        betData.scores = seletedTgglarray;
        if (seletedTgglarray.Length > 0)
        {
            SocketIOManager.instance.sioCom.Instance.Emit("playBet", JsonUtility.ToJson(betData), false);
            canBet = false;
            btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Wait..";
        }

        Debug.Log("BET: canBet = " + canBet + ", betCoinsNum" + betCoinsNum);

        if (betCoinsNum != 0)   
            canBet = false;
        else
            canBet = true;

        EnableTglBets();

        betCoinsNum = 0;
        txt_info.GetComponent<TMP_Text>().text = "";
    }

    private void LateUpdate()
    {
        if (!isMobile)
        {
            if (resolutionX == Screen.width && resolutionY == Screen.height) return;

            if (UseJSLib.GetWindowWidth() < 640)
            {
                MobileTypeUI(UseJSLib.GetWindowWidth());
            }                
            else
            {
                DeskTopModeUI(UseJSLib.GetWindowWidth());
            }

            foreach (Transform historyObj in HistoryContainer.content)
            {
                historyObj.gameObject.GetComponent<RectTransform>().localScale = panelViewScale;
            }
        }
    }

    public void EnableTglBets( )
    {
        GameObject[] tglBets = GameObject.FindGameObjectsWithTag("tglBet");
        if (!canBet)
        {            
            foreach (GameObject item in tglBets)
            {
                item.GetComponent<Toggle>().interactable = false;

            }
        }
        else
        {
            foreach (GameObject item in tglBets)
            {
                item.GetComponent<Toggle>().interactable = true;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (bGoalBoardTransition)
        {
            goalBoardTransitionValue = Mathf.Lerp(goalBoardTransitionValue, 1f, Time.deltaTime * fTransitionSpeed);
            hitplane.GetComponent<Renderer>().material.SetFloat("_Progress", goalBoardTransitionValue);
            //Debug.Log("transValue = " + goalBoardTransitionValue);
        }

        if (img_ViewBG.activeSelf)
        {
            img_TimeBG.SetActive(true);
            txt_Time.SetActive(true);

            //int timeView = (int)(elapseTime - (Time.time - startTime));
            int timeView = SocketIOManager.instance.startTime;
            string stime = string.Format("{0, 3:D}", timeView);
            if (timeView < 10)
                stime = "0" + timeView.ToString();
            else
                stime = timeView.ToString();

            txt_Time.GetComponent<TMP_Text>().text = "00 : " + stime;
        }

        switch (curInfoTabIdx)
        {
            case 1:
                {
                    img_BtnBG.transform.position = new Vector3 (Mathf.Lerp (img_BtnBG.transform.position.x, btn_CurrentBet.transform.position.x, Time.deltaTime * 8f), img_BtnBG.transform.position.y, img_BtnBG.transform.position.z);
                    //if (Mathf.Abs (img_BtnBG.transform.position.x - btn_CurrentBet.transform.position.x) < 0.5f)
                        //curInfoTabIdx = 0;
                    //Debug.Log("dx = " + img_BtnBG.transform.position);
                    break;
                }
            case 2:
                {
                    img_BtnBG.transform.position = new Vector3(Mathf.Lerp(img_BtnBG.transform.position.x, btn_History.transform.position.x, Time.deltaTime * 8f), img_BtnBG.transform.position.y, img_BtnBG.transform.position.z);                    
                    break;                    
                }
            case 3:
                {
                    img_BtnBG.transform.position = new Vector3(Mathf.Lerp(img_BtnBG.transform.position.x, btn_Result.transform.position.x, Time.deltaTime * 8f), img_BtnBG.transform.position.y, img_BtnBG.transform.position.z);                    
                    break;
                }
            case 4:
                {
                    img_BtnBG.transform.position = new Vector3(Mathf.Lerp(img_BtnBG.transform.position.x, btn_Leaders.transform.position.x, Time.deltaTime * 5f), img_BtnBG.transform.position.y, img_BtnBG.transform.position.z);                    
                    break;
                }
        }        
    }

    public void DoShoot()
    {
        //Debug.Log("Doshoot!");

        canBet = false;

        EnableTglBets();

        // if DoShootTrigger is triggered ( on round-result ), client can get Score (GoalKind)
        nGoalKind = SocketIOManager.instance.score;
        if (nGoalKind == 32)
        {
            nGoalKind = SocketIOManager.instance.save;
            bCatch = true;
        }
        else
        {
            bCatch = false;
        }

        // the value of arrayShoot [26] is Empty.- between 25 to 31 (left, middle, right goal post and miss) player direction problem
        if (nGoalKind == 27)
            nGoalKind = 26;

        int playerPosState = nGoalKind % 8;

        Player.transform.rotation = Quaternion.identity;
        if (1 <= playerPosState && playerPosState <= 3)
        {
            Player.transform.position = vPlayerPos[2];
            Player.transform.Rotate(vplayerRot[2]);
        }
        else if (4 <= playerPosState && playerPosState <= 5)
        {
            Player.transform.position = vPlayerPos[1];
            Player.transform.Rotate(vplayerRot[1]);
        }
        else if (6 <= playerPosState || playerPosState == 0)
        {
            Player.transform.position = vPlayerPos[0];
            Player.transform.Rotate(vplayerRot[0]);
        }

        if (nGoalKind < 25)
            goalState = 0; // shoot in
        else if (nGoalKind == 26 || nGoalKind == 28 || nGoalKind == 30)
            goalState = 1; // goal post
        else
            goalState = 2; // goal miss

        img_ViewBG.SetActive(false);
        img_TimeBG.SetActive(false);
        txt_Time.SetActive(false);

        ani_Player.SetTrigger ("Shoot");

        //Debug.Log("Update: DoShoot: nGoalKind = " + nGoalKind);
        //startTime = Time.time;
    }

    public void NewRound() // Don't excute on loading scene
    {
        //Debug.Log("NewRound!");

        img_ViewBG.SetActive(true);

        GameObject.Destroy(Player);// Delete old Player
        
        // add receive goal value to history panel
        GameObject img_History = null;
        if (nGoalKind > 24)
            img_History = GameObject.Instantiate(img_MissHistory, img_MissHistory.transform.position, Quaternion.identity);
        else
        {
            if (isBlue(nGoalKind))
                img_History = GameObject.Instantiate(img_BlueHistory, img_BlueHistory.transform.position, Quaternion.identity);
            else
                img_History = GameObject.Instantiate(img_YellowHistory, img_YellowHistory.transform.position, Quaternion.identity);

            img_History.transform.GetChild(0).GetComponent<TMP_Text>().text = nGoalKind.ToString();
        }
        img_History.transform.SetParent(scrollRect_History.content.transform, false);
        //img_History.GetComponent<RectTransform>().localScale = panelViewScale;

        // display animation ball when goal in      
        if (nGoalKind < 25 && !bCatch)
        {
            GameObject AniBallObj = GameObject.Instantiate(img_BallAni, img_BallAni.transform.position, Quaternion.identity);
            AniBallObj.transform.SetParent(img_ViewBG.transform, false);
            AniBallObj.GetComponent<RectTransform>().anchoredPosition = GetGoalBoardImgRectPos(AniBallObj, nGoalKind);
            GameObject.Destroy(AniBallObj, 2f);
        }
        WinMark();
        XTogglesAnimation();

        // if history count is over, first history is deleded
        if (scrollRect_History.content.transform.childCount > 8)
        {
            GameObject.Destroy(scrollRect_History.content.transform.GetChild(0).gameObject);
        }

        // some reset process;

        // clear bet score value
        for (int i = 0; i < arrayBetToggle.Length; i++)
        {
            arrayBetToggle[i] = 0;
        }
        for (int i = 0; i < seletedTgglarray.Length; i++)
        {
            seletedTgglarray[i] = 0;
        }
        // reset toggles
        for (int i = 0; i < container_tglHitNum.transform.childCount; i++)
        {
            container_tglHitNum.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
        }
        tgl_XGoalPost.GetComponent<Toggle>().isOn = false;
        tgl_XMiss.GetComponent<Toggle>().isOn = false;
        tgl_XTopRow.GetComponent<Toggle>().isOn = false;
        tgl_XMiddleRow.GetComponent<Toggle>().isOn = false;
        tgl_XDownRow.GetComponent<Toggle>().isOn = false;
        tgl_XBlue.GetComponent<Toggle>().isOn = false;
        tgl_XYellow.GetComponent<Toggle>().isOn = false;

        txt_info.GetComponent<TMP_Text>().text = "";
        txt_Value.GetComponent<TMP_Text>().text = "50";

        hitplane.GetComponent<Renderer>().material.SetFloat("_Progress", 0f);
        bGoalBoardTransition = false;
        goalBoardTransitionValue = 0f;

    }

    public void CreatPlayer()
    {
        curPlayerIdx++;
        if (curPlayerIdx > 5)
            curPlayerIdx = 0;
        if (!Player)
            Player = GameObject.Instantiate(PlayerList[curPlayerIdx], PlayerList[curPlayerIdx].transform.position, Quaternion.identity);
        ani_Player = Player.GetComponent<Animator>();
    }

    public IEnumerator ViewScoreBoard()
    {
        yield return new WaitForSeconds(GoalBoardApearTime);

        bGoalBoardTransition = true;

        int nhitGoalKind = GameManager.instance.nGoalKind;
        //hitplane.GetComponent<Renderer>().enabled = true;

        if (GameManager.instance.bCatch)
        {
            hitplane.GetComponent<Renderer>().material = Resources.Load("GoalSave", typeof(Material)) as Material;
        }
        else if (GameManager.instance.nGoalKind < 25)
        {
            hitplane.GetComponent<Renderer>().material = Resources.Load("GoalIn", typeof(Material)) as Material;

            if (GameManager.instance.isBlue(nhitGoalKind))
            {
                instHitMark = GameObject.Instantiate(HitMark_Blue, new Vector3(GameManager.instance.arrayShootBallPos[nhitGoalKind - 1].x,
                    GameManager.instance.arrayShootBallPos[nhitGoalKind - 1].y, hitMarkZPos), HitMark_Yellow.transform.rotation);
            }
            else
            {
                instHitMark = GameObject.Instantiate(HitMark_Yellow, new Vector3(GameManager.instance.arrayShootBallPos[nhitGoalKind - 1].x,
                    GameManager.instance.arrayShootBallPos[nhitGoalKind - 1].y, hitMarkZPos), HitMark_Blue.transform.rotation);
            }
        }
        else
        {
            switch (GameManager.instance.nGoalKind)
            {
                case 25:
                    {
                        hitplane.GetComponent<Renderer>().material = Resources.Load("Missed_Right", typeof(Material)) as Material;
                        break;
                    }
                case 26:
                    {
                        hitplane.GetComponent<Renderer>().material = Resources.Load("GoalPost_Right", typeof(Material)) as Material;
                        break;
                    }
                case 27:
                    {
                        hitplane.GetComponent<Renderer>().material = Resources.Load("GoalIn", typeof(Material)) as Material;
                        break;
                    }
                case 28:
                    {
                        hitplane.GetComponent<Renderer>().material = Resources.Load("GoalPost_Up", typeof(Material)) as Material;
                        break;
                    }
                case 29:
                    {
                        hitplane.GetComponent<Renderer>().material = Resources.Load("Missed_Up", typeof(Material)) as Material;
                        break;
                    }
                case 30:
                    {
                        hitplane.GetComponent<Renderer>().material = Resources.Load("GoalPost_Left", typeof(Material)) as Material;
                        break;
                    }
                case 31:
                    {
                        hitplane.GetComponent<Renderer>().material = Resources.Load("Missed_Left", typeof(Material)) as Material;
                        break;
                    }
            }

        }
        GameManager.Destroy(instHitMark, 2f);
        GameManager.instance.goalNet_1.randomAcceleration = new Vector3(0f, 0f, 0f);
        GameManager.instance.goalNet_2.randomAcceleration = new Vector3(0f, 0f, 0f);
        GameManager.instance.goalNet_3.randomAcceleration = new Vector3(0f, 0f, 0f);
        GameManager.instance.goalNet_4.randomAcceleration = new Vector3(0f, 0f, 0f);
    }

    private void WinMark()
    {
        GameObject img_WinCupObj = null;
        Vector2 vWinCupRectPost = Vector2.zero;
        bool isGoal = true;

        IEnumerable<int> goalPostResult = from betValue in seletedTgglarray
                                          where betValue == 25
                                          select betValue;
        if (goalPostResult.ToArray().Length > 0 && goalState == 1)
        {
            img_WinCupObj = GameObject.Instantiate(img_WinCup, img_WinCup.transform.position, Quaternion.identity);
            img_WinCupObj.transform.SetParent(img_ViewBG.transform, false);
            img_WinCupObj.GetComponent<RectTransform>().anchoredPosition = tgl_XGoalPost.GetComponent<RectTransform>().anchoredPosition;

            isGoal = false;
            GameObject.Destroy(img_WinCupObj, 2f);
        }

        IEnumerable<int> missedResult = from betValue in seletedTgglarray
                                        where betValue == 26
                                        select betValue;
        if (missedResult.ToArray().Length > 0 && (goalState == 2 || bCatch))
        {
            img_WinCupObj = GameObject.Instantiate(img_WinCup, img_WinCup.transform.position, Quaternion.identity);
            img_WinCupObj.transform.SetParent(img_ViewBG.transform, false);
            img_WinCupObj.GetComponent<RectTransform>().anchoredPosition = tgl_XMiss.GetComponent<RectTransform>().anchoredPosition;
            isGoal = false;
            GameObject.Destroy(img_WinCupObj, 2f);
        }

        for (int i = 0; i < seletedTgglarray.Length; i++)
        {
            int nCurValInBets = seletedTgglarray[i];
            int nGoal = nGoalKind;


            if (nCurValInBets < 25 && nCurValInBets == nGoal)
            {
                img_WinCupObj = GameObject.Instantiate(img_WinCup, img_WinCup.transform.position, Quaternion.identity);

                img_WinCupObj.GetComponent<RectTransform>().anchoredPosition = GetGoalBoardImgRectPos(img_WinCupObj, nGoal);
                img_WinCupObj.GetComponent<RectTransform>().anchoredPosition += new Vector2(30f, -30f); // offset
            }

            if (!isGoal || nGoal > 24) return; // if GoalPost or Miss then there is not any goalin 

            if (nCurValInBets == 27 && (int)((nGoalKind - 1) / 8) == 0) // Top Row
            {
                img_WinCupObj = GameObject.Instantiate(img_WinCup, img_WinCup.transform.position, Quaternion.identity);
                img_WinCupObj.GetComponent<RectTransform>().anchoredPosition = tgl_XTopRow.GetComponent<RectTransform>().anchoredPosition;
            }
            else if (nCurValInBets == 28 && (int)((nGoalKind - 1) / 8) == 1) // Middle Row
            {
                img_WinCupObj = GameObject.Instantiate(img_WinCup, img_WinCup.transform.position, Quaternion.identity);
                img_WinCupObj.GetComponent<RectTransform>().anchoredPosition = tgl_XMiddleRow.GetComponent<RectTransform>().anchoredPosition;
            }
            else if (nCurValInBets == 29 && (int)((nGoalKind - 1) / 8) == 2) // Down Row
            {
                img_WinCupObj = GameObject.Instantiate(img_WinCup, img_WinCup.transform.position, Quaternion.identity);
                img_WinCupObj.GetComponent<RectTransform>().anchoredPosition = tgl_XDownRow.GetComponent<RectTransform>().anchoredPosition;
            }

            if (nCurValInBets == 30 && isBlue(nGoal)) // Blue
            {
                img_WinCupObj = GameObject.Instantiate(img_WinCup, img_WinCup.transform.position, Quaternion.identity);

                img_WinCupObj.GetComponent<RectTransform>().anchoredPosition = tgl_XBlue.GetComponent<RectTransform>().anchoredPosition;
            }
            else if (nCurValInBets == 31 && !isBlue(nGoal)) // Yellow
            {
                img_WinCupObj = GameObject.Instantiate(img_WinCup, img_WinCup.transform.position, Quaternion.identity);

                img_WinCupObj.GetComponent<RectTransform>().anchoredPosition = tgl_XYellow.GetComponent<RectTransform>().anchoredPosition;
            }

            if (img_WinCupObj)
            {
                //img_WinCupObj.transform.parent = img_ViewBG.transform;
                img_WinCupObj.transform.SetParent(img_ViewBG.transform, false);
                GameObject.Destroy(img_WinCupObj, 2f);
                //if (objWinImage == null)
                {
                    //objWinImage = GameObject.Instantiate(btn_BetWin, btn_BetWin.transform.position, Quaternion.identity);
                    //objWinImage.transform.parent = panel_Info.transform;
                    //float yPos = 75f * panelInfoScale.y; // 75 origianl (in 1920) pos
                    //objWinImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, yPos);
                    //objWinImage.GetComponent<RectTransform>().localScale = panelInfoScale;
                    //objWinImage.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "WIN  " + winAmount + " !"; 
                    //GameObject.Destroy(objWinImage, 2f);
                    if (winAmount > 50)
                    {
                        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "WIN  " + winAmount + " !";
                    }
                    else if (winAmount == 0)
                        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Try again !";
                    else if (winAmount < 0)
                        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Loose !";
                    StartCoroutine(RenameButton());
                }
            }
        }

    }

    IEnumerator RenameButton()
    {
        yield return new WaitForSeconds(2f);
        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = ConstVars.btnBetName;
    }

    private Vector2 GetGoalBoardImgRectPos(GameObject obj, int idx)
    {
        float dX = img_GoalBoard.GetComponent<RectTransform>().sizeDelta.x / 8f;
        float dY = img_GoalBoard.GetComponent<RectTransform>().sizeDelta.y / 3f;
        float firstPosX = img_GoalBoard.GetComponent<RectTransform>().anchoredPosition.x;
        float firstPosY = img_GoalBoard.GetComponent<RectTransform>().anchoredPosition.y;

        int colNum = (idx - 1) % 8;
        int rowNum = (int)((idx - 1) / 8);
        Vector2 ImgRectPos = new Vector2(firstPosX + dX * colNum, firstPosY - dY * rowNum);

        ImgRectPos += new Vector2(obj.GetComponent<RectTransform>().sizeDelta.x / 2, -obj.GetComponent<RectTransform>().sizeDelta.x / 2);
        /*Debug.Log("width = " + img_GoalBoard.GetComponent<RectTransform>().sizeDelta.x + ", height = " +
            img_GoalBoard.GetComponent<RectTransform>().sizeDelta.y +
            ", posx = " + img_GoalBoard.GetComponent<RectTransform>().anchoredPosition.x + 
            ", posy = " + img_GoalBoard.GetComponent<RectTransform>().anchoredPosition.y);
        */
        return ImgRectPos;
    }

    private void XTogglesAnimation()
    {
        if (bCatch || goalState == 2)
        {
            tgl_XMiss.GetComponent<Animator>().SetTrigger("AniToggle");
        }
        else if (goalState == 1)
        {
            tgl_XGoalPost.GetComponent<Animator>().SetTrigger("AniToggle");
        }

        if (bCatch || nGoalKind > 24) return;

        if ((int)((nGoalKind - 1) / 8) == 0)
        {
            tgl_XTopRow.GetComponent<Animator>().SetTrigger("AniToggle");
        }
        else if ((int)((nGoalKind - 1) / 8) == 1)
        {
            tgl_XMiddleRow.GetComponent<Animator>().SetTrigger("AniToggle");
        }
        else if ((int)((nGoalKind - 1) / 8) == 2)
        {
            tgl_XDownRow.GetComponent<Animator>().SetTrigger("AniToggle");
        }

        if (isBlue(nGoalKind))
            tgl_XBlue.GetComponent<Animator>().SetTrigger("AniToggle");
        else
            tgl_XYellow.GetComponent<Animator>().SetTrigger("AniToggle");
    }

    public bool isBlue(int idx)
    {
        int rownum = (int)(idx - 1) / 8;
        int colnum = (idx - 1) % 8;
        switch (rownum)
        {
            case 0:
                {
                    if (colnum % 2 == 0)
                        return true;
                    break;
                }
            case 1:
                {
                    if (colnum % 2 == 1)
                        return true;
                    break;
                }
            case 2:
                {
                    if (colnum % 2 == 0)
                        return true;
                    break;
                }
        }
        return false;
    }

    public void IncreaseValue()
    {
        if (SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text == "")
            return;

        string strVal = txt_Value.GetComponent<TMP_Text>().text;
        int FUN = int.Parse(SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text);
        int curBetAmount = int.Parse(strVal) + 50;
        //Debug.Log("FUN = " + FUN + ", curBetMount" + curBetAmount);
        if (FUN < curBetAmount)
            return;
        txt_Value.GetComponent<TMP_Text>().text = (int.Parse(strVal) + 50).ToString ();
        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET : " + betCoinsNum * int.Parse(txt_Value.GetComponent<TMP_Text>().text);
    }
    public void DecreaseValue()
    {

        string strVal = txt_Value.GetComponent<TMP_Text>().text;
        if (int.Parse(strVal) == 50)
            return;
        txt_Value.GetComponent<TMP_Text>().text = (int.Parse(strVal) - 50).ToString();
        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET : " + betCoinsNum * int.Parse(txt_Value.GetComponent<TMP_Text>().text);
    }

    public void DoubleValue()
    {
        if (SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text == "")
            return;

        string strVal = txt_Value.GetComponent<TMP_Text>().text;
        int FUN = int.Parse(SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text);
        int curBetAmount = int.Parse(strVal) * 2;
        //Debug.Log("FUN = " + FUN + ", curBetMount" + curBetAmount);
        if ( FUN < curBetAmount)
            return;
        txt_Value.GetComponent<TMP_Text>().text = (int.Parse(strVal) * 2).ToString();
        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET : " + betCoinsNum * int.Parse(txt_Value.GetComponent<TMP_Text>().text);
    }

    public void Add50()
    {
        if (SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text == "")
            return;

        string strVal = txt_Value.GetComponent<TMP_Text>().text;
        int FUN = int.Parse(SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text);
        int curBetAmount = int.Parse(strVal) + 50;
        //Debug.Log("FUN = " + FUN + ", curBetMount" + curBetAmount);
        if (FUN < curBetAmount)
            return;
        txt_Value.GetComponent<TMP_Text>().text = (int.Parse(strVal) + 50).ToString();
        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET : " + betCoinsNum * int.Parse(txt_Value.GetComponent<TMP_Text>().text);
    }

    public void Add250()
    {
        if (SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text == "")
            return;

        string strVal = txt_Value.GetComponent<TMP_Text>().text;
        int FUN = int.Parse(SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text);
        int curBetAmount = int.Parse(strVal) + 250;
        //Debug.Log("FUN = " + FUN + ", curBetMount" + curBetAmount);
        if (FUN < curBetAmount)
            return;
        txt_Value.GetComponent<TMP_Text>().text = (int.Parse(strVal) + 250).ToString();
        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET : " + betCoinsNum * int.Parse(txt_Value.GetComponent<TMP_Text>().text);
    }

    public void Add500()
    {
        if (SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text == "")
            return;

        string strVal = txt_Value.GetComponent<TMP_Text>().text;
        int FUN = int.Parse(SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text);
        int curBetAmount = int.Parse(strVal) + 500;
        //Debug.Log("FUN = " + FUN + ", curBetMount" + curBetAmount);
        if (FUN < curBetAmount)
            return;
        txt_Value.GetComponent<TMP_Text>().text = (int.Parse(strVal) + 500).ToString();
        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET : " + betCoinsNum * int.Parse(txt_Value.GetComponent<TMP_Text>().text);
    }

    public void AllIn()
    {
        if (SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text == "")
            return;

        string strVal = txt_Value.GetComponent<TMP_Text>().text;
        int FUN = int.Parse(SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text);
        int curBetAmount = FUN;
        //Debug.Log("FUN = " + FUN + ", curBetMount" + curBetAmount);
        if (FUN < curBetAmount)
            return;
        txt_Value.GetComponent<TMP_Text>().text = FUN.ToString();
        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET : " + betCoinsNum * int.Parse(txt_Value.GetComponent<TMP_Text>().text);
    }

    private void SetActivePanel(byte idx)
    {
        switch (idx)
        {
            case 1:
                {
                    Panel_Btn_Leaders.SetActive(false);
                    Panel_Btn_Histroy.SetActive(false);
                    Panel_Btn_Current.SetActive(true);
                    break;
                }
            case 2:
                {
                    Panel_Btn_Leaders.SetActive(false);
                    Panel_Btn_Histroy.SetActive(true);
                    Panel_Btn_Current.SetActive(false);
                    break;
                }
            case 3:
                {
                    Panel_Btn_Leaders.SetActive(false);
                    Panel_Btn_Histroy.SetActive(false);
                    Panel_Btn_Current.SetActive(false);
                    break;
                }
            case 4:
                {
                    Panel_Btn_Leaders.SetActive(true);
                    Panel_Btn_Histroy.SetActive(false);
                    Panel_Btn_Current.SetActive(false);
                    break;
                }
        }
    }   
    
    public void OnCurrentBet()
    {
        curInfoTabIdx = 1;
        SetActivePanel(1);
        SocketIOManager.instance.ViewInfoPanel();
    }

    public void OnLeaders()
    {
        curInfoTabIdx = 4;
        SetActivePanel(4);
        SocketIOManager.instance.ViewInfoPanel();
    }

    public void OnHistory()
    {
        curInfoTabIdx = 2;
        SetActivePanel(2);
        SocketIOManager.instance.ViewInfoPanel();
        
        /*
        if (btnGroupMoved)
        {
            btnGroupMoved = false;
            float movX = Panel_AnalyTab.GetComponent<RectTransform>().rect.width * Panel_Info.GetComponent<RectTransform>().localScale.x / 3f;
            pnl_BtnGropMove.transform.Translate(movX, 0f, 0f); //new Vector3 (pnl_BtnGropMove.transform.position 
        }*/
    }

    /*
    public void OnResult()
    {
        curInfoTabIdx = 3;
        SetActivePanel(3);
        if (!btnGroupMoved )
        {
            btnGroupMoved = true;
            float movX = Panel_AnalyTab.GetComponent<RectTransform>().rect.width * Panel_Info.GetComponent<RectTransform>().localScale.x / 3f;
            pnl_BtnGropMove.transform.Translate( -movX, 0f, 0f); //new Vector3 (pnl_BtnGropMove.transform.position 
        }
        SocketIOManager.instance.ViewInfoPanel();
    }
    */

    //index is goalkind
    private void SetArrayHitPos()
    {
        float dX = (3.62f * 2f) / 16f;
        float dY = 2.44f / 6f;
        float firstPosX = -3.62f + dX;
        float firstPosY = 2.44f - dY;

        arrayShootBallPos[0] = new Vector2(firstPosX, firstPosY);

        for (int i = 0; i < 24; i ++)
        {
            int colNum = i % 8;
            int rowNum = (int)(i / 8);
            //Debug.Log("rowNum = " + rowNum + ", colNum = " + colNum);
            arrayShootBallPos[i] = new Vector2(firstPosX + 2 * dX * colNum, firstPosY - 2 * dY * rowNum + 0.04f);
            //Debug.Log("Array [" + i + "]" + arrayHit[i]);
        }
        arrayShootBallPos[25] = new Vector2(-3.722f, 2.0f); // Goal Post, right (to player)
        arrayShootBallPos[27] = new Vector2(2f, 2.564f); // Goal Post, Up
        arrayShootBallPos[29] = new Vector2(3.722f, 1.8f);//Goal Post, Left

        arrayShootBallPos[24] = new Vector2(-4f, 1.8f); // Miss , right
        arrayShootBallPos[28] = new Vector2(-1f, 3f); // Miss, Up
        arrayShootBallPos[30] = new Vector2(4f, 1.8f); // Miss left
    }

    private bool NotYetBet()
    {
        if (!canBet)
        {
            txt_info.GetComponent<TMP_Text>().text = ConstVars.errorNotyetBET;
            StartCoroutine(ClearInfoText(2f));
        }
        return canBet;
    }

    IEnumerator ClearInfoText(float fInfoTextDelay)
    {
        yield return new WaitForSeconds(fInfoTextDelay);
        txt_info.GetComponent<TMP_Text>().text = "";
    }

    public void OnToggleScores(GameObject tgl)
    {
        //if (SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text == "")
        {
            //tgl.GetComponent<Toggle>().isOn = false;
            //return;
        }
                           
        if (!NotYetBet())
        {
            //bool curTgle = tgl.GetComponent<Toggle>().isOn;
            //tgl.GetComponent<Toggle>().isOn = !curTgle;
            return;
        }
       
            
        //get gameobject name index
        int idx = int.Parse(tgl.name.Substring(tgl.name.Length - 2));
                
        if (tgl.GetComponent<Toggle>().isOn)
        {
            //txt_info.GetComponent<TMP_Text>().text = ConstVars.btnBetName;
            betCoinsNum += 1;
            int betAmountTemp = betCoinsNum * int.Parse(txt_Value.GetComponent<TMP_Text>().text);

            int FUN = int.Parse(SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text);
            int curBetAmount = betAmountTemp;
            //Debug.Log("FUN = " + FUN + ", curBetMount" + curBetAmount);
            if (FUN < curBetAmount)
            {
                txt_info.GetComponent<TMP_Text>().text = "Not Enough FUN !";
                StartCoroutine(ClearInfoText(2f));
                //betCoinsNum -= 1; // don't this! because toggle is false,  it decrease at next frame by onchange trigger. 
                tgl.GetComponent<Toggle>().isOn = false;
                return;
            }
            
            arrayBetToggle[idx - 1] = idx;            
        }            
        else
        {
            arrayBetToggle[idx - 1] = 0;
                betCoinsNum -= 1;
        }

        //btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Not Enough";
        btn_BetWin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "BET : " + betCoinsNum * int.Parse(txt_Value.GetComponent <TMP_Text>().text );
    }
}

[Serializable]
public class BetInfos
{
    public string roundId;
    public string name;
    public string betAmount;
    public string combination;
    public string winFun;
}



public static  class ConstVars 
{
    public const byte constTimeCount = 15;
    public const string errorNotyetBET = "";//Please bet while the countdown !
    public const float designeWidth = 1920f;
    public const float designeHeight = 1080f;
    public const float phoneViewPanelPosY = 0.5f; 
    public const float phoneTopPanelHeight = 0.12f;
    public const string btnBetName = "BET";

    //public const string errorEndBET = "Bet time is ended";
}