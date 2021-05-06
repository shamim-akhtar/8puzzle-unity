using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuState : Patterns.State
{
    public enum StateTypes
    {
        FADEIN,
        STAY,
        FADEOUT,
        MAINBACKGROUND,
    }
    protected SplashScreen m_menu;
    protected float m_deltaTime = 0.0f;

    public MainMenuState(Patterns.FSM fsm, int id, SplashScreen menu) 
        : base(fsm, id)
    {
        m_menu = menu;
    }
    public override void Enter()
    {
        m_deltaTime = 0.0f;
    }
    public override void Exit() { }
    public override void Update() { }
    public override void FixedUpdate() { }

    protected void FadeIn(Image image, float duration)
    {
        m_deltaTime += Time.deltaTime;
        if (m_deltaTime <= duration)
        {
            Color c = image.color;
            c.a = m_deltaTime / duration;
            image.color = c;
        }
    }

    protected void FadeOut(Image image, float duration)
    {
        m_deltaTime += Time.deltaTime;
        if (m_deltaTime <= duration)
        {
            Color c = image.color;
            c.a = 1.0f - m_deltaTime / duration;
            image.color = c;
        }
    }
}

public class MainMenuState_FADEIN : MainMenuState
{
    private float m_duration = 2.0f;
    public MainMenuState_FADEIN(Patterns.FSM fsm, int id, SplashScreen menu) 
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
        m_menu.Source.PlayOneShot(m_menu.audioSplash);
        m_menu.LogoGameobject.SetActive(true);
    }
    public override void Exit() { }
    public override void Update()
    {
        m_deltaTime += Time.deltaTime;
        if(m_deltaTime <= m_duration)
        {
            Color c = m_menu.Logo.color;
            c.a = m_deltaTime / m_duration;
            m_menu.Logo.color = c;
        }
        else
        {
            m_fsm.SetCurrentState((int)StateTypes.STAY);
        }
    }
}

public class MainMenuState_STAY : MainMenuState
{
    private float m_duration = 2.0f;
    public MainMenuState_STAY(Patterns.FSM fsm, int id, SplashScreen menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit() { }
    public override void Update()
    {
        m_deltaTime += Time.deltaTime;
        if (m_deltaTime <= m_duration)
        {
        }
        else
        {
            m_fsm.SetCurrentState((int)StateTypes.FADEOUT);
        }
    }
}

public class MainMenuState_FADEOUT : MainMenuState
{
    private float m_duration = 1.0f;
    public MainMenuState_FADEOUT(Patterns.FSM fsm, int id, SplashScreen menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        m_menu.Source.Stop();
        m_menu.LogoGameobject.SetActive(false);
    }
    public override void Update()
    {
        m_deltaTime += Time.deltaTime;
        if (m_deltaTime <= m_duration)
        {
            Color c = m_menu.Logo.color;
            c.a = 1.0f - m_deltaTime / m_duration;
            m_menu.Logo.color = c;
        }
        else
        {
            m_fsm.SetCurrentState((int)StateTypes.MAINBACKGROUND);
        }
    }
}

public class MainMenuState_MAINBACKGROUND : MainMenuState
{
    //private float m_duration1 = 4.0f;
    //private float m_duration2 = 5.0f;

    public MainMenuState_MAINBACKGROUND(Patterns.FSM fsm, int id, SplashScreen menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
        //m_menu.Source.PlayOneShot(m_menu.audioMenu);
    }
    public override void Exit() { }
    public override void Update()
    {
        m_menu.LoadGameMenu();
    }
}

public class SplashScreen : MonoBehaviour
{
    public GameObject LogoGameobject;

    public Image Filler;

    [HideInInspector]
    public Image Logo;

    public AudioClip audioSplash;

    [HideInInspector]
    public AudioSource Source;

    private Patterns.FSM m_fsm;

    void Awake()
    {
        Logo = LogoGameobject.GetComponent<Image>();
        Color c = Logo.color;
        c.a = 0.0f;
        LogoGameobject.SetActive(false);

        Source = GetComponent<AudioSource>();

        // create the FSM.
        m_fsm = new Patterns.FSM();
        m_fsm.Add(new MainMenuState_FADEIN(m_fsm, (int)MainMenuState.StateTypes.FADEIN, this));
        m_fsm.Add(new MainMenuState_FADEOUT(m_fsm, (int)MainMenuState.StateTypes.FADEOUT, this));
        m_fsm.Add(new MainMenuState_STAY(m_fsm, (int)MainMenuState.StateTypes.STAY, this));
        m_fsm.Add(new MainMenuState_MAINBACKGROUND(m_fsm, (int)MainMenuState.StateTypes.MAINBACKGROUND, this));

        m_fsm.SetCurrentState((int)MainMenuState.StateTypes.FADEIN);
    }

    // Update is called once per frame
    void Update()
    {
        m_fsm.Update();
    }

    public void LoadGameMenu()
    {
        // for now we only have the 8 puzzle game.
        //SceneManager.LoadScene(1);
        LoadMiniGame(0);
    }

    public void LoadMiniGame(int index)
    {
        // manually call exit because there are no other states.
        m_fsm.GetCurrentState().Exit();
        m_fsm = null;

        string sceneName = "mini_" + index.ToString();
        Source.Stop();
        // for now we only have the 8 puzzle game.
        SceneManager.LoadScene(sceneName);
    }
}
