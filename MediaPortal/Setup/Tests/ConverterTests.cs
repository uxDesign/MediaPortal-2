#region Copyright (C) 2007-2017 Team MediaPortal

/*
    Copyright (C) 2007-2017 Team MediaPortal
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

using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using MP2BootstrapperApp.Converters;
using System.Globalization;
using System.Windows;
using Xunit;

namespace Tests
{
  public class ConverterTests
  {
    [Fact]
    public void ReturnHidden_When_PackageIsPresentAndConverterParameterIsAbsent()
    {
      // Arrange
      PackageStateToVisibilityConverter converter = new PackageStateToVisibilityConverter();
      
      // Act
      object result = converter.Convert(PackageState.Present, null, PackageState.Absent, CultureInfo.InvariantCulture);

      // Assert
      Assert.Equal(Visibility.Hidden, result);
    }
    
    [Fact]
    public void ReturnHidden_When_PackageIsAbsentAndConverterParameterIsPresent()
    {
      // Arrange
      PackageStateToVisibilityConverter converter = new PackageStateToVisibilityConverter();
      
      // Act
      object result = converter.Convert(PackageState.Absent, null, PackageState.Present, CultureInfo.InvariantCulture);

      // Assert
      Assert.Equal(Visibility.Hidden, result);
    }
    
    [Fact]
    public void ReturnVisible_When_PackageIsPresentAndConverterParameterIsPresent()
    {
      // Arrange
      PackageStateToVisibilityConverter converter = new PackageStateToVisibilityConverter();
      
      // Act
      object result = converter.Convert(PackageState.Present, null, PackageState.Present, CultureInfo.InvariantCulture);

      // Assert
      Assert.Equal(Visibility.Visible, result);
    }
    
    [Fact]
    public void ReturnVisible_When_PackageIsAbsentAndConverterParameterIsAbsent()
    {
      // Arrange
      PackageStateToVisibilityConverter converter = new PackageStateToVisibilityConverter();
      
      // Act
      object result = converter.Convert(PackageState.Absent, null, PackageState.Absent, CultureInfo.InvariantCulture);

      // Assert
      Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void ReturnNearestKilobyte_When_ConverterParameterIsKb()
    {
      // Arrange
      FileSizeUnitConverter converter = new FileSizeUnitConverter();

      // Act
      object result = converter.Convert(560315, null, "Kb", CultureInfo.InvariantCulture);

      // Assert
      Assert.Equal(547L, result);
    }

    [Fact]
    public void ReturnNearestMegabyte_When_ConverterParameterIsMb()
    {
      // Arrange
      FileSizeUnitConverter converter = new FileSizeUnitConverter();

      // Act
      object result = converter.Convert(8892141, null, "Mb", CultureInfo.InvariantCulture);

      // Assert
      Assert.Equal(8L, result);
    }

    [Fact]
    public void ReturnNearestGigabyte_When_ConverterParameterIsGb()
    {
      // Arrange
      FileSizeUnitConverter converter = new FileSizeUnitConverter();

      // Act
      object result = converter.Convert(8892141, null, "Gb", CultureInfo.InvariantCulture);

      // Assert
      Assert.Equal(8L, result);
    }
  }
}
