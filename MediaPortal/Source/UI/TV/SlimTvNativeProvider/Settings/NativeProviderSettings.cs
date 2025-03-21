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

using MediaPortal.Common.Settings;

namespace MediaPortal.Plugins.SlimTv.Providers.Settings
{
  public class NativeProviderSettings
  {
    /// <summary>
    /// Holds the last selected channel Tv group ID.
    /// </summary>
    [Setting(SettingScope.User)]
    public int LastChannelGroupId { get; set; }

    /// <summary>
    /// Holds the last selected Tv channel ID.
    /// </summary>
    [Setting(SettingScope.User)]
    public int LastChannelId { get; set; }

    /// <summary>
    /// Holds the last selected radio channel group ID.
    /// </summary>
    [Setting(SettingScope.User)]
    public int LastRadioChannelGroupId { get; set; }

    /// <summary>
    /// Holds the last selected radio channel ID.
    /// </summary>
    [Setting(SettingScope.User)]
    public int LastRadioChannelId { get; set; }
  }
}
