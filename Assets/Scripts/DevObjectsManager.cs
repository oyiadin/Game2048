using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevObjectsManager : MonoBehaviour
{
    void Start()
    {
        GameObject.Find("BleedContainer").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        var editorOnlys = GameObject.FindGameObjectsWithTag("EditorOnly");
        foreach (GameObject one in editorOnlys)
        {
            one.SetActive(false);
        }
    }
}
