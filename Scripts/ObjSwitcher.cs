
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.Japan.Weather.Board
{
    public class ObjSwitcher : UdonSharpBehaviour
    {
        [SerializeField] GameObject[] objs;
        [SerializeField] GameObject[] unactiveObjs;
        public int index = 0;

        public void Switch()
        {
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].SetActive(i == index);
                unactiveObjs[i].SetActive(i != index);
            }
        }
        public void SetActiveObject(int i)
        {
            index = i;
            Switch();
        }
        public void SetActive0() => SetActiveObject(0);
        public void SetActive1() => SetActiveObject(1);
        public void SetActive2() => SetActiveObject(2);
    }
}
