using UnityEngine;
using static PixelCrushers.AnimatorSaver;

public class GameManager : MonoBehaviour
{
    
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

            if (canvas != null)
            {
                DontDestroyOnLoad(canvas);
            }

            if (audioManager != null)
            {
                DontDestroyOnLoad(audioManager);
            }

           

        }
        else
        {
            // Nếu đã có instance, xóa đối tượng mới tạo
            Destroy(gameObject);
        }
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
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas otherCanvas in allCanvases)
        {
            if (otherCanvas != canvas && otherCanvas.tag != "Canvas")
            {
                Destroy(otherCanvas.gameObject); 
            }
        }
    }

}
