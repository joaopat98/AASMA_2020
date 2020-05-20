﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFriend : Action
{
    public CallFriend(Agent agent) : base(agent)
    {

    }

    public override void Execute()
    {
        agent.SocialNeeds = 0.5f;
    }
}