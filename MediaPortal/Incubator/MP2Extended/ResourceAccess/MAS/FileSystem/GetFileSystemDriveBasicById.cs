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
using MediaPortal.Plugins.MP2Extended.Controllers.Contexts;
using MediaPortal.Plugins.MP2Extended.MAS.FileSystem;
using MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.FileSystem.BaseClasses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.FileSystem
{
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, ReturnType = typeof(WebDriveBasic), Summary = "")]
  [ApiFunctionParam(Name = "id", Type = typeof(string), Nullable = false)]
  internal class GetFileSystemDriveBasicById : BaseDriveBasic
  {
    public static Task<WebDriveBasic> ProcessAsync(RequestContext context, string id)
    {
      List<WebDriveBasic> driveList = DriveBasic();

      return Task.FromResult(driveList.Single(x => x.Id == id));
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
