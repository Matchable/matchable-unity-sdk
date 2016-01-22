using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections;

using MatchableSDK;

public class MatchableDemo : MonoBehaviour
{
    string _test = "";
    string _log = "";

    /// <summary>
    /// Demo of call the the GetStats SDK method.
    /// Displays the response JSON string.
    /// </summary>
    public IEnumerator DemoGetStats()
    {
        _test = "Matchable.GetStats()";
        _log = "Waiting for response...";
        // Call GetStats asynchronously as a Coroutine
        yield return StartCoroutine(Matchable.GetStats((response) =>
        {
            // Handle the API response the way you want
            _log = response.ToJsonString();
        }));
    }

    /// <summary>
    /// Demo of call the the GetAdvisor SDK method.
    /// Displays the response JSON string.
    /// </summary>
    public IEnumerator DemoGetAdvisor()
    {
        _test = "Matchable.GetAdvisor()";
        _log = "Waiting for response...";
        // Call GetAdvisor asynchronously as a Coroutine
        yield return StartCoroutine(Matchable.GetAdvisor((response) =>
        {
            // Handle the API response the way you want
            _log = response.ToJsonString();
            // Get the advisor
            _log += "\n\nAdvisor: " + response.GetKey("advisor");
        }));
    }

    /// <summary>
    /// Demo of call the the SendAction SDK method.
    /// Displays the response JSON string.
    /// </summary>
    public IEnumerator DemoSendAction()
    {
        _test = "Matchable.SendAction()";
        _log = "Waiting for response...";
        Hashtable parameters = new Hashtable();
        parameters.Add("version", "1.0");
        // Call SendAction asynchronously as a Coroutine
        yield return StartCoroutine(Matchable.SendAction("start_session", parameters, (response) =>
        {
            // Handle the API response the way you want
            _log = response.ToJsonString();
        }));
    }

    void OnGUI()
    {
        GUIStyle style;
        style = new GUIStyle(GUI.skin.box);
        style.wordWrap = true;
        style.padding = new RectOffset(5, 5, 5, 5);
        style.normal.textColor = Color.white;
        GUI.skin.box = style;

        int buttonSize = Screen.width / 3 - 10;
        int buttonHeight = 50;

        Rect rect = new Rect(10, 10, buttonSize, buttonHeight);
        if (GUI.Button(rect, "GetStats()"))
        {
            StartCoroutine(DemoGetStats());
        }

        rect = new Rect(buttonSize+10, 10, buttonSize, buttonHeight);
        if (GUI.Button(rect, "GetAdvisor()"))
        {
            StartCoroutine(DemoGetAdvisor());
        }

        rect = new Rect(2*buttonSize+10, 10, buttonSize, buttonHeight);
        if (GUI.Button(rect, "SendAction()"))
        {
            StartCoroutine(DemoSendAction());
        }

        rect = new Rect(10, buttonHeight + 20, Screen.width - 20, Screen.height);
        string text = String.Format("{0}\n\n{1}", _test, _log);
        GUI.Box(rect, text);

    }
}
