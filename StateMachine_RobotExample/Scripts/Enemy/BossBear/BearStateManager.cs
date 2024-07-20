using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class BearStateManager : MonoBehaviour
{
    
    
    //Health==
    [SerializeField]
    private float maxHealth = 200f;
    [SerializeField]
    float health;
    [SerializeField]
    Slider healthSlide;

    private float lastHealthRecord;
    //Health===



    [SerializeField]
    GameObject player;

    //StateBuildWall
    [SerializeField]
    GameObject wall;

    //StateKnifeRain
    [SerializeField]
    GameObject knife;
    //StateSummonTeam
    [SerializeField]
    GameObject minionTypeShoot;
    [SerializeField]
    GameObject minionTypePunch;

    List<IState> states = new List<IState>();
    IState stateBuildWall;
    IState stateKnifeRain;
    IState stateSummonTeam;
    IState stateExactKnife;
    IState currentState;
    Animator anim;


    private void Start()
    {
        anim = GetComponent<Animator>();
        health = this.maxHealth;
        lastHealthRecord = health;
        healthSlide.value = this.health / this.maxHealth;

        stateBuildWall = new StateBuildWall(this, wall, this.transform);
        stateKnifeRain = new StateKnifeRain(this, knife, this.transform);
        stateSummonTeam = new StateSummonTeam(this, minionTypeShoot, minionTypePunch, this.transform);
        stateExactKnife = new StateExactKnife(this, knife, this.transform, player.transform, 20f);
        states.Add(stateBuildWall);
        states.Add(stateKnifeRain);
        states.Add(stateSummonTeam);
        states.Add(stateExactKnife);

        StateEnter(stateExactKnife);
    }

    public void StateEnter(IState state)
    {
        anim.SetBool("punch", true);
        Debug.Log("Enter State");
        state.Enter();
        currentState = state;
    }


    public void StateExit(float exitTime)
    {
        StartCoroutine(ExitWait(exitTime));
    }
    IEnumerator ExitWait(float exitTime)
    {
        yield return new WaitForSeconds(exitTime);
        anim.SetBool("punch", false);
        Debug.Log("Exit State");
        currentState = null;
        ChooseNewState();
    }

    public void ChooseNewState()
    {
        if (lastHealthRecord - health >= maxHealth / 10)
        {
            lastHealthRecord = health;
            //Phong thu
            float randomChoice = UnityEngine.Random.Range(1, 10);
            if (randomChoice > 5)
            {
                StateEnter(stateBuildWall);
            }
            else
            {
                StateEnter(stateSummonTeam);
            }
        }
        else
        {
            //Tan cong
            float randomChoice = UnityEngine.Random.Range(1, 10);
            if (randomChoice > 1)
            {
                StateEnter(stateKnifeRain);
            }
            else
            {
                StateEnter(stateExactKnife);
            }
        }
    }

    public GameObject InstantiateNew(GameObject obj, Vector3 pos, Quaternion rot)
    {
        return Instantiate(obj, pos, rot);
    }

    public void UseCoroutine(float waitTime, Action action)
    {
        StartCoroutine(WaitForAction(waitTime, action));
    }

    IEnumerator WaitForAction(float delayTime, Action action)
    {
        yield return new WaitForSeconds(delayTime);
        action();
    }


    public void ReduceHealth(float damage)
    {
        this.health -= damage;
        healthSlide.value = this.health / this.maxHealth;
        if (health <= 0) HandleDeath();
    }

    private void HandleDeath()
    {
        Action destroyMe = () =>
        {
            Destroy(gameObject);
        };
        ClearGarbage(destroyMe);
    }

    private void ClearGarbage(Action action)
    {
        List<GameObject> garbages = new List<GameObject>();
        foreach (var state in states)
        {
            if(state.CollectGarbage() != null)
                garbages.AddRange(state.CollectGarbage());

        }
        GameObject garbage;
        while (garbages.Count > 0)
        {
            garbage = garbages[0];
            garbages.RemoveAt(0);
            Destroy(garbage.gameObject);
        }
        action();
    }
}
