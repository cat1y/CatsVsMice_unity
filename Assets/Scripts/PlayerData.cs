using static User;

public static class PlayerData
{
    public static CatData[] Cats = new CatData[0];
    public static LevelData[] Levels = new LevelData[0];

    public static int GetBalance()
    {
        return Game.Instance.coins;
    }

    public static CatData GetCat(int id)
    {
        foreach (CatData cat in Cats)
        {
            if (cat.id == id) return cat;
        }
        return null;
    }

    public static bool IsOwned(int id)
    {
        CatData cat = GetCat(id);
        return cat != null && cat.isOwned;
    }

    public static bool IsLevelUnlocked(int number)
    {
        foreach (LevelData level in Levels)
        {
            if (level.id == number) return level.isUnlocked;
        }
        return number == 1;
    }
}