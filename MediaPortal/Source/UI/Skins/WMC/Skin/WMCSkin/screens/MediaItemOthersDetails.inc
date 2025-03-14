<?xml version="1.0" encoding="utf-8"?>
<Grid
  xmlns="www.team-mediaportal.com/2008/mpf/directx"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:fanart="clr-namespace:MediaPortal.Extensions.UserServices.FanArtService.Client;assembly=FanArtService.Client"
  xmlns:media="clr-namespace:MediaPortal.UiComponents.Media.Models.AspectWrappers;assembly=Media">

  <Grid.RowDefinitions>
    <RowDefinition Height="Auto"/>
    <RowDefinition Height="*"/>
  </Grid.RowDefinitions>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="480"/>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>

  <Label x:Name="MediaItemName" Color="{ThemeResource TextColor}" FontSize="{ThemeResource SmallFontSize}" FontFamily="Noto-Bold"
         DataContext="{Binding Source={StaticResource FanArtBackgroundModel}}" Content="{Binding SimpleTitle}" Grid.ColumnSpan="2"
         IsVisible="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem,Converter={StaticResource ReferenceToBoolConverter}}"/>
    
  <StackPanel Orientation="Vertical" Grid.Row="1" Margin="0,0,0,5" IsVisible="{Binding Source={StaticResource FanArtBackgroundModel},Path=SelectedItem,Converter={StaticResource ReferenceToBoolConverter}}">
    <StackPanel Orientation="Vertical" DataContext="{Binding Source={StaticResource FanArtBackgroundModel}}">
      <StackPanel Orientation="Horizontal" IsVisible="{Binding Path=NumItems,Converter={StaticResource ReferenceToBoolConverter}}">
        <Label FontSize="{ThemeResource SmallFontSize}" Content="{Binding NumItems}"
               Margin="0,0,8,0" Color="{ThemeResource MenuTextBackground}" Opacity="1.3"/>
        <Label FontSize="{ThemeResource SmallFontSize}" Content="[Media.MediaItemsHeader]"
               Margin="0,0,8,0" Color="{ThemeResource MenuTextBackground}" Opacity="1.3"
               IsVisible="{Binding Path=NumItems,Converter={StaticResource ReferenceToBoolConverter}}"/>
      </StackPanel>
    </StackPanel>
    
    <media:MediaAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
      <media:MediaAspectWrapper.Template>
        <ControlTemplate>
          <StackPanel Orientation="Horizontal">
            <Label x:Name="Year" HorizontalAlignment="Left" FontSize="{ThemeResource SmallFontSize}"
                   DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}" Color="{ThemeResource MenuTextBackground}" Opacity="1.3"
                   Content="{Binding Path=RecordingTime, Converter={StaticResource DateFormatConverter}, ConverterParameter=g}"/>
          </StackPanel>
        </ControlTemplate>
      </media:MediaAspectWrapper.Template>
    </media:MediaAspectWrapper>
    
    <media:VideoStreamAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
      <media:VideoStreamAspectWrapper.Template>
        <ControlTemplate>
          <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">
            <StackPanel Orientation="Horizontal">
              <Label x:Name="Duration" FontSize="{ThemeResource SmallFontSize}" Color="{ThemeResource MenuTextBackground}" Opacity="1.3"
                     IsVisible="{Binding Path=Duration,Converter={StaticResource ReferenceToBoolConverter}}">
                <Label.Content>
                  <MultiBinding Converter="{StaticResource ExpressionMultiValueConverter}" ConverterParameter="{}{0}/60">
                    <Binding Path="Duration"/>
                  </MultiBinding>
                </Label.Content>
              </Label>
              <Label Content=" min." FontSize="{ThemeResource SmallFontSize}" Color="{ThemeResource MenuTextBackground}" Opacity="1.3"
                     IsVisible="{Binding Path=Duration,Converter={StaticResource ReferenceToBoolConverter}}"/>
            </StackPanel>
          </Grid>
        </ControlTemplate>
      </media:VideoStreamAspectWrapper.Template>
    </media:VideoStreamAspectWrapper>

    <media:UserDataWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}" HorizontalAlignment="Right">
      <media:UserDataWrapper.Template>
        <ControlTemplate>
          <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"
                IsVisible="{Binding Path=PlayPercentage,Converter={StaticResource ReferenceToBoolConverter}}">
            <Image HorizontalAlignment="Right" VerticalAlignment="Center" Stretch="Fill" Width="85" Height="85" Margin="0,-25,10,0" >
              <Image.Source>
                <Binding Path="PlayPercentage" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{}CircularBar\{0:F0}.png"/>
              </Image.Source>
            </Image>
          </Grid>
        </ControlTemplate>
      </media:UserDataWrapper.Template>
    </media:UserDataWrapper>
    
  </StackPanel>

  <media:VideoAspectWrapper Grid.Row="1" Grid.Column="1" MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
    <media:VideoAspectWrapper.Template>
      <ControlTemplate>
          <Label x:Name="MediaItemName" FontSize="{ThemeResource SmallFontSize}" Opacity="1.3" VerticalAlignment="Stretch" Margin="20,0,0,0"
                 Color="{ThemeResource MenuTextBackground}" TextTrimming="WordEllipsis" Wrap="True" Content="{Binding Path=StoryPlot}"
                 DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"/>
      </ControlTemplate>
    </media:VideoAspectWrapper.Template>
  </media:VideoAspectWrapper>

</Grid>
