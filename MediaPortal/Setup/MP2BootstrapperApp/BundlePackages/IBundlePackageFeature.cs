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

using System.Collections.Generic;
using WixToolset.Dtf.WindowsInstaller;
using WixToolset.Mba.Core;

namespace MP2BootstrapperApp.BundlePackages
{
  public interface IBundlePackageFeature
  {
    /// <summary>
    /// The unique identifier for a feature.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The parent package id.
    /// </summary>
    string Package { get; }

    /// <summary>
    /// The parent feature id; or <see cref="string.Empty"/> if this feature does not have a parent.
    /// </summary>
    string Parent { get; }

    /// <summary>
    /// The install level of the feature.
    /// </summary>
    int InstallLevel { get; }

    /// <summary>
    /// The title of the feature.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// The description of the feature.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The installled size of the feature in bytes.
    /// </summary>
    long InstalledSize { get; }

    /// <summary>
    /// The attributes specified for the feature.
    /// </summary>
    FeatureAttributes Attributes { get; }

    /// <summary>
    /// Whether a previous version of this feature is installed in a previous version of the parent package.
    /// </summary>
    bool PreviousVersionInstalled { get; set; }

    /// <summary>
    /// The current state of the feature if the current version of the parent package is installed. 
    /// </summary>
    FeatureState CurrentFeatureState { get; set; }

    /// <summary>
    /// Ids of any related features that should optionally be installed alongside this feature. These features
    /// should only be installed if their main parent feature is also being installed and they do not conflict with any
    /// other features that are being installed, e.g. if this contains client features and the client is not being
    /// installed, then those features will not be installed.
    /// </summary>
    ICollection<string> RelatedFeatures { get; }

    /// <summary>
    /// Any features that conflict with this feature, conflicting features should not be installed alongside each other.
    /// </summary>
    ICollection<string> ConflictingFeatures { get; }
  }
}
