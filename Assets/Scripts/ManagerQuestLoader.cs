using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ManagerQuestLoader
{
    #region Private variables
    private static ManagerQuestLoader instance = null;
    private static readonly object padlock = new object();
    #endregion

    #region Singleton Pattern
    ManagerQuestLoader()
    {
    }

    public static ManagerQuestLoader Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new ManagerQuestLoader();
                }
                return instance;
            }
        }
    }
    #endregion

    public string QuestIndexToSceneName(int index)
    {
        const string defaultScene = "quest_0";

        return defaultScene;
    }

}