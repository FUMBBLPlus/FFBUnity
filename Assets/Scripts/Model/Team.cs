﻿namespace Fumbbl.Model
{
    public class Team
    {
        public Coach Coach { get; internal set; }

        public bool IsHome => Coach.IsHome;

        public Team()
        {
            Coach = new Coach();
        }
    }
}
