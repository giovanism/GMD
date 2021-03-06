﻿using GMD.ViewModels;
using GMD.Views;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GMD
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();            
            MenuFrame.Navigate(typeof(SearchPage));

            var update = new Action(() =>
            {
                // update radiobuttons after frame navigates
                var type = MenuFrame.CurrentSourcePageType;
                foreach (var radioButton in AllRadioButtons(this))
                {
                    var target = radioButton.CommandParameter as NavType;
                    if (target == null)
                        continue;
                    radioButton.IsChecked = target.Type.Equals(type);
                }
                switch (HamburgerSplitView.DisplayMode)
                {
                    case SplitViewDisplayMode.CompactOverlay:
                    case SplitViewDisplayMode.Overlay:
                        HamburgerSplitView.IsPaneOpen = false;
                        break;
                    case SplitViewDisplayMode.CompactInline:
                    default:
                        break;
                        
                }
                // BackCommand.RaiseCanExecuteChanged();
            });
            MenuFrame.Navigated += (s, e) => update();
            Loaded += (s, e) => update();
            DataContext = this;

            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility
                = Windows.UI.Core.AppViewBackButtonVisibility.Visible;

            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                if (MenuFrame.CanGoBack)
                {
                    if (!e.Handled)
                    {
                        e.Handled = true;
                        MenuFrame.GoBack();
                    }                    
                }
            };
        }

        private void HamburgerMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteMenu();
        }
        private void ExecuteMenu()
        {
            HamburgerSplitView.IsPaneOpen = !HamburgerSplitView.IsPaneOpen;
        }

        // nav
        Command<NavType> _navCommand;
        public Command<NavType> NavCommand { get { return _navCommand ?? (_navCommand = new Command<NavType>(ExecuteNav)); } }
        private void ExecuteNav(NavType navType)
        {
            MenuFrame.Navigate(navType.Type);
            
            // when we nav home, clear history
            if (navType.Type.Equals(typeof(SearchPage)))
                MenuFrame.BackStack.Clear();

            // navigate only to new pages
            if (MenuFrame.Content != null && MenuFrame.Content.GetType() != navType.Type)
                MenuFrame.Navigate(navType.Type, navType.Parameter);
        }

        // utility
        public List<RadioButton> AllRadioButtons(DependencyObject parent)
        {
            var list = new List<RadioButton>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is RadioButton)
                {
                    list.Add(child as RadioButton);
                    continue;
                }
                list.AddRange(AllRadioButtons(child));
            }
            return list;
        }

        // prevent check
        private void DontCheck(object s, RoutedEventArgs e)
        {
            // don't let the radiobutton check
            (s as RadioButton).IsChecked = false;
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {

        }
    }

    public class NavType
    {
        public Type Type { get; set; }
        public string Parameter { get; set; }
    }

}
