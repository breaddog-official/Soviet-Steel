using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Mirror
{
    [DisallowMultipleComponent]
    public class EmptyTransport : Transport, PortTransport
    {
        public ushort Port { get; set; }


        public override void ClientEarlyUpdate()
        {
            
        }

        public override void ServerEarlyUpdate()
        {
            
        }

        public override void ClientLateUpdate()
        {
            
        }

        public override void ServerLateUpdate()
        {
            
        }

        public override bool Available()
        {
            return true;
        }

        #region Client

        public override void ClientConnect(string address)
        {
            
        }

        public override void ClientConnect(Uri uri)
        {
            
        }

        public override bool ClientConnected()
        {
            return true;
        }

        public override void ClientDisconnect()
        {

        }

        public override void ClientSend(ArraySegment<byte> segment, int channelId)
        {

        }

        #endregion

        #region Server
        void AddServerCallbacks()
        {
            
        }

        // for now returns the first uri,
        // should we return all available uris?
        public override Uri ServerUri() => default;

        public override bool ServerActive()
        {
            return true;
        }

        public override string ServerGetClientAddress(int connectionId)
        {
            return string.Empty;
        }

        public override void ServerDisconnect(int connectionId)
        {
            
        }

        public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
        {
            
        }

        public override void ServerStart()
        {
            
        }

        public override void ServerStop()
        {
            
        }
        #endregion

        public override int GetMaxPacketSize(int channelId = 0)
        {
            return 1024;
        }

        public override void Shutdown()
        {
            
        }

        public override string ToString()
        {
            return "EmptyTransport";
        }
    }
}
