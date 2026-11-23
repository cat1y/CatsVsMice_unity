using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static User;

[System.Serializable]
public class ShopItem
{
    public int id;
    public Button buyButton;
    public Text priceText;
    public Text limitText;
}

public class MenuScript : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject notEnoughPanel;

    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;

    [SerializeField] private Text balanceText;
    [SerializeField] private ShopItem[] items;

    void Start()
    {
        menuPanel.SetActive(true);
        shopPanel.SetActive(false);
        notEnoughPanel.SetActive(false);

        foreach (ShopItem item in items)
        {
            ShopItem current = item;
            current.buyButton.onClick.AddListener(() => Buy(current));
        }

        RefreshLevels();
        RefreshShop();

        ApiRequest.GetProfile(OnProfile);
        ApiRequest.GetLevels(OnLevels);
        ApiRequest.GetShopCats(OnCats);
    }

    void OnProfile(ProfileResponse response)
    {
        if (!response.status) return;
        Game.Instance.coins = response.coins;
        RefreshShop();
    }

    void OnLevels(LevelsResponse response)
    {
        if (!response.status) return;
        PlayerData.Levels = response.items;
        RefreshLevels();
    }

    void OnCats(CatsResponse response)
    {
        if (!response.status) return;
        PlayerData.Cats = response.items;
        RefreshShop();
    }

    void RefreshLevels()
    {
        SetButtonState(level1Button, PlayerData.IsLevelUnlocked(1));
        SetButtonState(level2Button, PlayerData.IsLevelUnlocked(2));
        SetButtonState(level3Button, PlayerData.IsLevelUnlocked(3));
    }

    void RefreshShop()
    {
        balanceText.text = PlayerData.GetBalance().ToString();

        foreach (ShopItem item in items)
        {
            CatData cat = PlayerData.GetCat(item.id);
            if (cat == null) continue;

            item.buyButton.gameObject.SetActive(!cat.isOwned);

            if (item.priceText != null) item.priceText.text = cat.price.ToString();
            if (item.limitText != null) item.limitText.text = cat.limit.ToString();
        }
    }

    void Buy(ShopItem item)
    {
        CatData cat = PlayerData.GetCat(item.id);
        if (cat == null || cat.isOwned) return;

        if (PlayerData.GetBalance() < cat.price)
        {
            notEnoughPanel.SetActive(true);
            return;
        }

        ApiRequest.BuyCat(OnBought, item.id);
    }

    void OnBought(BuyCatResponse response)
    {
        if (!response.status)
        {
            notEnoughPanel.SetActive(true);
            return;
        }
        AudioManager.Instance.Buy();

        Game.Instance.coins = response.coinsLeft;
        ApiRequest.GetShopCats(OnCats);
    }

    void SetButtonState(Button button, bool active)
    {
        button.interactable = active;

        ColorBlock colors = button.colors;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.6f);
        button.colors = colors;
    }

    public void OpenShop()
    {
        menuPanel.SetActive(false);
        shopPanel.SetActive(true);
        RefreshShop();
    }

    public void CloseShop()
    {
        notEnoughPanel.SetActive(false);
        shopPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void CloseNotEnough()
    {
        notEnoughPanel.SetActive(false);
    }

    public void LoadLevel1() { LoadLevel("Level1"); }
    public void LoadLevel2() { LoadLevel("Level2"); }
    public void LoadLevel3() { LoadLevel("Level3"); }

    public void BackToLogin()
    {
        Game.Instance.token = "";
        LoadLevel("SampleScene");
    }

    void LoadLevel(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}