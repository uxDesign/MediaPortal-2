﻿using System.Collections.Generic;
using HttpServer;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Plugins.MP2Extended.MAS.General;
using MediaPortal.Plugins.MP2Extended.TAS.Misc;
using MediaPortal.Plugins.SlimTv.Interfaces;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.TAS.Misc
{
  // TODO: not integrated into slimTvInterface
  internal class GetActiveCards : IRequestMicroModuleHandler
  {
    public dynamic Process(IHttpRequest request)
    {
      List<WebVirtualCard> output = new List<WebVirtualCard>();

      IChannelAndGroupInfo channelAndGroupInfo = ServiceRegistration.Get<ITvProvider>() as IChannelAndGroupInfo;

      return output;
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}