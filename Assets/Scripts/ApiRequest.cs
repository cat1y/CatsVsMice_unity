using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using static User;

public class ApiRequest : MonoBehaviour
{
    private static string apiUrl = "http://localhost:5000/api/";

    public static void Register(Action<RegisterResponse> callback, RegisterRequest request)
    {
        Game.Instance.StartCoroutine(Send("POST", "auth/register", request, false, callback));
    }

    public static void Login(Action<LoginResponse> callback, LoginRequest request)
    {
        Game.Instance.StartCoroutine(Send("POST", "auth/login", request, false, callback));
    }

    public static void GetProfile(Action<ProfileResponse> callback)
    {
        Game.Instance.StartCoroutine(Send("GET", "users/me", null, true, callback));
    }

    public static void GetLevels(Action<LevelsResponse> callback)
    {
        Game.Instance.StartCoroutine(Send("GET", "levels", null, true, callback, true));
    }

    public static void CompleteLevel(Action<CompleteLevelResponse> callback, int levelNumber, int score)
    {
        CompleteLevelRequest request = new CompleteLevelRequest { score = score };
        Game.Instance.StartCoroutine(Send("POST", "levels/" + levelNumber + "/complete", request, true, callback));
    }

    public static void GetShopCats(Action<CatsResponse> callback)
    {
        Game.Instance.StartCoroutine(Send("GET", "shop/cats", null, true, callback, true));
    }

    public static void GetMyCats(Action<CatsResponse> callback)
    {
        Game.Instance.StartCoroutine(Send("GET", "shop/my-cats", null, true, callback, true));
    }

    public static void BuyCat(Action<BuyCatResponse> callback, int catId)
    {
        Game.Instance.StartCoroutine(Send("POST", "shop/cats/" + catId + "/buy", null, true, callback));
    }

    private static IEnumerator Send<T>(string method, string path, object body, bool auth, Action<T> callback, bool isArray = false) where T : ApiResponse, new()
    {
        using (UnityWebRequest req = new UnityWebRequest(apiUrl + path, method))
        {
            if (body != null)
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(body));
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
                req.SetRequestHeader("Content-Type", "application/json");
            }
            else if (method == "POST")
            {
                req.uploadHandler = new UploadHandlerRaw(new byte[0]);
                req.SetRequestHeader("Content-Type", "application/json");
            }

            req.downloadHandler = new DownloadHandlerBuffer();

            if (auth && !string.IsNullOrEmpty(Game.Instance.token))
            {
                req.SetRequestHeader("Authorization", "Bearer " + Game.Instance.token);
            }

            yield return req.SendWebRequest();

            T response;

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                if (isArray) json = "{\"items\":" + json + "}";

                response = JsonUtility.FromJson<T>(json);
                if (response == null) response = new T();
                response.status = true;
            }
            else
            {
                response = new T();
                response.status = false;
                response.message = GetError(req);
                Debug.Log(response.message);
            }

            callback?.Invoke(response);
        }
    }

    private static string GetError(UnityWebRequest req)
    {
        if (req.result == UnityWebRequest.Result.ProtocolError)
        {
            try
            {
                ErrorBody error = JsonUtility.FromJson<ErrorBody>(req.downloadHandler.text);
                if (error != null && !string.IsNullOrEmpty(error.message)) return error.message;
            }
            catch { }
            return "Ошибка " + req.responseCode;
        }
        return "Нет соединения с сервером";
    }
}