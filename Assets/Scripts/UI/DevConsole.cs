using System;
using Collectibles;
using Core;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class DevConsole : MonoBehaviour {
        private bool _show = false;

        private TextField _input;

        public static readonly String[] COMMANDS = {"give", "teleport", "time"};
        public Action<string>[] COMMAND_ACTIONS = {Give, Teleport, TimeScale};

        void Awake() {
            SetShow(true); //We manually set everything inactive to make it easier to edit, undo this to find references
            _input = GetComponentInChildren<TextField>();
            SetShow(false);
        }

        
        /*void Update() {
            if (_show) {
                if (Input.GetKeyDown(KeyCode.z)) {
                    ProcessCommand(_input.text);
                    _input.text = "";
                    FocusField();
                }
            }
        }*/

        public void ProcessCommand(string cmd) {
            var cmdArr = cmd.Split(' ');
            int ind = Array.IndexOf(COMMANDS, cmdArr[0]);
            try {
                COMMAND_ACTIONS[ind](cmd);
            } catch (Exception e) {
                Debug.LogException(e);
                //print(ind);
            }
        }

        void SetShow(bool s) {
            _show = s;
            for (int i = 0; i < transform.childCount; ++i) {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(_show);
            }

            if (_show) {
                FocusField();
            }
        }

        void FocusField() {
            //_input.SelectAll();
            // _input.
        }

        static void Give(string num)
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
        }

        static void TimeScale(string cmd)
        {
            var cmdArr = cmd.Split(" ");
            double scale = Convert.ToDouble(cmdArr[1]);
            Game.Instance.TimeScale = (float)scale;
        }

        /*static void ClearTutorial(string e) {
            var boundaries = GameObject.Find("Boundaries");
            if (boundaries) { boundaries.gameObject.SetActive(false);}

            var hq = GameObject.Find("HQ");
            if (hq) {
                for (int i = 0; i < hq.transform.childCount; ++i) {
                    Transform child = hq.transform.GetChild(i);
                    child.gameObject.SetActive(true);
                }
            }
        }

        static void UnlockRoom(string cmd) {
            var rooms = GameObject.FindObjectsOfType<UnlockableSquares>();
            var cmdArr = cmd.Split(' ');
            if (cmdArr.Length == 1) {
                foreach (var room in rooms) {
                    room.Unlock();
                }
            } else {
                int arg = Convert.ToInt32(cmdArr[1]);
                Array.Sort(
                    rooms, 
                    (a, b) => (int) (a.transform.position.y - b.transform.position.y)
                );
                rooms[arg].Unlock();
            }
        }

        static void RhythmLockToggle(string cmd) {
            Conductor.Instance.RhythmLock = !Conductor.Instance.RhythmLock;
        }*/
    }
}