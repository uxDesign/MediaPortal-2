<?xml version="1.0" encoding="utf-8"?>
<Grid
    HorizontalAlignment="Stretch" VerticalAlignment="Stretch"
    xmlns="www.team-mediaportal.com/2008/mpf/directx"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:mp_special_behavior="clr-namespace:MediaPortal.UI.SkinEngine.SpecialElements.Behaviors;assembly=SkinEngine"
    DataContext="{Binding Source={Model 4CDD601F-E280-43b9-AD0A-6D7B2403C856},Path=NavigationData.CurrentScreenData}"
    > <!-- Model = MediaNavigationModel -->
  <Grid.Resources>
    <Model x:Key="MediaModel" Id="4CDD601F-E280-43b9-AD0A-6D7B2403C856"/>
  </Grid.Resources>
  <Grid.RowDefinitions>
    <RowDefinition Height="*"/>
  </Grid.RowDefinitions>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="Auto"/>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>

  <DockPanel DataContext="{Binding Source={Model 4CDD601F-E280-43b9-AD0A-6D7B2403C856},Path=NavigationData.CurrentScreenData}"
       VerticalAlignment="Top" HorizontalAlignment="Left" LastChildFill="false" Margin="18,-125,0,0" Grid.ColumnSpan="2"
       IsVisible="{Binding IsItemsValid}">
    <!---Grid+CoverView-->
    <StackPanel DockPanel.Dock="Left" Orientation="Horizontal">
      <Label HorizontalAlignment="Center" VerticalAlignment="Center" Margin="0,0,10,0" FontFamily="Noto-Bold"
             Color="{ThemeResource TextColor}" FontSize="{ThemeResource SmallFontSize}">
        <Label.Content>
          <MultiBinding Converter="{StaticResource GetIndexMultiConverter}" ConverterParameter="1">
            <Binding Source="{StaticResource FanArtBackgroundModel}" Path="SelectedItem"/>
            <Binding Source="{Binding ElementName=MediaItemsList}" Path="ItemsSource"/>
          </MultiBinding>
        </Label.Content>
      </Label>
      <Label Color="{ThemeResource TextColor}" Content="| " FontSize="{ThemeResource SmallFontSize}" Opacity="1"/>
      <Label Color="{ThemeResource TextColor}" Content="{Binding NumItems}" FontSize="{ThemeResource SmallFontSize}" Opacity="1"/>
    </StackPanel>
  </DockPanel>
  
  <Grid HorizontalAlignment="Stretch" VerticalAlignment="Stretch" IsVisible="{Binding IsItemsValid}">
    <Grid.RowDefinitions>
      <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="Auto"/>
    </Grid.ColumnDefinitions>

    <!-- All KeyBindings are included from shared resource -->
    <Include Source="screens\MediaItemsFilterKeyBindings.inc"/>
    
    <Grid.Margin>
      <MultiBinding Converter="{StaticResource ExpressionMultiValueConverter}" ConverterParameter="{}{0} == {LayoutType}.{ListLayout} ? {1} : ({0} == {LayoutType}.{GridLayout} ? {2} : {3})">
        <Binding Source="{StaticResource ViewModeModel}" Path="LayoutType"/>
        <Binding Source="-83,0,200,84"/>
        <Binding Source="-83,0,400,41"/>
        <Binding Source="-118,0,-100,41"/>
      </MultiBinding>
    </Grid.Margin>

    <Image Source="win10_bg.png" Opacity="0.75" Margin="-5,-13,-5,0" Stretch="Fill">
      <Image.Width>
      <MultiBinding Converter="{StaticResource ExpressionMultiValueConverter}" ConverterParameter="{}{0} == {LayoutType}.{ListLayout} ? {1} : ({2}+10)">
        <Binding Source="{StaticResource ViewModeModel}" Path="LayoutType"/>
        <Binding Source="610"/>
        <Binding Source="{Binding ElementName=MediaItemsList, Path=ActualWidth}"/>
      </MultiBinding>
      </Image.Width>
      <Image.IsVisible>
        <Binding Source="{StaticResource ViewModeModel}" Path="LayoutType" Converter="{StaticResource ExpressionValueConverter}" ConverterParameter="{}{0} != {LayoutType}.{CoverLayout}"/>
      </Image.IsVisible>
    </Image>
    
     <ListView x:Name="MediaItemsList" Style="{ThemeResource MediaItemsListListViewStyle}" ItemsSource="{Binding Items}" RestoreFocus="True" Margin="2,22,2,0"
               HorizontalAlignment="Stretch" VerticalAlignment="Stretch" IsVisible="{Binding ShowList}"
               mp_special_behavior:SelectedItemBehavior.FocusSelectedItem="True">
      <ListView.Resources>
        <Command x:Key="Menu_Command" Source="{StaticResource MediaModel}" Path="Select"
            Parameters="{LateBoundValue BindingValue={Binding}}"/>
      </ListView.Resources>
    </ListView>

    <Label IsVisible="{Binding ShowListHint}"
        HorizontalAlignment="Center" VerticalAlignment="Center" Wrap="true"
        Content="{Binding ListHint}" Color="{ThemeResource HintTextColor}"/>
  </Grid>
  <Label IsVisible="{Binding !IsItemsValid}"
      HorizontalAlignment="Center" VerticalAlignment="Center"
      Content="[Media.ViewInvalid]" Color="{ThemeResource ErrorTextColor}"/>
</Grid>
