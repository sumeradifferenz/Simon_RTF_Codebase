using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;
using Plugin.Settings;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Simon.Helpers;
using Simon.Interfaces;
using Simon.Models;
using Simon.ServiceHandler;
using Simon.Views;
using Simon.Views.Popups;
using Xamarin.Forms;

namespace Simon.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        JObject jObject = null;
        public BaseViewModel()
        {
            FooterItems = SessionService.BaseFooterItems;
        }

        private string _ScreenTitle { get; set; }
        public string ScreenTitle
        {
            get
            {
                return _ScreenTitle;
            }
            set
            {
                _ScreenTitle = value;
                App.ScreenTitle = value;
                OnPropertyChanged(nameof(ScreenTitle));
            }
        }

        private string _headerTitle = "";
        public string HeaderTitle
        {
            get { return _headerTitle; }
            set { SetProperty(ref _headerTitle, value); }
        }

        private string _headerLeftImage = "";
        public string HeaderLeftImage
        {
            get { return _headerLeftImage; }
            set { SetProperty(ref _headerLeftImage, value); }
        }

        // here's your shared IsBusy property
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                // again, this is very important
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        // this little bit is how we trigger the PropertyChanged notifier.
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backfield, T value,
            [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backfield, value))
            {
                return false;
            }
            backfield = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        /// <summary>
        /// ShowLoader
        /// </summary>
        /// <param name="animate">Pass true/false for animate the popup view</param>
        /// <returns></returns>
        public async Task ShowLoader(bool animate = false)
        {
            try
            {
                await PopupNavigation.Instance.PushAsync(new LoadingPopup(), animate);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        /// <summary>
        /// Hides loading popup page
        /// </summary>
        public ICommand ClosePopupCommand { get { return new Command(ClosePopupFromCommand); } }
        public async void ClosePopupFromCommand()
        {
            await ClosePopup(false);
        }

        /// <summary>
        /// ClosePopup
        /// </summary>
        /// <param name="animate">Pass true/false for animate the popup view</param>
        /// <returns></returns>

        public static async Task ShowPopup(PopupPage popup, bool animate = false, bool closePreviousPopup = true)
        {
            try
            {
                if (closePreviousPopup)
                    await ClosePopup();

                await PopupNavigation.Instance.PushAsync(popup, animate);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public ICommand BackCommand { get { return new Command(Back); } }
        public void Back()
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    int NavigationStackCount = App.Current.MainPage.Navigation.NavigationStack.Count;
                    if (NavigationStackCount > 1)
                    {
                        var PreviousScreen = App.Current.MainPage.Navigation.NavigationStack[NavigationStackCount - 2];
                        if (PreviousScreen != null)
                        {
                            //App.ScreenTitle = ((BaseViewModel)PreviousScreen.BindingContext).;
                            //App.CurrentMenuTitle = string.Empty;
                        }
                    }
                    await App.Current.MainPage.Navigation.PopAsync(false);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public ICommand LogOutCommand { get { return new Command(Logout_Click); } }
        private async void Logout_Click()
        {
            if (string.IsNullOrEmpty(Settings.DeviceToken))
            {
                App.tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogFile.txt");
                File.AppendAllText(App.tempFile, "\n\nLogout without Device token....");
                Debug.WriteLine("File Name====" + App.tempFile);

                Settings.LoggedInUser = null;
                App.SelectedUserData = null;
                Application.Current.MainPage = new NavigationPage(new LoginPage()) { BarTextColor = Color.Black, BarBackgroundColor = Color.White };
                return;
            }
            else
            {
                string sessionToken = Settings.SessionToken.ToUpper();
                var client = new HttpClient();
                string url = Config.LOGOUT_API + sessionToken;
                var content1 = new StringContent(JsonConvert.SerializeObject(jObject), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content1);

                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Content == null)
                {
                    await ClosePopup();
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ShowAlert("Data Not Sent!!", string.Format("Response contained status code: {0}", response.StatusCode));
                    });
                }
                else
                {
                    Settings.LoggedInUser = null;
                    App.SelectedUserData = null;
                    Application.Current.MainPage = new NavigationPage(new LoginPage()) { BarTextColor = Color.Black, BarBackgroundColor = Color.White };
                }
            }
        }

        public static async Task ClosePopup(bool animate = false)
        {
            try
            {
                if (PopupNavigation.Instance.PopupStack.Count > 0)
                {
                    await PopupNavigation.Instance.PopAsync(animate);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public void ShowExceptionAlert(Exception ex)
        {
            App.Current.MainPage.DisplayAlert("Exception", ex.Message, "OK");
        }

        public async Task ShowAlert(string title, string message)
        {
            await App.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        public async Task ShowAlertWithAction(string title, string message, string action1, string action2)
        {
            var action = await App.Current.MainPage.DisplayAlert(title, message, action1, action2);
            if (action)
            {
                Back();
            }
        }

        public ICommand FooterNavigationCommand => new Command<FooterModel>(FooterNavigationClick);
        private void FooterNavigationClick(FooterModel footer)
        {
            try
            {
                FooterNavigation(footer);
            }
            catch (Exception exception)
            {
                Console.Write(exception);
            }
        }

        public void FooterNavigation(FooterModel selectedItem)
        {
            try
            {
                //if (App.SelectedFooterItem == selectedItem) {
                //    return;
                //}

                CrossSettings.Current.Remove(Settings.ApprovePageSelectedTabKey);

                SessionService.SelectedFooterItem = selectedItem;
                SessionService.BaseFooterItems.All((arg) =>
                {
                    if (arg.Id == selectedItem.Id)
                    {
                        arg.IsSelected = true;
                    }
                    else
                    {
                        arg.IsSelected = false;
                    }
                    return true;
                });
                NavigateToTab(selectedItem.Id);
            }
            catch (Exception exception)
            {
                Debug.Write(exception.Message);
            }
        }

        public void NavigateToTab(int id)
        {
            try
            {
                switch (id)
                {
                    case 0:
                        Application.Current.MainPage = new NavigationPage(new LandingPage()) { BarTextColor = Color.Black };
                        break;
                    case 1:
                        Application.Current.MainPage = new NavigationPage(new DealsPage()) { BarTextColor = Color.Black };
                        break;
                    case 2:
                        Application.Current.MainPage = new NavigationPage(new MessagesPage()) { BarTextColor = Color.Black };
                        break;
                    case 3:
                        Application.Current.MainPage = new NavigationPage(new AssentMainPage()) { BarTextColor = Color.Black };
                        break;
                }
                //Application.Current.MainPage = new NavigationPage(new CarouselMainPage(id)) { BarTextColor = Color.Black };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public ObservableCollection<FooterModel> _footerItems = new ObservableCollection<FooterModel>();
        public ObservableCollection<FooterModel> FooterItems
        {
            get { return _footerItems; }
            set
            {
                SetProperty(ref _footerItems, value);
            }
        }

        public async void ImagePicker(Action<string, MediaFile> result)
        {
            try
            {
                MediaFile mediaFile = new MediaFile(null, null);
                var action = "";
                action = await App.Current.MainPage.DisplayActionSheet(Constants.CameraOptionTitle.ToUpper(), Constants.CancelCapsText, null, Constants.TakePhotoOption.ToUpper(), Constants.PickPhotoOption.ToUpper());

                if (action.ToUpper() == Constants.TakePhotoOption.ToUpper())
                {
                    var status = await RuntimePermission.RuntimePermissionStatus(Permission.Camera);
                    var Storegestatus = await RuntimePermission.RuntimePermissionStatus(Permission.Storage);

                    if (status == PermissionStatus.Granted && Storegestatus == PermissionStatus.Granted)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                            {
                                await App.Current.MainPage.DisplayAlert(Constants.WarningText, Constants.NoCameraAvailableMsg, Constants.OkText);
                                return;
                            }

                            await ClosePopup();
                            mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                            {
                                Directory = "Simon",
                                SaveToAlbum = true,
                                Name = "SIMON.jpg",
                                CompressionQuality = 70,
                                PhotoSize = PhotoSize.Medium,
                                AllowCropping = true
                            });
                            await ClosePopup();
                            if (mediaFile != null && !string.IsNullOrEmpty(mediaFile.Path))
                            {
                                new ImageCropper()
                                {
                                    CropShape = ImageCropper.CropShapeType.Rectangle,
                                    Success = (imageFile) =>
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            if (string.IsNullOrEmpty(imageFile))
                                            {
                                                result.Invoke(string.Empty, null);
                                            }
                                            else
                                            {
                                                result.Invoke(imageFile, mediaFile);
                                            }

                                        });
                                    }
                                }.Show(mediaFile.Path);
                            }
                            else
                            {
                                result.Invoke(null, null);
                            }
                        });
                    }
                    else if (status != PermissionStatus.Unknown)
                    {
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            var res = await App.Current.MainPage.DisplayAlert(Constants.WarningText, Constants.CameraPermissionDeniedMsg.ToUpper(), Constants.PermissionGrantedText, Constants.OkText);
                            if (res)
                            {
                                DependencyService.Get<IOpenSetting>().OpenAppSetting();
                            }
                            result.Invoke(null, null);
                        }
                    }
                    else
                    {
                        result.Invoke(null, null);
                    }
                }
                else if (action.ToUpper() == Constants.PickPhotoOption.ToUpper())
                {
                    var PhotosStatus = await RuntimePermission.RuntimePermissionStatus(Permission.Photos);
                    var Storegestatus = await RuntimePermission.RuntimePermissionStatus(Permission.Storage);
                    if (PhotosStatus == PermissionStatus.Granted && Storegestatus == PermissionStatus.Granted)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (!CrossMedia.Current.IsPickPhotoSupported)
                            {
                                await App.Current.MainPage.DisplayAlert(Constants.WarningText, Constants.NoCameraAvailableMsg.ToUpper(), Constants.OkText);
                                return;
                            }

                            mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                            {
                                CompressionQuality = 70,
                                PhotoSize = PhotoSize.Medium,
                                SaveMetaData = false,
                            });
                            //result.Invoke(mediaFile);
                            await ClosePopup();

                            if (mediaFile != null && !string.IsNullOrEmpty(mediaFile.Path))
                            {
                                new ImageCropper()
                                {
                                    CropShape = ImageCropper.CropShapeType.Rectangle,
                                    Success = (imageFile) =>
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            if (string.IsNullOrEmpty(imageFile))
                                            {
                                                result.Invoke(string.Empty, null);
                                            }
                                            else
                                            {
                                                result.Invoke(imageFile, mediaFile);
                                            }

                                        });
                                    }
                                }.Show(mediaFile.Path);
                            }
                            else
                            {
                                result.Invoke(null, null);
                            }

                        });
                    }
                    else if (PhotosStatus != PermissionStatus.Unknown)
                    {
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            var res = await App.Current.MainPage.DisplayAlert(Constants.WarningText, Constants.PhotosPermissionDeniedMsg.ToUpper(), Constants.PermissionGrantedText, Constants.OkText);
                            if (res)
                            {
                                DependencyService.Get<IOpenSetting>().OpenAppSetting();
                            }
                            result.Invoke(null, null);
                        }
                    }
                    else
                    {
                        result.Invoke(null, null);
                    }
                }
                else
                {
                    result.Invoke(null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                result.Invoke(null, null);
            }
        }

        public static async void DocumentPicker(Action<(string name, string FileBase64String, string FileType, string FileName)> result)
        {
            FileData fileData = new FileData();

            var status = await RuntimePermission.RuntimePermissionStatus(Permission.Storage);

            if (status == PermissionStatus.Granted)
            {
                fileData = await CrossFilePicker.Current.PickFile();

                if (fileData != null)
                {
                    byte[] data = fileData.DataArray;

                    var FileBase64String = Convert.ToBase64String(data);
                    string FileName = fileData.FileName;
                    string name = (fileData.FilePath != null) ? Path.GetFileNameWithoutExtension(fileData.FilePath) : string.Empty;
                    string type = Path.GetExtension(fileData.FilePath);

                    var supportedTypes = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx" };
                    var fileExt = Path.GetExtension(fileData.FileName).Substring(1);

                    if (!supportedTypes.Contains(fileExt))
                    {
                        await App.Current.MainPage.DisplayAlert(Constants.WarningText, "File Extension Is InValid - Only Upload WORD/PDF/EXCEL/TXT File", Constants.OkText);
                        result.Invoke((null, null, null, null));
                    }
                    else
                    {
                        result.Invoke((name, FileBase64String, type, FileName));
                    }
                }
            }
            else if (status != PermissionStatus.Unknown)
            {
                await App.Current.MainPage.DisplayAlert(Constants.WarningText, Constants.StoragePermissionDeniedMsg.ToUpper(), Constants.PermissionGrantedText, Constants.OkText);
            }
        }

        public void ConvertIntoByte(string image)
        {
            try
            {
                byte[] imageData = File.ReadAllBytes(image);

                //string url = "";

                //url = Constants.BaseServiceURL + Constants.AddExericseImageURL;

                //create new HttpClient and MultipartFormDataContent and add our file
                string boundary = "---8d0f01e6b3b5dafaaadaad";
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 10, 0);
                MultipartFormDataContent multipartContent = new MultipartFormDataContent(boundary);
                ByteArrayContent baContent = new ByteArrayContent(imageData);

                baContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                baContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "Upload",
                    FileName = Path.GetFileName(image)
                };

                multipartContent.Add(baContent);

                //upload MultipartFormDataContent content async and store response in response var
                //var response = await client.PostAsync(url + "/" + ExerciseId, multipartContent);

                //read response result as a string async into json var
                //var result = response.Content.ReadAsStringAsync().Result;
                //Debug.WriteLine("\nResult : " + result);
                //var resultobject = JsonConvert.DeserializeObject<AddExericseImageResponseModel>(result);
                //return resultobject;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}


