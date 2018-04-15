using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class UnitTestingWindow : EditorWindow
{
    private struct TestResult
    {
        public string testName;
        public bool success;
    }

    [MenuItem("Window/Unit Testing")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow<UnitTestingWindow>("Unit Testing");
    }

    private List<Func<bool>> tests = new List<Func<bool>>();
    private List<TestResult> results = new List<TestResult>();
    private bool overallResult = true;

    private void OnEnable()
    {
        tests.Add(UnitTests.Test1);
        tests.Add(UnitTests.Test2);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Run Tests"))
        {
            RunTests();
        }

        if (results.Count != 0)
        {
            GUILayout.Label("Results:");
            foreach (TestResult result in results)
            {
                GUILayout.Label("\t" + result.testName + ": " + GetSuccessString(result.success));
            }
            GUILayout.Label("Overall result: " + GetSuccessString(overallResult));
        }
        else
        {
            GUILayout.Label("No results to display.");
        }
    }

    private void RunTests()
    {
        results = new List<TestResult>();
        overallResult = true;

        foreach (Func<bool> test in tests)
        {
            TestResult result = new TestResult()
            {
                testName = test.Method.Name,
                success = true
            };

            if (!test())
            {
                overallResult = false;
                result.success = false;
            }

            results.Add(result);
        }
    }

    private string GetSuccessString(bool success)
    {
        if (success)
        {
            return "Success";
        }
        return "Failure";
    }
}
