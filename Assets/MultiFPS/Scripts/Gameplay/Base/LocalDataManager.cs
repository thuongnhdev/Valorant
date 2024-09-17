using MultiFPS;
using UnityEngine;

public static class LocalDataManager
{

    private static string _playerPrefs_CharacterModelID = "_playerPrefs_CharacterModelID";

    
    public static void SetCharacterModelId(int id)
    {
        UserSettings.SelectedCharacterModel = id;
        PlayerPrefs.SetInt(_playerPrefs_CharacterModelID, UserSettings.SelectedCharacterModel);
    }

    public static int LoadCharacterModelId()
    {
        return UserSettings.SelectedCharacterModel = PlayerPrefs.GetInt(_playerPrefs_CharacterModelID, 0);
    }
}
