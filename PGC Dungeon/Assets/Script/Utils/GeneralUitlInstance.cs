using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralUitlInstance : MonoBehaviour
{

    public static GeneralUitlInstance instance;

    [Header("Alert message spawn vars")]

    [SerializeField] GameObject UIErrorPrefab;
    [SerializeField] Canvas SceneAlertCanvas;

    private GameObject previousErrorPrefabRef;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    #region spawn alert message reagion

    public void SpawnMessagePrefab(string MessDesc, bool type)
    {

        if (previousErrorPrefabRef != null)     // its still there
        {
            Destroy(previousErrorPrefabRef);
        }


        GameObject newRef = Instantiate(UIErrorPrefab, SceneAlertCanvas.transform);

        if (type)      // positve is green
        {
            newRef.GetComponent<RawImage>().color = new Color32(0, 200, 0, 100);
            Debug.Log($"calling for green");
        }
        else    // negative is red
        {
            newRef.GetComponent<RawImage>().color = new Color32(200, 0, 0, 100);
        }


        newRef.GetComponentInChildren<TMP_Text>().text = MessDesc;


        previousErrorPrefabRef = newRef;
    }

    public void DeleteError()
    {
        Destroy(previousErrorPrefabRef);
    }

    #endregion
}
