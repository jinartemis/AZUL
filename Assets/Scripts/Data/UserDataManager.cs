using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }
}
