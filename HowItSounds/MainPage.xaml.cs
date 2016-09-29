using HowItSounds.Assets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Input;

namespace HowItSounds
{
    /// <summary>
    /// Let's play some sounds!
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Element> ElementsList;
        // App configuration, default error messages.
        private string configurationErrorMessage = "Configuration error!";
        private string fileErrorMessage = "File or configuration error!";
        // App configuration, where are the sound files, the pictures, and the Config.xml file.
        private string AssetElements = @"Assets/Elements/";
        private string configFile = @"Config.xml";

        public MainPage()
        {
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
            Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
        }

        private void App_Resuming(object sender, object e)
        {
            FillList();
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
           FillList();
        }

        private void FillList()
        {
            
            //Populate a list with the given configuration.
            ElementsList = new List<Element>(ElementFactory.GetElements(AssetElements, configFile));

            if (ElementsList.Count == 0)
            {
                SetTitleTextBlock(configurationErrorMessage);
            }
            else
            {
                //MyGridView's source is the populated list.
                MyGridView.ItemsSource = ElementsList;
            }
        }

        private async void MyGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Get the clicked item and set the uri to the sound file.
            var _whatToPlay = (Element)e.ClickedItem;
            var _uri = new Uri(_whatToPlay.Sound.ToString());

            // Try to set the mediaelement's source to the soundfile. Gives error, if the file is not there, or the Config.xml contains wrong information (misspelled name).
            try
            {
                var _file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(_uri);
                var _stream = await _file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                idMedia.SetSource(_stream, _file.ContentType);
            }
            catch (System.IO.FileNotFoundException) { SetTitleTextBlock(fileErrorMessage); }
        }

        private void SetTitleTextBlock(string text)
        {
            TopTitleTextBlock.Text = text;
            BottomTitleTextBlock.Text = text;
        }

        private void AppBarButtonAll_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MyGridView.ItemsSource = ElementsList;
        }

        private void AppBarPointer_Entered (object sender, PointerRoutedEventArgs e)
        {
            EnterStoryboard.Begin();
        }

        private void AppBarPointer_Exited(object sender, PointerRoutedEventArgs e)
        {
            ExitStoryboard.Begin();
        }

        private void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            args.Request.Data.SetText(TopAppBarButtonShare.Tag.ToString());
            args.Request.Data.Properties.Title = TopTitleTextBlock.Text;
        }

        private void AppBarButtonShare_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }

        private void MenuFlyoutItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            //Getting which category to look up from MenuFlyoutItem Tag properties (e.g. 1)
            var _lookUp = Convert.ToInt16(menuFlyoutItem.Tag);
            //Using the number as category reference after conversion (e.g. 1)
            MyGridView.ItemsSource = ElementsList.Where(x => x.Category == _lookUp);
        }

        private void MyGridView_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            var _panel = (ItemsWrapGrid)MyGridView.ItemsPanelRoot;

            var _actual = VisualStateGroup.CurrentState;
            int _gridColumnNumber;
            switch (_actual.Name)
            {
                case "medium":
                    {
                        _gridColumnNumber = 2;
                        break;
                    }
                case "large":
                    {
                        _gridColumnNumber = 3;
                        break;
                    }
                default:
                    {
                        _gridColumnNumber = 1;
                        break;
                    }
            }

            ////panel.ItemWidth = panel.ItemHeight = e.NewSize.Width / _gridColumnNumber;
            _panel.ItemWidth = e.NewSize.Width / _gridColumnNumber;
        }
    }
}
