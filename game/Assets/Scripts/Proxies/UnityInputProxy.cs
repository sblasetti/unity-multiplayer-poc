using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Proxies
{
    public interface IUnityInputProxy
    {
        float GetAxis(string axis);
    }

    public class RealUnityInputProxy : IUnityInputProxy
    {
        public float GetAxis(string axis)
        {
            return Input.GetAxis(axis);
        }
    }
}
