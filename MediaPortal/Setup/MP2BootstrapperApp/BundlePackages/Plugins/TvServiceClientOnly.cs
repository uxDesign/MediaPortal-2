﻿#region Copyright (C) 2007-2021 Team MediaPortal

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

namespace MP2BootstrapperApp.BundlePackages.Plugins
{
  public class TvServiceClientOnly : PluginDescriptor
  {
    protected static readonly string[] OPTIONAL_FEATURES = new[] { FeatureId.SlimTvClient };
    protected static readonly string[] EXCLUDED_PARENTS = new string[] { FeatureId.Server };
    protected static readonly string[] CONFLICTING_PLUGINS = new string[] { /*PluginId.TvMosaic*/ };

    /// <summary>
    /// Plugin that installs the necessary client features for using TV Server
    /// </summary>
    public TvServiceClientOnly()
      : base(PluginId.TvServiceClient, "TV Service Client", "b-1", true, FeatureId.SlimTvNativeProvider, OPTIONAL_FEATURES, EXCLUDED_PARENTS, CONFLICTING_PLUGINS)
    { }
  }
}
