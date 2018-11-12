﻿using Start9.UI.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsSharp.Processes;

namespace Superbar
{
    /// <summary>
    /// Interaction logic for JumpListWindow.xaml
    /// </summary>
    public partial class JumpListWindow : CompositingWindow
    {
        string _closeOneWindowText = "Close window";
        string _closeAllWindowsText = "Close all windows";

        string _pinAppText = "Pin to Superbar";
        string _unpinAppText = "Unpin from Superbar";

        PinnedApplication _app = null;
        ProcessWindow _window = null;

        public bool IsPinned { get; set; } = false;

        public JumpListWindow()
        {
            InitializeComponent();
        }

        public void ShowWindow(PinnedApplication app, ProcessWindow window)
        {
            _app = app;
            _window = window;

            if (_app != null)
            {
                Show();
                Focus();
                Activate();

                Resources["JumpListApplicationNameText"] = _app.DiskApplication.ItemDisplayName;
                Resources["JumpListApplicationIcon"] = _app.DiskApplication.ItemSmallIcon;

                IsPinned = _app.IsPinned;

                if (_app.IsPinned)
                    Resources["JumpListPinApplicationText"] = _unpinAppText;
                else
                    Resources["JumpListPinApplicationText"] = _pinAppText;

                if (_app.OpenWindows.Count > 0)
                    CloseWindowsListViewItem.Visibility = Visibility.Visible;
                else
                    CloseWindowsListViewItem.Visibility = Visibility.Collapsed;

                if (CloseWindowsListViewItem.IsVisible)
                {
                    if (_app.OpenWindows.Count == 1)
                        Resources["JumpListCloseWindowsText"] = _closeOneWindowText;
                    else
                        Resources["JumpListCloseWindowsText"] = _closeAllWindowsText;
                }
            }
        }

        private void ApplicationListViewItem_Click(object sender, RoutedEventArgs e)
        {
            if (_app != null)
                _app.DiskApplication.Open();

            ResetSelection();
        }

        private void PinOrUnpinListViewItem_Click(object sender, RoutedEventArgs e)
        {
            if (_app != null)
            {
                if (_app.IsPinned)
                    _app.IsPinned = false;
                else
                    _app.IsPinned = true;
            }

            ResetSelection();
        }

        private void CloseWindowsListViewItem_Click(object sender, RoutedEventArgs e)
        {

            if (Config.TaskbarCombineMode == Config.CombineMode.Always)
            {
                if (_app != null)
                    foreach (ProcessWindow w in _app.OpenWindows)
                        w.Close();
            }
            else
            {
                if (_window != null)
                    _window.Close();
            }

            ResetSelection();
        }

        private void ResetSelection()
        {
            BottomSegmentListView.SelectedItem = null; //DummyListViewItem;
            Hide();
        }

        private void JumpListWindow_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void HideJumpListButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void BottomSegmentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] == ApplicationListViewItem)
                    ApplicationListViewItem_Click(sender, null);
                else if (e.AddedItems[0] == PinOrUnpinListViewItem)
                    PinOrUnpinListViewItem_Click(sender, null);
                else if (e.AddedItems[0] == CloseWindowsListViewItem)
                    CloseWindowsListViewItem_Click(sender, null);
            }
        }
    }
}