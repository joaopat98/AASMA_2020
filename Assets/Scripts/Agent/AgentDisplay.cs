using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentDisplay : MonoBehaviour
{
    public Color HealthyColor, UnknowinglyInfectedColor, OpenlyInfectedColor, CuredColor, DeadColor;

    private SpriteRenderer healthBar, mainSprite;
    private Agent agent;
    private Vector3 healthBarScale;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<Agent>();
        mainSprite = GetComponent<SpriteRenderer>();
        healthBar = transform.Find("HealthBar").GetComponent<SpriteRenderer>();
        healthBarScale = healthBar.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        UpdateInfectionColor();
    }

    void UpdateHealthBar()
    {
        healthBar.color = Color.Lerp(Color.red, Color.green, agent.Health);
        var newScale = healthBarScale;
        newScale.x *= agent.Health;
        healthBar.transform.localScale = newScale;
    }

    void UpdateInfectionColor()
    {
        switch (agent.Infection)
        {
            case InfectionState.Healthy:
                mainSprite.color = HealthyColor;
                break;
            case InfectionState.UnknowinglyInfected:
                mainSprite.color = UnknowinglyInfectedColor;
                break;
            case InfectionState.OpenlyInfected:
                mainSprite.color = OpenlyInfectedColor;
                break;
            case InfectionState.Cured:
                mainSprite.color = CuredColor;
                break;
            case InfectionState.Dead:
                mainSprite.color = DeadColor;
                break;
        }
    }
}
