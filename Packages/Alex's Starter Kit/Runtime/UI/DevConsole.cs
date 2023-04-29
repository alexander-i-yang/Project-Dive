using System;
using UnityEngine;
using UnityEngine.UIElements;
using ASK.Core;

namespace ASK.UI
{
    public abstract class DevConsole : MonoBehaviour {
        private bool _show = false;

        private TextField _input;

        [SerializeField] private GameObject objToShow;

        // public abstract String[] Commands() => new []{"give", "teleport", "time"};
        // public virtual Action<string>[] CommandActions() => {Give, Teleport, TimeScale};
        public abstract String[] Commands();
        public abstract Action<string>[] CommandActions();

        private void OnEnable()
        {
            Game.Instance.OnDebugEvent += Show;
        }
        
        private void OnDisable()
        {
            Game.Instance.OnDebugEvent -= Show;
        }

        public void ProcessCommand(string cmd) {
            var cmdArr = cmd.Split(' ');
            int ind = Array.IndexOf(Commands(), cmdArr[0]);
            try {
                CommandActions()[ind](cmd);
            } catch (Exception e) {
                Debug.LogException(e);
                //print(ind);
            }
        }

        public void Show() {
            _show = true;
            objToShow.SetActive(true);
        }

        /*static void Give(string num)
        {
            int numToGive = Convert.ToInt32(num.Split(" ")[1]);
            var inventory = FindObjectOfType<PlayerInventory>();
            inventory.AddItem(Firefly.s_ID, numToGive);
            FindObjectOfType<FireflyUICounter>().HandleCollectibleAdded(inventory.NumCollectibles(Firefly.s_ID));
        }

        static void Teleport(string cmd)
        {
            var cmdArr = cmd.Split(" ");
            int x = Convert.ToInt32(cmdArr[1]);
            int y = Convert.ToInt32(cmdArr[2]);
            PlayerCore.Actor.transform.position = new Vector3(x, y);
        }*/

        static void TimeScale(string cmd)
        {
            var cmdArr = cmd.Split(" ");
            double scale = Convert.ToDouble(cmdArr[1]);
            Game.Instance.TimeScale = (float)scale;
        }
    }
}