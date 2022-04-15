#region Copyright (C) 2007-2020 Team MediaPortal

/*
    Copyright (C) 2007-2020 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Exceptions;
using MediaPortal.Plugins.MP2Extended.ResourceAccess.TAS.EPG.BaseClasses;
using MediaPortal.Plugins.MP2Extended.TAS.Tv;
using MediaPortal.Plugins.SlimTv.Interfaces;
using MediaPortal.Plugins.SlimTv.Interfaces.Items;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.TAS.EPG
{
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, Summary = "")]
  [ApiFunctionParam(Name = "channelId", Type = typeof(string), Nullable = false)]
  internal class GetNextProgramOnChannel : BaseProgramDetailed
  {
    public static async Task<WebProgramDetailed> ProcessAsync(HttpContext context, string channelId)
    {
      if (!ServiceRegistration.IsRegistered<ITvProvider>())
        throw new BadRequestException("GetNextProgramOnChannel: ITvProvider not found");

      var programs = await TVAccess.GetChannelNowNextProgramAsync(context, int.Parse(channelId));
      if (programs == null)
        throw new NotFoundException(string.Format("GetNextProgramOnChannel: Couldn't get Now/Next Info for channel with Id: {0}", channelId));

      WebProgramDetailed webProgramDetailed = ProgramDetailed(programs[0]);
      return webProgramDetailed;
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
