using Plugin.Media;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace PrismDePhoto.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private string _fileLocation;
        public string FileLocation
        {
            get { return _fileLocation; }
            set { SetProperty(ref _fileLocation, value); }
        }

        private ImageSource _photoData;
        public ImageSource PhotoData
        {
            get { return _photoData; }
            set { SetProperty(ref _photoData, value); }
        }

        private IPageDialogService _pageDialogService;
        public ICommand TakeClickedCommand { get; }
        public ICommand PickClickedCommand { get; }



        public MainPageViewModel(IPageDialogService pageDialogService)
        {
            _pageDialogService = pageDialogService;
            TakeClickedCommand = new DelegateCommand(TakeClicked);
            PickClickedCommand = new DelegateCommand(PickClicked);

            FileLocation = "n/a";
            PhotoData = null;
        }



        async private void TakeClicked()
        {
            await CrossMedia.Current.Initialize();

            //throw new NotImplementedException();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await _pageDialogService.DisplayAlertAsync("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;

            //DisplayAlert("File Location", file.Path, "OK");
            FileLocation = string.Format($"File location: {file.Path}");

            PhotoData = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }


        async private void PickClicked()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await _pageDialogService.DisplayAlertAsync("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }
            var file = await CrossMedia.Current.PickPhotoAsync();


            if (file == null)
                return;

            PhotoData = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }

    }
}
