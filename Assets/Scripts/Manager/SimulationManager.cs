using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public GameObject CivilianPrefab, PolicePrefab, MedicalPrefab;
    [Range(0, 1)]
    public float AgentScale = 0.75f;
    public int NumCivilians = 25, NumPolice = 5, NumMedical = 5;
    public float StepsPerSecond = 1;
    float timePerStep;
    int numAgents;
    private List<Agent> agents;
    float counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        timePerStep = 1 / StepsPerSecond;
        numAgents = NumCivilians + NumPolice + NumMedical;
        int side = Mathf.CeilToInt(Mathf.Sqrt(numAgents));

        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        float minX = bounds.min.x;
        float minY = bounds.min.y;
        float width = bounds.size.x;
        float height = bounds.size.y;
        float sizeX = width / side, sizeY = height / side;
        Vector3 topLeft = bounds.max + width * Vector3.left + new Vector3(sizeX / 2, -sizeY / 2, 0);

        float scaleFac = 1 / (float)side * AgentScale;
        agents = new List<Agent>();
        int i = 0;
        for (int j = 0; j < NumCivilians; j++)
        {
            int x = i % side, y = i / side;
            agents.Add(Instantiate(CivilianPrefab, topLeft + new Vector3(x * sizeX, -y * sizeY, 0), Quaternion.identity, transform).GetComponent<Agent>());
            agents[i].transform.localScale *= scaleFac;
            i++;
        }
        for (int j = 0; j < NumPolice; j++)
        {
            int x = i % side, y = i / side;
            agents.Add(Instantiate(PolicePrefab, topLeft + new Vector3(x * sizeX, -y * sizeY, 0), Quaternion.identity, transform).GetComponent<Agent>());
            agents[i].transform.localScale *= scaleFac;
            i++;
        }
        for (int j = 0; j < NumMedical; j++)
        {
            int x = i % side, y = i / side;
            agents.Add(Instantiate(MedicalPrefab, topLeft + new Vector3(x * sizeX, -y * sizeY, 0), Quaternion.identity, transform).GetComponent<Agent>());
            agents[i].transform.localScale *= scaleFac;
            i++;
        }
    }

    void Update()
    {
        counter += Time.deltaTime;
        while (counter > timePerStep)
        {
            Step();
            counter -= timePerStep;
        }
    }

    void Step()
    {
        foreach (var agent in agents)
        {
            agent.Step();
        }
    }
}
