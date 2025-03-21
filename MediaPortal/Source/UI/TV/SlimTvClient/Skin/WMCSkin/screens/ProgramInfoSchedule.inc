<?xml version="1.0" encoding="utf-8"?>
<DockPanel xmlns="www.team-mediaportal.com/2008/mpf/directx"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:fanart="clr-namespace:MediaPortal.Extensions.UserServices.FanArtService.Client;assembly=FanArtService.Client"
    x:Name="ProgramInfoPanel" LastChildFill="true" Height="200">

  <Grid DockPanel.Dock="Center" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Margin="0,-30,0,0">
    <Grid.RowDefinitions>
      <RowDefinition Height="Auto"/>
      <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="450"/>
      <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>

    <StackPanel Orientation="Vertical" Grid.Row="0" Grid.RowSpan="2" Grid.Column="0" >
      <Label x:Name="ProgramStartLabel" Margin="0,0,0,0"  Color="{ThemeResource TextColor}" FontSize="{ThemeResource SmallFontSize}"
             Content="{Binding Path=StartTime, Converter={StaticResource SlimTvDateFormatConverter}}"/>
      <Label x:Name="ProgramEndLabel" Content="{Binding Path=EndTime, Converter={StaticResource SlimTvDateFormatConverter}}"
             Color="{ThemeResource TextColor}" FontSize="{ThemeResource SmallFontSize}"/>
      <Label x:Name="GenreLabel" Content="{Binding Path=CurrentProgram.Genre}" Color="{ThemeResource MenuTextBackground}" Opacity="1.2" 
             FontSize="{ThemeResource SmallFontSize}" IsVisible="{Binding Path=CurrentProgram.Genre,Converter={StaticResource ReferenceToBoolConverter}}"/>
      <Label x:Name="ChannelLabel" Content="{Binding ChannelName}" Color="{ThemeResource MenuTextBackground}"
             Opacity="1.2" FontSize="{ThemeResource SmallFontSize}"/>
      <Image x:Name="ChannelLogo" Width="{ThemeResource ChannelLogoLarge}" Margin="0,10,0,0"
             Stretch="Uniform" HorizontalAlignment="Left" VerticalAlignment="Top" FontSize="{ThemeResource SmallFontSize}" >
        <Image.Source>
          <fanart:FanArtImageSource fanart:FanArtMediaType="{Binding ChannelLogoType}" fanart:FanArtType="Banner"
                                    fanart:FanArtName="{Binding ChannelName}" fanart:MaxWidth="0" fanart:MaxHeight="0"/>
        </Image.Source>
      </Image>
    </StackPanel>

    <Grid Grid.Row="0" Grid.Column="1" >
      <Grid.RowDefinitions>
        <RowDefinition Height="*"/>
      </Grid.RowDefinitions>
      <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto"/>
        <ColumnDefinition Width="*"/>
      </Grid.ColumnDefinitions>
      <Label x:Name="Program" Scroll="Auto" FontFamily="Noto-Bold" HorizontalAlignment="Stretch" VerticalAlignment="Stretch"
             Content="{Binding CurrentProgram.Title}" Margin="10,0,0,0" Grid.Column="0" FontSize="{ThemeResource SmallFontSize}"
             Color="{ThemeResource TextColor}"/>
      <Label x:Name="SeriesLabel" Content="{Binding CurrentProgram.Series}" Opacity="1.3" Grid.Column="1" FontSize="{ThemeResource SmallFontSize}"
             Color="{ThemeResource MenuTextBackground}" Margin="10,0,5,0" TextTrimming="WordEllipsis"/>
    </Grid>

    <Label Grid.Row="1" Grid.Column="1" x:Name="Description" HorizontalAlignment="Stretch" VerticalAlignment="Stretch"
           Wrap="True" TextTrimming="WordEllipsis" Opacity="1.3" FontSize="{ThemeResource SmallFontSize}"
           Content="{Binding Path=CurrentProgram.Description}"
           Color="{ThemeResource MenuTextBackground}" Margin="10,0,10,5" />
  </Grid>
</DockPanel>
