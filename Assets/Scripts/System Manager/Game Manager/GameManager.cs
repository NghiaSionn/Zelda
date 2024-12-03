using UnityEngine;
using static PixelCrushers.AnimatorSaver;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public Canvas canvas;
    public AudioManager audioManager;



    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager instance is null!");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(player);
        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(audioManager);
    }

    private void OnEnable()
    {
        // Đăng ký sự kiện scene loaded
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Hủy đăng ký sự kiện khi script bị vô hiệu hóa
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Tìm tất cả các Canvas trong scene hiện tại
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();

        foreach (Canvas otherCanvas in allCanvases)
        {
            // Nếu Canvas không phải của GameManager (theo Tag), thì xóa nó
            if (otherCanvas != canvas && otherCanvas.tag != "Canvas")
            {
                Destroy(otherCanvas.gameObject); // Xóa Canvas thừa
            }
        }
    }
}
