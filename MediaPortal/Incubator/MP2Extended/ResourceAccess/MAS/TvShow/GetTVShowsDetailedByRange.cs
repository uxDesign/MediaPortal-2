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
using MediaPortal.Plugins.MP2Extended.Common;
using MediaPortal.Plugins.MP2Extended.Extensions;
using MediaPortal.Plugins.MP2Extended.MAS.TvShow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaPortal.Plugins.MP2Extended.Controllers.Contexts;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.TvShow
{
  internal class GetTVShowsDetailedRange
  {
    public static async Task<IList<WebTVShowDetailed>> ProcessAsync(RequestContext context, int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      var output = await GetTVShowsDetailed.ProcessAsync(context, filter, sort, order);

      // get range
      output = output.TakeRange(start, end).ToList();
      return output;
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
