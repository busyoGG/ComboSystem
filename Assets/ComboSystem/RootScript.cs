using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ComboManager.Instance().Init();
        ComboManager.Instance().OnIdle(() =>
        {
            Debug.Log("ткпп©уопв╢л╛");
        });
    }
}
