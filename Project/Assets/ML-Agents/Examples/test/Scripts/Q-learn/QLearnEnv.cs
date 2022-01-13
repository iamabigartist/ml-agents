using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearnEnv : MonoBehaviour
{
    public List<GameObject> AgentsList = new List<GameObject>();
    public float width;
    public float length;
    public GameObject wall_z;
    public GameObject wall__z;
    public GameObject wall_x;
    public GameObject wall__x;

    [SerializeField]
    public int Max = 50000;
    public int count = 0;
    [SerializeField]
    public float epsilon;
    List<QLearnAgent> scriptsList = new List<QLearnAgent>();
    [SerializeField]
    public float learningRate;
    [SerializeField]
    public float DiscountRate;
    public GameObject ball;
    List<float> featuresWeight = new List<float>();

    [HideInInspector]
    public Dictionary<string, Vector3> weight = new Dictionary<string, Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        width = 6.0f;
        length = 14.0f;
        DiscountRate = 0.5f;
        epsilon = 0.2f;
        learningRate = 0.01f;
        for (int i = 0; i < AgentsList.Count; i++)
        {
            featuresWeight.Add(1);
            scriptsList.Add(AgentsList[i].GetComponent<QLearnAgent>());
            scriptsList[i].agentNum = i;
            weight.Add(i.ToString(), Vector3.zero);
        }
        weight.Add("ball", Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        foreach (var item in AgentsList)
        {
            item.transform.Translate(item.GetComponent<QLearnAgent>().getAction(), Space.Self);
            item.GetComponent<QLearnAgent>().SetReward(0);
        }
        if(count ++ > Max)
        {
            ResetEnv();
        }
    }
    public Vector3 getWeight(string idx)
    {
        return weight[idx];
    }
    public void updateWeight(float difference, int id , Vector3 position)
    {
        for(int i = 0; i < AgentsList.Count; i++)
        {
            if(i != id)
                weight[i.ToString()] += learningRate * difference * (AgentsList[i].transform.position - position);
        }
        weight["ball"] += learningRate * difference * (ball.transform.position - position);
    }
    public void setReward(Team team, float reward)
    {
        foreach (var item in AgentsList)
        {
            if(team == Team.Blue)
            {
                if(item.tag == "blueAgent")
                {
                    item.GetComponent<QLearnAgent>().SetReward(reward/Max);
                }
                else
                {
                    item.GetComponent<QLearnAgent>().SetReward(-reward);
                }
            }
            else
            {
                if (item.tag == "purpleAgent")
                {
                    item.GetComponent<QLearnAgent>().SetReward(reward / Max);
                }
                else
                {
                    item.GetComponent<QLearnAgent>().SetReward(-reward);
                }

            }
            
        }
    }
    public void ResetEnv()
    {
        ball.GetComponent<QLearnBall>().ResetBall();
        foreach(var agent in AgentsList)
        {
            agent.GetComponent<QLearnAgent>().ResetAgent();
        }
    }
}
