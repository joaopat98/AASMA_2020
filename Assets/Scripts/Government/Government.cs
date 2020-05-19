using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Government
{
    public struct Advice
    {
        public bool useMask, socialDistancing;

        public Advice(bool _useMask, bool _socialDistancing)
        {
            useMask = _useMask;
            socialDistancing = _socialDistancing;
        }
    };

    protected Advice currentAdvice = new Advice(false, false);


    //Portugal: Social distancing at 331 infected and 1 dead
    //          Masks mandatory at 24987 infected and 1007 dead
    //          Population = 10 280 000
    //////////////
    //UK:       Social distancing at 3900 infected and 81 dead
    //          Masks still not mandatory (144127 infected and 34796 dead)
    //          Population = 66 650 000
    [Range(0,1)]
    public float boldness = 0.5f;

    public Advice GetAdvice()
    {
        return this.currentAdvice;
    }

    protected void DecideAdvice(float _percentDead, float _percentSick)
    {
        //If boldness = 1,
        //never use mask
        //Social distancing at 0.00585 % infected and 0.000121 % dead
        //If boldness = 0,
        //Masks mandatory at 0.243 % infected and 0.00980 % dead
        //Social distancing at 0.00322 % infected and 0 % dead
        
        
        //We should define a neighborhood as X percent of the population, thus simluating a small percentage but having the government act as if there was a larger population.


        this.currentAdvice.useMask = true;
        this.currentAdvice.socialDistancing = true;
    }

    public void Step(int _civilianCount, int _infectedCount, int _deadCount)
    {
        float percentDead = _deadCount / _civilianCount;
        float percentSick = _infectedCount / _civilianCount;

        DecideAdvice(percentDead, percentSick);
    }
}

