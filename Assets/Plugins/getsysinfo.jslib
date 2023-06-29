mergeInto(LibraryManager.library, {
  isAndroid : function() {
    return Module.SystemInfo.os === "Android";
  },
  getWindowWidth : function() {
    return window.innerWidth;
  },
  getWindowHeight : function() {
    return window.innerHeight;
  },
  isIos : function() {
    return Module.SystemInfo.os === "iPhoneOS" || Module.SystemInfo.os === "iPadOS"; // || Module.SystemInfo.os === "macOS"
  },
  
  getOperationSystemFamilyName: function() {
    var os = Module.SystemInfo.os;
    var bufferSize = lengthBytesUTF8(os) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(os, buffer, bufferSize);
    return buffer;
  }, 

  freeBuffer: function(ptr) {
    _free(ptr);
  },
  gameready: function(){
     window.dispatchReactUnityEvent(
      "GameReady",
      Pointer_stringify("ready")
    );
  },
  getSearchParams: function () {
        var returnStr = window.location.search;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
  }
});