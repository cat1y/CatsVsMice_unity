using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static User;

public class Loginscript : MonoBehaviour
{
    [SerializeField] private GameObject login;
    [SerializeField] private GameObject loginName;
    [SerializeField] private GameObject loginPassword;

    [SerializeField] private GameObject register;
    [SerializeField] private GameObject regName;
    [SerializeField] private GameObject regPassword;

    [SerializeField] private Text errorText;

    void Start()
    {
        login.SetActive(true);
        register.SetActive(false);
    }

    public void OnLoginBtn()
    {
        string name = loginName.GetComponent<InputField>().text;
        string password = loginPassword.GetComponent<InputField>().text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password)) return;

        LoginRequest request = new LoginRequest { username = name, password = password };
        ApiRequest.Login(OnLoginReceived, request);
    }

    void OnLoginReceived(LoginResponse response)
    {
        if (!response.status)
        {
            errorText.text = response.message;
            return;
        }

        Game.Instance.token = response.token;
        Game.Instance.userId = response.user.id;
        Game.Instance.username = response.user.username;
        Game.Instance.coins = response.user.coins;
        SceneManager.LoadScene("LevelMenu");
    }

    public void OnRegisterBtn()
    {
        string name = regName.GetComponent<InputField>().text;
        string password = regPassword.GetComponent<InputField>().text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password)) return;

        RegisterRequest request = new RegisterRequest { username = name, password = password };
        ApiRequest.Register(OnRegisterReceived, request);
    }

    void OnRegisterReceived(RegisterResponse response)
    {
        if (!response.status)
        {
            errorText.text = response.message;
            return;
        }

        errorText.text = "";
        ShowLogin();
    }

    public void ShowRegister()
    {
        login.SetActive(false);
        register.SetActive(true);
        errorText.text = "";
    }

    public void ShowLogin()
    {
        login.SetActive(true);
        register.SetActive(false);
        errorText.text = "";
    }
}