using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using ClassLibrary;

namespace TrackerServer
{
    class ChildUnregHandler
    {
        //bool[] childDead;
        //List<PeerNode> childPeers;

        public ChildUnregHandler(TrackerMainFrm mainFrm, int tree, string peerId, int waitTime, string Peerlist_name)
        {
            //childPeers = new List<PeerNode>();
 
            Thread.Sleep(waitTime); //time to wait for peer reconnect
            PeerInfoAccessor treeAccessor = new PeerInfoAccessor(Peerlist_name + tree);
            
            //if(!treeAccessor.load())
            //    mainFrm.rtbClientlist.BeginInvoke(new UpdateTextCallback(mainFrm.UpdatertbClientlist), new object[] { "xml load error \n" });
            while (!treeAccessor.load())
            {

                Thread.Sleep(20);
                treeAccessor.load();
            }
                         
            PeerNode p1 = new PeerNode(peerId, "deleting", 0, 0, "-1");
            while (true)
            {
                try
                {
                    

                    //recursive call
                    List<PeerNode> childPeerList = treeAccessor.getPeersByParent(peerId);

                    foreach (PeerNode child in childPeerList)
                    {
                        if (child != null)
                        {
                            mainFrm.rtbClientlist.BeginInvoke(new UpdateTextCallback(mainFrm.UpdatertbClientlist), new object[] { "Peer" + peerId + " create unregThread Peer" + child.Id + "\n" });
                            Thread unRegChild = new Thread(delegate() { new ChildUnregHandler(mainFrm, tree, child.Id, RandomNumber(waitTime, waitTime+1000), Peerlist_name); });
                            unRegChild.IsBackground = true;
                            unRegChild.Name = "ChildUnreg_Tree:" + tree + ":" + child.Id;
                            unRegChild.Start();
                            //Thread.Sleep(20);
                            mainFrm.unRegChildThread.Add(unRegChild);
                        }
                    }
                    treeAccessor.deletePeer(p1);
                    mainFrm.rtbClientlist.BeginInvoke(new UpdateTextCallback(mainFrm.UpdatertbClientlist), new object[] { "T[" + tree + "]:" + peerId + " is unregist by ChildUnregHandler\n" });
                }
                catch(Exception ex)
                {
                    mainFrm.rtbClientlist.BeginInvoke(new UpdateTextCallback(mainFrm.UpdatertbClientlist), new object[] { "ChildUnregHandler Ex:\n"+ex });
                    Thread.Sleep(40);
                    continue;
                }
                break;
            }
        }
        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private delegate void UpdateTextCallback(string message);
    }
}
