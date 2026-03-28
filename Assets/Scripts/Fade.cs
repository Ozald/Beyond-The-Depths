using UnityEngine;

public class Fade : MonoBehaviour
{
    public static Fade instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
