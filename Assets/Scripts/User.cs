using System;
using UnityEngine;

public class User : MonoBehaviour
{
    [Serializable]
    public class ApiResponse
    {
        public bool status;
        public string message;
    }

    [Serializable]
    public class ErrorBody
    {
        public string message;
    }

    [Serializable]
    public class RegisterRequest
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class UserData
    {
        public int id;
        public string username;
        public int coins;
    }

    [Serializable]
    public class RegisterResponse : ApiResponse
    {
        public int id;
        public string username;
        public int coins;
    }

    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class LoginResponse : ApiResponse
    {
        public string token;
        public UserData user;
    }

    [Serializable]
    public class ProfileResponse : ApiResponse
    {
        public int id;
        public string username;
        public int coins;
    }

    [Serializable]
    public class LevelData
    {
        public int id;
        public int rewardCoins;
        public bool isUnlocked;
        public bool isCompleted;
    }


    [Serializable]
    public class LevelsResponse : ApiResponse
    {
        public LevelData[] items;
    }

    [Serializable]
    public class CompleteLevelRequest
    {
        public int score;
    }

    [Serializable]
    public class CompleteLevelResponse : ApiResponse
    {
        public bool firstCompletion;
        public int coinsAwarded;
        public int totalCoins;
        public int nextLevelNumber;
    }

    [Serializable]
    public class CatData
    {
        public int id;
        public int price;
        public int limit;
        public int damage;
        public int health;
        public bool isOwned;
    }

    [Serializable]
    public class CatsResponse : ApiResponse
    {
        public CatData[] items;
    }

    [Serializable]
    public class BuyCatResponse : ApiResponse
    {
        public int catId;
        public int coinsLeft;
    }
}