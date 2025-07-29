using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static UIManager Instance;
    public static UIManager GetUIManager()
    {
        return Instance;
    }

    [SerializeField] RawImage mapImage;
    [SerializeField] Text coinText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // start with the UI turned on
        mapImage.gameObject.SetActive(true);
        coinText.gameObject.SetActive(true);
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Tab))
        {
            mapImage.gameObject.SetActive(!mapImage.gameObject.activeSelf);
        }*/
    }

    public void UpdateCoinCount()
    {
        coinText.text = "Coins: " + PlayerMovement.GetPlayer().GetNumCoins().ToString();
    }
}
