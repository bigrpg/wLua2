using UnityEngine;
using System;
using System.IO;

public class StreamingAssetsHelper {

	public static byte[] ReadAllBytes(String fullFilePath)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
			WWW www = new WWW(fullFilePath); 
			while(!www.isDone){}
            {
                if(string.IsNullOrEmpty(www.error))
			        return www.bytes;
                return null;
            }
#else
		return File.ReadAllBytes(fullFilePath);
#endif
	}
}
