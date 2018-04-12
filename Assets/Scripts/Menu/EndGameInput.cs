using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameInput : MonoBehaviour {
    [SerializeField] private Button[] buttonArray;
    [SerializeField] private float delayBetweenVerticalInputs = 0.2f;
    private ControllerManager controllerManager;
    private X360_controller controller;
    private ManagerInGame gameManager;
    private int buttonIndex;
    private float timer = 0.0f;
    private bool enableInput = false;
    private static EndGameInput instance;
    public bool activeMenu = true;
    // Use this for initialization

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start () {
        enableInput = false;
        gameManager = ManagerInGame.GetInstance();
        controllerManager = ControllerManager.Instance;
        buttonIndex = 0;
        StartCoroutine(GetWinnerController());
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(activeMenu);
		if(controller != null && enableInput && activeMenu)
        {
            if(Time.unscaledTime - timer > delayBetweenVerticalInputs)
            {
                if (controller.GetStick_L().Y < 0)
                {
                    IncrementButtonIndex();
                }
                if (controller.GetStick_L().Y > 0)
                {
                    DecrementButtonIndex();
                }
            }
            
            buttonArray[buttonIndex].Select();

            if (controller.GetButtonDown("A"))
            {
                switch (buttonIndex)
                {
                    case 0:
                        gameManager.Reload();
                        break;
                    case 1:
                        gameManager.LoadMainMenu();
                        break;
                    case 2:
                        gameManager.LoadLobby();
                        break;
                }
            }
        }
	}

    void IncrementButtonIndex()
    {
        buttonIndex = (buttonIndex + 1) % buttonArray.Length;
        timer = Time.unscaledTime;
    }

    void DecrementButtonIndex()
    {
        buttonIndex--;
        if (buttonIndex < 0)
        {
            buttonIndex = buttonArray.Length - 1;
        }
        timer = Time.unscaledTime;
    }
    private IEnumerator GetWinnerController()
    {
        while(controller == null && gameManager.GetWinner() == null)
        {
            yield return null;
        }
        controller = controllerManager.GetController(gameManager.GetWinner().playerID + 1);
    }

    public void EnableInput()
    {
        enableInput = true;
    }
    public static EndGameInput GetInstance()
    {
        if(instance == null)
        {
            Debug.LogError("No instance of [EndGame]");
            return null;
        }
        return instance;
    }
}
