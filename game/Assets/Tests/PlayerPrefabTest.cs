using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerPrefabTest
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator PlayerPrefab_HasNetworkMove()
        {
            var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
            var networkMove = player.GetComponent<RemoteMovement>();
            yield return new WaitForSeconds(0.1f);
            Assert.IsNotNull(networkMove);
            Object.Destroy(player.gameObject);
        }
    }
}
