using System.Runtime.InteropServices;
using UnityEngine;

public class UseJSLib: MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern bool isAndroid();
#else
    [DllImport("getsysinfo.jslib")]
    private static extern string isAndroid();
#endif

    public static bool IsAndroid()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return isAndroid();
#else
        return false;
#endif
    }


#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern bool isIos();
#else
    [DllImport("getsysinfo.jslib")]
    private static extern bool isIos();
#endif

    public static bool IsIos()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return isIos();
#else
        return false;
#endif
    }

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string getOperationSystemFamilyName();
#else
    [DllImport("getsysinfo.jslib")]
    private static extern string getOperationSystemFamilyName();
#endif

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string getSearchParams();
#else
    [DllImport("getsysinfo.jslib")]
    private static extern string getSearchParams();
#endif

    public static string GetSearchParams()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_WEBGL
    return getSearchParams();
#else
    return getSearchParams();
#endif
    }
    /*
    public static string GetOperationSystemFamilyName()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        var os = getOperationSystemFamilyName();
        var copy = String.Copy(os);
        freeBuffer(os);
        return copy;
        
#else
        return SystemInfo.operatingSystem;
#endif
    }
    */

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void freeBuffer(string ptr);
#else
    [DllImport("getsysinfo.jslib")]
     private static extern void freeBuffer(string ptr);
#endif


#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern int getWindowWidth();
#else
    [DllImport("getsysinfo.jslib")]
    private static extern int getWindowWidth();
#endif 

    public static int GetWindowWidth()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return getWindowWidth();
#else
        return Screen.width;
#endif
    }    

}
