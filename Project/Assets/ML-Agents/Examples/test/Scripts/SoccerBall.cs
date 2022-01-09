using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public GameObject area;
    [HideInInspector]
    public SoccerEnv envController;
    public string purpleGoalTag; //will be used to check if collided with purple goal
    public string blueGoalTag; //will be used to check if collided with blue goal
    Team lastHitTeam;
    void Start()
    {
        envController = area.GetComponent<SoccerEnv>();
        lastHitTeam = Team.Blue;  //Initialize as blue, will change when some people hit.
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(purpleGoalTag)) //ball touched purple goal
        {
            envController.GoalTouched(Team.Blue);
        }
        if (col.gameObject.CompareTag(blueGoalTag)) //ball touched blue goal
        {
            envController.GoalTouched(Team.Purple);
        }
        if(col.gameObject.tag == "purpleAgent")
        {
            lastHitTeam = Team.Purple;
        }
        else if (col.gameObject.tag == "blueAgent")
        {
            lastHitTeam = Team.Blue;
        }
        if(col.gameObject.tag == "wall")
        {
            envController.KickOut(lastHitTeam);
        }
        
    }
}
