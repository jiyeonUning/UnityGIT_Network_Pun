using Photon.Realtime;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperty
{
    public const string READY = "Ready";


    // this Player �ڷ��� = localPlayer ��� ��, �ش� �Լ��� ����� �� �ְ� ���ش�.
    public static void SetReady(this Player player, bool ready)
    {
        PhotonHashtable customProperty = new PhotonHashtable();
        customProperty[READY] = ready;
        player.SetCustomProperties(customProperty);
    }

    public static bool GetReady(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;

        if (customProperty.ContainsKey(READY))
        {
            return (bool)customProperty[READY];
        }
        else return false;
    }

    public const string LOAD = "Load";

    public static void SetLoad(this Player player, bool load)
    {
        PhotonHashtable customProperty = new PhotonHashtable();
        customProperty[LOAD] = load;
        player.SetCustomProperties(customProperty);
    }

    public static bool GetLoad(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;

        if (customProperty.ContainsKey(READY))
        {
            return (bool)customProperty[READY];
        }
        else return false;
    }
}
