using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MENU_DAILY_QUEST : MENU_STATE
{
    public MENU_DAILY_QUEST(Patterns.FSM fsm, int id, GameMenu menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        gameMenu.LoadDailyQuestUI();
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
    }
}