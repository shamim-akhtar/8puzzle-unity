using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MENU_MINI_GAMES : MENU_STATE
{
    public MENU_MINI_GAMES(Patterns.FSM fsm, int id, GameMenu menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        gameMenu.btnMinigameParent.SetActive(true);

        // set all these buttons to transparent.
        for(int i = 0; i < gameMenu.btnMinigamesImages.Count; ++i)
        {
            Puzzle.Utils.SetImageTransparency(gameMenu.btnMinigamesImages[i], 0.0f);
        }

        base.Enter();
    }
    public override void Exit()
    {
        gameMenu.btnMinigameParent.SetActive(false);
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
        for (int i = 0; i < gameMenu.btnMinigamesImages.Count; ++i)
        {
            FadeIn(gameMenu.btnMinigamesImages[i], 1.0f);
        }

        for(int i = 0; i < gameMenu.btnMinigamesGameObjects.Count; ++i)
        {
            FixedButton fixedButton = gameMenu.btnMinigamesGameObjects[i].GetComponent<FixedButton>();
            if(fixedButton != null && fixedButton.Pressed)
            {
                gameMenu.LoadMiniGame(i);
                fixedButton.Pressed = false;
            }
        }
    }
}