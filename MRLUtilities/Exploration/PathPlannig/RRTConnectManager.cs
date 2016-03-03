using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MRL.Commons;
using MRL.CustomMath;
using MRL.Mapping;

namespace MRL.Exploration.PathPlannig
{
    //public class RRTParameters
    //{
    //    public Pose2D sourcePose { get; set; }
    //    public Pose2D goalPose { get; set; }
    //    public int robotIndex { get; set; }
    //}
    //public class RRTConnectManager
    //{
    //    private RRTConnect pathPlannig = null;
    //    private BlockingCollection<RRTParameters> requestCollection = null;
    //    private CancellationTokenSource cts = null;
    //    private EGMap egMap = null;
    //    public delegate void ReceivedPathDlg(List<Pose2D> pathList, int robotIndex);
    //    public event ReceivedPathDlg ReceivedPath_event;
    //    public void Start(EGMap currMap)
    //    {
    //        try
    //        {
    //            this.egMap = currMap;

    //            requestCollection = new BlockingCollection<RRTParameters>();
    //            pathPlannig = new RRTConnect();
    //            pathPlannig.Init(egMap);
                
    //            cts = new CancellationTokenSource();
    //            Task.Factory.StartNew(() =>
    //            {
    //                foreach (var item in requestCollection.GetConsumingEnumerable())
    //                {
    //                    if (cts.Token.IsCancellationRequested)
    //                        break;

    //                    List<Pose2D> plist = null;

    //                    for (int i = 0; i < 5; i++)
    //                    {
    //                        plist = pathPlannig.FindPathRRTConnect(item.sourcePose, item.goalPose, 500);
    //                        if (plist != null)
    //                            break;
    //                    }

    //                    if (plist != null)
    //                        if (ReceivedPath_event != null)
    //                            ReceivedPath_event(plist, item.robotIndex);
    //                }
    //            }, cts.Token);

    //        }
    //        catch (Exception)
    //        {
    //            ProjectCommons.writeConsoleMessage("Error In RRTConnectManager->StartThread", Utils.ConsoleMessageType.Error);
    //        }
    //    }

    //    public void Stop()
    //    {
    //        cts.Cancel();
    //    }

    //    public void AddPathPlannigRequest(RRTParameters rrtPamas)
    //    {
    //        if (requestCollection != null)
    //            requestCollection.Add(rrtPamas);
    //    }
    //}
}
