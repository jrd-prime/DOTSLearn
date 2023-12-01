using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    public static CameraSingleton Instance;

    private CameraSingleton()
    {
    }

    public float Speed => Speed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}