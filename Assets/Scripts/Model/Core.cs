﻿using Fumbbl.Ffb.Dto.ModelChanges;
using Fumbbl.Model.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fumbbl.Model
{
    public class Core
    {
        //private ModelChangeFactory ModelChangeFactory { get; }
        private ReflectedFactory<ModelUpdater<Ffb.Dto.ModelChange>, Type> ModelChangeFactory { get; }
        public ActingPlayer ActingPlayer { get; set; }

        private Dictionary<string, Player> Players { get; set; }

        public Coach HomeCoach { get; internal set; }
        public Dictionary<int, View.PushbackSquare> PushbackSquares;
        public Dictionary<int, View.TrackNumber> TrackNumbers;
        public List<View.BlockDie> BlockDice;
        public int BlockDieIndex;

        internal void AddPushbackSquare(PushbackSquare square)
        {
            int key = square.coordinate[0] * 100 + square.coordinate[1];
            if (!PushbackSquares.ContainsKey(key))
            {
                PushbackSquares.Add(key, new View.PushbackSquare(square));
            }
            else
            {
                PushbackSquares[key].Refresh(new View.PushbackSquare(square));
            }
        }

        internal void RemovePushbackSquare(PushbackSquare square)
        {
            int key = square.coordinate[0] * 100 + square.coordinate[1];
            PushbackSquares.Remove(key);
        }

        internal void AddTrackNumber(TrackNumber square)
        {
            int key = square.coordinate[0] * 100 + square.coordinate[1];
            if (!TrackNumbers.ContainsKey(key))
            {
                TrackNumbers.Add(key, new View.TrackNumber(square));
            }
            else
            {
                TrackNumbers[key].Refresh(new View.TrackNumber(square));
            }
        }

        internal void RemoveTrackNumber(TrackNumber square)
        {
            int key = square.coordinate[0] * 100 + square.coordinate[1];
            TrackNumbers.Remove(key);
        }

        public void AddBlockDie(int roll)
        {
            if (roll > 0)
            {
                if (BlockDice.Count > 0 && BlockDice[BlockDice.Count-1].Active == false)
                {
                    BlockDice.Clear();
                }
                int index = BlockDieIndex++;
                BlockDice.Add(new View.BlockDie(index, BlockDie.Get(roll)));
            }
            else
            {
                foreach (var die in BlockDice)
                {
                    die.Active = false;
                }
            }
        }

        public Coach AwayCoach { get; internal set; }
        public int Half { get; internal set; }
        public int TurnHome { get; internal set; }
        public int TurnAway { get; internal set; }
        public int ScoreHome { get; internal set; }
        public int ScoreAway { get; internal set; }
        public Team TeamHome { get; internal set; }
        public Team TeamAway { get; internal set; }
        public bool HomePlaying { get; internal set; }

        public Ball Ball;

        public Core()
        {
            ActingPlayer = new ActingPlayer();
            Ball = new Ball();
            BlockDice = new List<View.BlockDie>();
            //ModelChangeFactory = new ModelChangeFactory();
            ModelChangeFactory = new ReflectedFactory<ModelUpdater<Ffb.Dto.ModelChange>, Type>();
            Players = new Dictionary<string, Player>();
            PushbackSquares = new Dictionary<int, View.PushbackSquare>();
            TrackNumbers = new Dictionary<int, View.TrackNumber>();
            TurnMode = TurnMode.StartGame;
        }

        public void Clear()
        {
            Debug.Log("Clearing Model");
            Players.Clear();
            ActingPlayer.Clear();
            PushbackSquares.Clear();
            TrackNumbers.Clear();
            BlockDice.Clear();
        }

        internal IEnumerable<Player> GetPlayers()
        {
            return Players.Values;
        }

        internal void ApplyChange(Ffb.Dto.ModelChange change)
        {
            //IModelUpdater updater = ModelChangeFactory.Create(change);
            ModelUpdater<Ffb.Dto.ModelChange> updater = ModelChangeFactory.GetReflectedInstance(change.GetType());
            updater?.Apply(change);
        }

        internal Team GetTeam(string teamId)
        {
            if (string.Equals(teamId, TeamHome.Id))
            {
                return TeamHome;
            }
            else
            {
                return TeamAway;
            }
        }


        internal Player GetPlayer(string playerId)
        {
            if (playerId == null)
            {
                return null;
            }
            return Players[playerId];
        }

        internal void AddPlayer(Player player)
        {
            if (Players.ContainsKey(player.Id))
            {
                Players[player.Id] = player;
            }
            else
            {
                Players.Add(player.Id, player);
            }
        }

        internal Player GetPlayer(int x, int y)
        {
            foreach (Player p in Players.Values)
            {
                if (p.Coordinate.X == x && p.Coordinate.Y == y)
                {
                    return p;
                }
            }
            return null;
        }

        private TurnMode lastturnmode;
        public TurnMode LastTurnMode {
            get { return lastturnmode; }
            set {
                if (value == lastturnmode) return;
                lastturnmode = value;
                Debug.Log($"LastTurnMode: {lastturnmode.Name}");
            }
        }
        private TurnMode turnmode;
        public TurnMode TurnMode {
            get { return turnmode; }
            set {
                if (value == turnmode) return;
                TurnMode cachedlastturnmode = turnmode;
                turnmode = value;
                if (cachedlastturnmode != null && !cachedlastturnmode.StoreLast){
                    LastTurnMode = cachedlastturnmode;
                }
                Debug.Log($"TurnMode: {turnmode.Name}");
            }
        }

    }
}