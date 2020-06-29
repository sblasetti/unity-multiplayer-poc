﻿using Assets.Scripts.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public interface ILocalMovementController
    {
        Vector3 GetAxisDirection();
        void PerformLocalMoveOnFixedUpdate(Vector3 direction);
        void SetLocalPlayer(GameObject player);
        void SetSpeed(float speed);
        void SetRotationSpeed(float rotationSpeed);
    }

    public class LocalMovementController : ILocalMovementController
    {
        private readonly IUnityInputProxy unityInputProxy;
        private readonly INetworkController networkController;

        private GameObject localPlayer = null;
        private Rigidbody localPlayerRigidbody = null;
        private float speed = 0F;
        private float rotationSpeed = 0F;

        public LocalMovementController(IUnityInputProxy unityInputProxy, INetworkController networkController)
        {
            this.unityInputProxy = unityInputProxy;
            this.networkController = networkController;
        }

        public Vector3 GetAxisDirection()
        {
            var horizontal = unityInputProxy.GetAxis(INPUT_NAMES.AxisHorizontal);
            var vertical = unityInputProxy.GetAxis(INPUT_NAMES.AxisVertical);
            return new Vector3(horizontal, 0, vertical);
        }

        public void PerformLocalMoveOnFixedUpdate(Vector3 direction)
        {
            var horizontal = direction.x;
            var vertical = direction.z;

            #region Using physics (Rigidbody)

            if (vertical != 0)
            {
                VerticalMovementWithRigidbody(vertical);
            }

            if (horizontal != 0)
            {
                HorizontalMovementWithRigidbody(horizontal);
            }

            #endregion

            #region Using Transform (does not work with physics / collisions)

            // if (vertical != 0) {
            //    VerticalMovementWithTranform(vertical);
            // }

            //if (horizontal != 0)
            //{
            //    HorizontalMovementWithTransform(horizontal);
            //}

            #endregion

            if (localPlayer != null)
                Debug.DrawRay(localPlayer.transform.position, localPlayer.transform.forward, Color.red);

            if (horizontal == 0 && vertical == 0)
                return;

            //var playersMgmt = this.GetComponent<PlayersManagement>();
            //playersMgmt.SendPlayerMove(localPlayer.transform.position, horizontal, vertical);
        }

        private void HorizontalMovementWithRigidbody(float horizontal)
        {
            var h = horizontal * rotationSpeed * Time.deltaTime;
            var rotation = Quaternion.Euler(0F, h, 0F);
            localPlayerRigidbody.MoveRotation(localPlayerRigidbody.rotation * rotation);
        }

        private void HorizontalMovementWithTransform(float horizontal)
        {
            var h = horizontal * rotationSpeed * Time.deltaTime;
            localPlayer.transform.Rotate(Vector3.up, h);
        }

        private void VerticalMovementWithRigidbody(float vertical)
        {
            var v = vertical * speed * Time.deltaTime;
            // rb.AddForce(localPlayer.transform.forward * v, forceMode);
            // rb.velocity = localPlayer.transform.forward * vertical * force;
            localPlayerRigidbody.MovePosition(localPlayer.transform.position + (localPlayer.transform.forward * v));
        }

        private void VerticalMovementWithTransform(float vertical)
        {
            var v = vertical * speed * Time.deltaTime;
            localPlayer.transform.Translate(Vector3.forward * v);
        }

        public void SetLocalPlayer(GameObject player)
        {
            this.localPlayer = player;
            this.localPlayerRigidbody = player.GetComponent<Rigidbody>();
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public void SetRotationSpeed(float rotationSpeed)
        {
            this.rotationSpeed = rotationSpeed;
        }
    }

}