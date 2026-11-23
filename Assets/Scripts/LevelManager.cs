using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private float victoryDelay = 2.5f;
    [SerializeField] private string menuSceneName = "LevelMenu";

    [SerializeField] private Image progressFill;
    [SerializeField] private Text waveText;

    [SerializeField] private Image pauseButtonImage;
    [SerializeField] private Sprite playingSprite;
    [SerializeField] private Sprite pausedSprite;

    [SerializeField] private Image waveBanner;
    [SerializeField] private Sprite waveSprite;
    [SerializeField] private Sprite lastWaveSprite;
    [SerializeField] private float bannerShowTime = 2f;
    [SerializeField] private float bannerFade = 0.4f;
    [SerializeField] private float gameSpeed = 1f;

    [SerializeField] private Text rewardText;

    private bool waitingVictory;
    private int shownWave;
    private Coroutine bannerRoutine;


    void Start()
    {
        Time.timeScale = gameSpeed;
        SetBannerAlpha(0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();

        if (spawner.Total > 0)
        {
            float killed = spawner.spawned - Enemy.AliveCount;
            progressFill.fillAmount = Mathf.Clamp01(killed / spawner.Total);
            waveText.text = "Волна " + spawner.currentWave + "/" + spawner.TotalWaves;
        }

        if (spawner.spawned > 0 && spawner.currentWave != shownWave)
        {
            shownWave = spawner.currentWave;
            bool isLast = shownWave == spawner.TotalWaves;

            if (bannerRoutine != null) StopCoroutine(bannerRoutine);
            bannerRoutine = StartCoroutine(BannerRoutine(isLast));
        }

        if (waitingVictory) return;
        if (gameOverPanel.activeSelf || victoryPanel.activeSelf) return;

        if (spawner.allSpawned && Enemy.AliveCount <= 0)
        {
            waitingVictory = true;
            StartCoroutine(ShowVictory());
        }
    }

    void SetBannerAlpha(float a)
    {
        Color c = waveBanner.color;
        c.a = a;
        waveBanner.color = c;
    }

    IEnumerator BannerRoutine(bool isLast)
    {
        waveBanner.sprite = isLast ? lastWaveSprite : waveSprite;

        float t = 0f;
        while (t < bannerFade)
        {
            t += Time.unscaledDeltaTime;
            SetBannerAlpha(t / bannerFade);
            yield return null;
        }
        SetBannerAlpha(1f);

        yield return new WaitForSecondsRealtime(bannerShowTime);

        t = 0f;
        while (t < bannerFade)
        {
            t += Time.unscaledDeltaTime;
            SetBannerAlpha(1f - t / bannerFade);
            yield return null;
        }
        SetBannerAlpha(0f);
    }

    IEnumerator ShowVictory()
    {
        yield return new WaitForSeconds(victoryDelay);

        while (pausePanel.activeSelf) yield return null;

        if (gameOverPanel.activeSelf) yield break;

        ApiRequest.CompleteLevel(OnLevelCompleted, levelNumber, 0);
        AudioManager.Instance.Victory();
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnLevelCompleted(User.CompleteLevelResponse response)
    {
        if (response.status) Game.Instance.coins = response.totalCoins;
        rewardText.text = "+" + response.coinsAwarded;
    }

    public void Lose()
    {
        if (victoryPanel.activeSelf || gameOverPanel.activeSelf) return;
        StopAllCoroutines();
        SetBannerAlpha(0f);
        pausePanel.SetActive(false);
        pauseButtonImage.sprite = playingSprite;
        AudioManager.Instance.Defeat();
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        if (gameOverPanel.activeSelf || victoryPanel.activeSelf) return;

        if (pausePanel.activeSelf) Continue();
        else Pause();
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        pauseButtonImage.sprite = pausedSprite;
    }

    public void Continue()
    {
        pausePanel.SetActive(false);
        Time.timeScale = gameSpeed;
        pauseButtonImage.sprite = playingSprite;
    }

    public void Restart()
    {
        AudioManager.Instance.ResumeMusic();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMenu()
    {
        AudioManager.Instance.ResumeMusic();
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}