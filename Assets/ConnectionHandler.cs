﻿using Fumbbl;
using Fumbbl.Lib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ConnectionHandler : MonoBehaviour
{
    public GameObject Progress;
    private bool connected;

    private RectTransform ProgressRect;
    private int PlayersToLoad = 0;
    private int progress = 0;
    // Start is called before the first frame update
    void Start()
    {
        FFB.Instance.Initialize();
        connected = false;
        PlayersToLoad = 0;
        ProgressRect = Progress.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    async void Update()
    {
        if (!connected)
        {
            var players = FFB.Instance.Model.GetPlayers().ToList();
            if (players.Count > 0)
            {
                // Load player icon sprites.
                connected = true;
                List<Task> tasks = new List<Task>();
                progress = 0;
                PlayersToLoad = players.Count;
                foreach (var player in players)
                {
                    string icon = player.Position.IconURL;
                    tasks.Add(FFB.Instance.SpriteCache.GetAsync(icon, s => { Interlocked.Increment(ref progress); }));
                }

                await Task.WhenAll(tasks);
                Debug.Log("Loaded Player Icons");
                MainHandler.Instance.SetScene(MainHandler.SceneType.MainScene);
            }
        }

        if (PlayersToLoad != 0)
        {
            ProgressRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1665 * progress / PlayersToLoad);
            Progress.SetActive(true);
        }
        else
        {
            Progress.SetActive(false);
        }
    }
}