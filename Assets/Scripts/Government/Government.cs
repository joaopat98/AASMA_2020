using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Government
{
    public struct Advice
    {
        public bool useMask;

        [Range(0, 1)]
        public float socialDistancing;

        public Advice(bool _useMask, float _socialDistancing)
        {
            useMask = _useMask;
            socialDistancing = _socialDistancing;
        }
    };

    protected Advice currentAdvice = new Advice(false, 0.0f);


    //Portugal: Social distancing at 331 infected and 1 dead
    //          Masks mandatory at 24987 infected and 1007 dead
    //          Population = 10 280 000
    //////////////
    //UK:       Social distancing at 3900 infected and 81 dead
    //          Masks still not mandatory (144127 infected and 34796 dead)
    //          Population = 66 650 000
    [Range(0, 1)]
    public float boldness = 0.5f;

    protected float socialDistanceThreshSick = 0.0f;
    protected float socialDistanceThreshDead = 0.0f;
    protected float maskThreshSick = 0.0f;
    protected float maskThreshDead = 0.0f;

    public void Init()
    {
        //If boldness = 1,
        //never use mask
        //Social distancing at 0.585 % infected and 0.0121 % dead
        //If boldness = 0,
        //Masks mandatory at 24.3 % infected and 0.980 % dead
        //Social distancing at 0.322 % infected and 0 % dead
        socialDistanceThreshSick = Mathf.Lerp(0.00322f, 0.00585f, boldness);
        socialDistanceThreshDead = Mathf.Lerp(0, 0.000121f, boldness);
        maskThreshSick = Mathf.Lerp(0.243f, 1f, boldness);
        maskThreshDead = Mathf.Lerp(0.0098f, 1f, boldness);
    }

    public Advice GetAdvice()
    {
        return this.currentAdvice;
    }

    protected void DecideAdvice(float _percentDead, float _percentSick)
    {
        if (socialDistanceThreshSick + socialDistanceThreshDead != 0)
            this.currentAdvice.socialDistancing = Mathf.Lerp(0, 1, (_percentDead + _percentSick) / (socialDistanceThreshSick + socialDistanceThreshDead));
        else
            this.currentAdvice.socialDistancing = 0;

        if (_percentDead > maskThreshDead || _percentSick > maskThreshSick)
            this.currentAdvice.useMask = true;

    }

    public void Step(int _civilianCount, int _infectedCount, int _deadCount)
    {
        float percentDead = _deadCount / (float)_civilianCount;
        float percentSick = _infectedCount / (float)_civilianCount;

        DecideAdvice(percentDead, percentSick);
    }
}

