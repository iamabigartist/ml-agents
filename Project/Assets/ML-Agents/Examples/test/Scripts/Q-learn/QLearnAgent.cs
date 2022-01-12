using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearnAgent : MonoBehaviour
{
    public string position;
    QLearnEnv m_env;
    [HideInInspector]
    public int agentNum;

    float m_reward = 0.0f;
    List<Vector3> moveSpace = new List<Vector3>();
    Vector3 m_startPosition;
    Dictionary<string, int> Qvalue = new Dictionary<string, int>();
    List<GameObject> AgentsList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        m_env= GetComponentInParent<QLearnEnv>();
        m_startPosition = transform.position;
        foreach (var item in m_env.AgentsList)
        {
            if(item != gameObject)
            {
                AgentsList.Add(item.gameObject);
            }
        }

        for(int i = -1; i < 2; ++i)
        {
            for (int j = -1; j < 2; ++j)
            {
                moveSpace.Add(new Vector3(i * 1.0f, 0f, j * 1.0f));
            }      

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 getAction()
    {
        float f = UnityEngine.Random.value;
        Vector3 r = Vector3.zero;
        bool inline = false;
        if (f < m_env.epsilon)
        {
            while(!inline)
            {
                int a = Random.Range(0, moveSpace.Count);
                r = moveSpace[a];
                
                //inline = lineDetect(transform.position + r);
                //print(inline);
                inline = true;
            }
            return r;
        }
        else
        {
            return Qupdate();
        }
    }
    public void SetReward(float reward)
    {
        m_reward = reward;
    }
    Vector3 TransformAction(Vector3 action)
    {
        Vector3 move;
        if (position == "Back")
        {
            move = new Vector3(action.x * 1.0f, 0, action.z * 1.0f);
        }
        else if(position == "Striker")
        {
            move = new Vector3(action.x * 1.5f, 0, action.z * 0.5f);
        }
        else
        {
            move = new Vector3(action.x * 0.3f, 0, action.z * 1.5f);
        }
        return move;
    }
    public Vector3 Qupdate()
    {
        float maxValue = float.MinValue;
        List<Vector3> nextMove= new List<Vector3>();
        foreach(var action in moveSpace)
        {
            Vector3 move = TransformAction(action);
            float sample = CalQvalue(transform.position, move);
            if (sample > maxValue)
            {
                nextMove.Clear();
                nextMove.Add(move);
                maxValue = sample;
            }
            else if (! (sample < maxValue))
            {
                nextMove.Add(move);
            }
        }
        int i = Random.Range(0, nextMove.Count);
        float difference = (m_reward + m_env.DiscountRate * GetValue(transform.position + nextMove[i])) - maxValue;
        m_env.updateWeight(difference, agentNum, transform.position);
        return nextMove[i];
    }
    float GetQvalue(string state)
    {
        if(!Qvalue.ContainsKey(state))
        {
            Qvalue[state] = 0;
        }
        return Qvalue[state] = 0;

    }
    bool lineDetect(Vector3 position)
    {
        if (position.x < -8 -m_env.width || position.x > 8 + m_env.width)
        {
            return false;
        }
        if (position.z < -10 - m_env.length || position.x > 10 + m_env.length)
        {
            return false;
        }
        return true;
    }
    float GetValue(Vector3 position)
    {
        float max = float.MinValue;
        foreach (var action in moveSpace)
        {
            Vector3 move = TransformAction(action);
            if (!lineDetect(position + move))
                continue;
            string state = GetState(position, move);
            float value = GetQvalue(state);
            if (value > max)
                max = value;
        }
        return max;
    }
    string GetState(Vector3 position, Vector3 action)
    {
        string returnValue = "";
        int x;
        int y;
        int z;
        foreach (var item in AgentsList)
        {
            x = Mathf.RoundToInt(item.transform.position.x);
            z = Mathf.RoundToInt(item.transform.position.z);
            returnValue += x.ToString() + " " + z.ToString() + " ";
        }
        x = Mathf.RoundToInt(m_env.ball.transform.position.x);
        y = Mathf.RoundToInt(m_env.ball.transform.position.y);
        z = Mathf.RoundToInt(m_env.ball.transform.position.z);
        returnValue += x.ToString() + " " + y.ToString() + " " + z.ToString() + " ";
        x = Mathf.RoundToInt(position.x);
        z = Mathf.RoundToInt(position.z);
        returnValue += x.ToString() + " " + z.ToString() + " ";
        x = Mathf.RoundToInt(action.x);
        z = Mathf.RoundToInt(action.z);
        returnValue += x.ToString() + " " + z.ToString();
        return returnValue;
    }
    float CalQvalue(Vector3 position, Vector3 Action) //Since state is not parameter, need to action twice when input.
    {
        position += Action;
        float qvalue = 0;
        string id;
        Dictionary<string, Vector3> distance = new Dictionary<string, Vector3>();
        float sum = 0;
        foreach (var item in m_env.AgentsList)
        {
            if(item != gameObject)
            {
                id = item.GetComponent<QLearnAgent>().agentNum.ToString();
                distance.Add(id, item.transform.position - position);
                sum += Vector3.Distance(Vector3.zero, item.transform.position - position);
            }
        }
        sum += Vector3.Distance(Vector3.zero, m_env.ball.transform.position - position);
        foreach (string i in distance.Keys)
        {
            qvalue += Vector3.Dot(distance[i] / sum, m_env.getWeight(i));
        }
        qvalue += Vector3.Dot((m_env.ball.transform.position - position)/sum, m_env.getWeight("ball"));
        return qvalue;
    }
    public void ResetAgent()
    {
        transform.position = m_startPosition;
        Rigidbody b = GetComponent<Rigidbody>();
        b.velocity = Vector3.zero;
        b.angularVelocity = Vector3.zero;
    }

}
