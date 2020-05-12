using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    // Start is called before the first frame update
    Text PlayButton;
    InputField stepsField;
    SimulationManager manager;
    float stepsVal;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<SimulationManager>();
        stepsField = transform.Find("StepsPerSecond").GetComponent<InputField>();
        stepsVal = manager.StepsPerSecond;
        stepsField.SetTextWithoutNotify(stepsVal.ToString());
        PlayButton = transform.Find("Play").GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayButton.text = manager.Playing ? "Pause" : "Play";
    }

    public void PlayPause()
    {
        manager.Playing = !manager.Playing;
    }

    public void ReceiveSteps(string steps)
    {
        stepsVal = float.Parse(stepsField.text);
        var colors = stepsField.colors;
        colors.normalColor = colors.selectedColor = Color.yellow;
        stepsField.colors = colors;
    }

    public void SetSteps()
    {
        manager.StepsPerSecond = stepsVal;
        StartCoroutine(SaveHighlight());
    }

    public void Step()
    {
        manager.Step();
    }

    IEnumerator SaveHighlight()
    {
        var colors = stepsField.colors;
        colors.normalColor = colors.selectedColor = Color.green;
        stepsField.colors = colors;
        yield return new WaitForSeconds(0.5f);
        colors.normalColor = colors.selectedColor = Color.white;
        stepsField.colors = colors;
    }
}
