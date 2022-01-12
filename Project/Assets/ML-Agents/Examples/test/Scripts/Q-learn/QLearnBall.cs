using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearnBall : MonoBehaviour
{
    // Start is called before the first frame update
    QLearnEnv m_env;
    Team lastHitTeam;
    Vector3 m_startPosition; 

    void Start()
    {
        m_env = GetComponentInParent<QLearnEnv>();
        m_startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "purpleGoal") //ball touched purple goal
        {
            m_env.setReward(Team.Blue, float.MaxValue);
        }
        if (col.gameObject.tag == "blueGoal") //ball touched blue goal
        {
            m_env.setReward(Team.Purple, float.MaxValue);
        }
        if (col.gameObject.tag == "purpleAgent")
        {
            lastHitTeam = Team.Purple;
            m_env.setReward(Team.Purple, 0.01f);
        }
        else if (col.gameObject.tag == "blueAgent")
        {
            lastHitTeam = Team.Blue;
            m_env.setReward(Team.Blue, 0.01f);
        }
        if (col.gameObject.tag == "wall")
        {
            m_env.setReward(1 - lastHitTeam, float.MinValue);
            ResetBall();
        }

    }
    public void ResetBall()
    {
        transform.position = m_startPosition;
        Rigidbody b = GetComponent<Rigidbody>();
        b.velocity = Vector3.zero;
        b.angularVelocity = Vector3.zero;
    }
}
