using UnityEngine;

internal static class BepInExHelpers
{
    public static ObjectDB FindObjectDB()
    {
        GameObject gameMainObj = GameObject.Find("_GameMain");
        if (gameMainObj != null)
        {
            ObjectDB objectDB = gameMainObj.GetComponent<ObjectDB>();
            if (objectDB != null)
            {
                return objectDB;
            }
        }

        return null;
    }
}
