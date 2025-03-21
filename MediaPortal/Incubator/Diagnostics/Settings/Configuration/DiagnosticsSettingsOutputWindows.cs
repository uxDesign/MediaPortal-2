#region Copyright (C) 2007-2021 Team MediaPortal

/*
    Copyright (C) 2007-2021 Team MediaPortal
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
using MediaPortal.Common.Configuration.ConfigurationClasses;
using MediaPortal.Common.PathManager;
using MediaPortal.UI.SkinEngine.SkinManagement;
using MediaPortal.UiComponents.Diagnostics.Service;
using MediaPortal.UiComponents.Diagnostics.Tools;
using System;
using System.Collections.Generic;
using System.IO;

namespace MediaPortal.UiComponents.Diagnostics.Settings.Configuration
{
  internal class DiagnosticsSettingsOutputWindows : YesNo
  {
    #region Public Methods

    public override void Load()
    {
      SkinContext.Form.Invoke(new Action(()=> 
      { 
        _yes = DiagnosticsHandler.LogViewerInstance.Visible; 
      }));
    }

    public override void Save()
    {
      SkinContext.Form.Invoke(new Action(() =>
      {
        if (_yes)
        {
          if (DiagnosticsHandler.LogViewerInstance.Visible)
            return;

          string logPath = ServiceRegistration.Get<IPathManager>().GetPath("<LOG>");
          List<FileInfo> logsfiles = FileSorter.SortByLastWriteTime(logPath, "*.log");

          foreach (var file in logsfiles)
          {
            DiagnosticsHandler.LogViewerInstance.AddLog(file.FullName);
          }

          DiagnosticsHandler.LogViewerInstance.Show();
        }
        else
        {
          if (!DiagnosticsHandler.LogViewerInstance.Visible)
            return;

          DiagnosticsHandler.LogViewerInstance.Dispose();
        }
      }));
    }

    #endregion Public Methods

  }
}
