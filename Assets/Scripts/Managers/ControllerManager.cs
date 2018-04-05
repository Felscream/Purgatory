//source https://lcmccauley.wordpress.com/2015/12/01/x360-input-tutorial-unity-p4/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour {

    public int GamepadCount = 4; // Number of gamepads to support

    private List<X360_controller> controllers;     // Holds gamepad instances
    private static ControllerManager instance; // Singleton instance
    public static ControllerManager Instance
    {
        get{
            if (instance == null)
            {
                Debug.Log("No instance of " + typeof(ControllerManager));
                return null;
            }
            return instance;
        }
        
    }
    void Awake()
    {
        // Found a duplicate instance of this class, destroy it!
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            GamepadCount = Mathf.Clamp(GamepadCount, 1, 4);

            controllers = new List<X360_controller>();

            // Create specified number of gamepad instances
            for (int i = 0; i < GamepadCount; ++i)
            {
                controllers.Add(new X360_controller(i + 1));
            }
                
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        for (int i = 0; i < controllers.Count; ++i)
            controllers[i].Update();
    }
    public void Refresh()
    {
        for (int i = 0; i < controllers.Count; ++i)
            controllers[i].Refresh();
    }
    public X360_controller GetController(int index)
    {
        for (int i = 0; i < controllers.Count;)
        {
            // Indexes match, return this gamepad
            if (controllers[i].Index == (index - 1))
                return controllers[i];
            else
                ++i;
        }

        Debug.LogError("[ControllerManager]: " + index + " is not a valid controller index!");

        return null;
    }
    public int ConnectedTotal()
    {
        int total = 0;

        for (int i = 0; i < controllers.Count; ++i)
        {
            if (controllers[i].IsConnected)
                total++;
        }

        return total;
    }
    public bool GetButtonAny(string button)
    {
        for (int i = 0; i < controllers.Count; ++i)
        {
            // Gamepad meets both conditions
            if (controllers[i].IsConnected && controllers[i].GetButton(button))
                return true;
        }
        return false;
    }
    public bool GetButtonDownAny(string button)
    {
        for (int i = 0; i < controllers.Count; ++i)
        {
            // Gamepad meets both conditions
            if (controllers[i].IsConnected && controllers[i].GetButtonDown(button))
                return true;
        }

        return false;
    }
}
