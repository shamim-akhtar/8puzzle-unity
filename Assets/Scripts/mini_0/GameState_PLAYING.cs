using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_PLAYING : GameState
{
    public GameState_PLAYING(Patterns.FSM fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        Debug.Log("GameState_PLAYING");
        _puzzle.buttonBack.gameObject.SetActive(false);
        _puzzle.buttonNext.gameObject.SetActive(false);
        _puzzle.buttonStart.gameObject.SetActive(false);
        _puzzle.buttonHint.gameObject.SetActive(true);
        _puzzle.buttonCancel.gameObject.SetActive(_puzzle.m_astarSolved);
        _puzzle.buttonRetry.gameObject.SetActive(false);
    }

    public override void Exit()
    {
    }

    public void HandleTileClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject selected = Puzzle.Utils.Pick2D();
            if (selected != null)
            {
                // check for tile movement.
                int zero = _puzzle._state.GetEmptyTileIndex();
                int[] neighbours = Puzzle.Neighbours.Instance.GetNeighbors(zero);

                for (int i = 0; i < neighbours.Length; ++i)
                {
                    if (selected.name == _puzzle._state.Arr[neighbours[i]].ToString())
                    {
                        _puzzle.SwapTiles(neighbours[i], _puzzle._state, true);
                        _puzzle.audioSource.PlayOneShot(_puzzle.audioClickTile);
                        _puzzle.yourScore += 1;
                        _puzzle.textYourScore.text = _puzzle.yourScore.ToString();
                        _puzzle.yourSolutionStates.Add(new Puzzle.State(_puzzle._state));
                    }
                }
            }
        }
        if(_puzzle.buttonHint.Pressed)
        {
            m_fsm.SetCurrentState((int)StateID.SHOW_HINT);
        }
        if (_puzzle.buttonCancel.Pressed)
        {
            m_fsm.SetCurrentState((int)StateID.CANCEL);
            _puzzle.buttonCancel.Pressed = false;
        }
    }

    public override void Update()
    {
        HandleTileClick();

        bool solved = Puzzle.State.Equals(_puzzle._state, _puzzle._goal);
        if (solved)
        {
            m_fsm.SetCurrentState((int)StateID.WIN);
        }
    }
}