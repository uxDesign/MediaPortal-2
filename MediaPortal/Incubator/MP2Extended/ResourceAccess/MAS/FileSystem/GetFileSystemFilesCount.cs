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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Common;
using MediaPortal.Plugins.MP2Extended.MAS.FileSystem;
using MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.FileSystem.BaseClasses;
using MediaPortal.Plugins.MP2Extended.Utils;
using MediaPortal.Plugins.MP2Extended.Controllers.Contexts;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.FileSystem
{
  internal class GetFileSystemFilesCount : BaseFileBasic
  {
    [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, ReturnType = typeof(WebIntResult), Summary = "")]
    [ApiFunctionParam(Name = "id", Type = typeof(string), Nullable = false)]
    public static Task<WebIntResult> ProcessAsync(RequestContext context, string id)
    {
      List<WebFileBasic> output = new List<WebFileBasic>();
      string path = Base64.Decode(id);
      if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
      {
        output = new DirectoryInfo(path).GetFiles().Select(file => FileBasic(file)).ToList();
      }

      return Task.FromResult(new WebIntResult { Result = output.Count });
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
