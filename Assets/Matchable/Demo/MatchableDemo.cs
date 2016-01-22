using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections;

using MatchableSDK;

public class MatchableDemo : MonoBehaviour
{
    Matchable _match;
    string _test = "";
    string _log = "";
    int _curIndex;
    int _demoIndex;
    delegate IEnumerator DemoFunction();
    DemoFunction[] _demos;
    float _initialTime;

    void Awake()
    {
        _match = new Matchable("demoab", "demo_57");
        _demos = new DemoFunction[]
        {
            DemoPlayerStats,
            DemoPlayerAdvisor,
            DemoSendPlayerActions
        };

        Next();
        _demoIndex = 0;
    }

    public IEnumerator DemoPlayerStats()
    {
        _test = "GetPlayerStats";
        _log = "Waiting for response...";
        yield return StartCoroutine(_match.GetStats((stats) => {
            _log = stats.ToString();
        }));
    }

    public IEnumerator DemoPlayerAdvisor()
    {
        _test = "GetPlayerAdvisor";
        _log = "Waiting for response...";
        yield return StartCoroutine(_match.GetAdvisor((advisor) => {
            _log = advisor.ToString();
        }));
    }
    
    public IEnumerator DemoSendPlayerActions()
    {
        _test = "SendPlayerAction";
        _log = "Waiting for response...";
        Hashtable parameters = new Hashtable();
        parameters.Add("date", TimeStamp.UnixTimeStampUTC());
        yield return StartCoroutine(_match.SendAction("session_start", parameters, (actions) => {
            _log = actions.ToString();
        }));
    }

    void Next()
    {
        _demoIndex = (_demoIndex + 1) % _demos.Length;
    }

    IEnumerator StartDemo()
    {
        _initialTime = Time.realtimeSinceStartup;
        _log = "";
        _curIndex = _demoIndex;
        yield return StartCoroutine(_demos[_demoIndex]());
        Next();
    }

    void OnGUI()
    {
        GUIStyle style;
        style = new GUIStyle(GUI.skin.box);
        style.wordWrap = true;
        style.padding = new RectOffset(5, 5, 5, 5);
        style.normal.textColor = Color.white;
        GUI.skin.box = style;

        Rect rect;
        rect = new Rect(0, 0, Screen.width, Screen.height);

        string text = String.Format("{0}/{1}: {2}\n\n{3}",
            _curIndex + 1, _demos.Length, _test, _log);

        GUI.Box(rect, text);

        rect = new Rect(10, Screen.height - 100, Screen.width - 20, 90);

        if (GUI.Button(rect, "NEXT"))
        {
            StartCoroutine(StartDemo());
        }
    }
}
