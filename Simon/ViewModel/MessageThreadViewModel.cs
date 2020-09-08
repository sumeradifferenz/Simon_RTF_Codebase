﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media.Abstractions;
using Simon.Helpers;
using Simon.Interfaces;
using Simon.Models;
using Simon.Views;
using Simon.Views.Popups;
using Syncfusion.XForms.RichTextEditor;
using Xamarin.Forms;
using ImageSource = Xamarin.Forms.ImageSource;

namespace Simon.ViewModel
{
    public class MessageThreadViewModel : BaseViewModel
    {
        private bool _isImageVisible { get; set; } = false;
        public bool isImageVisible
        {
            get { return _isImageVisible; }
            set
            {
                _isImageVisible = value;
                OnPropertyChanged(nameof(isImageVisible));
            }
        }

        private bool _isDocsVisible { get; set; } = false;
        public bool isDocsVisible
        {
            get { return _isDocsVisible; }
            set
            {
                _isDocsVisible = value;
                OnPropertyChanged(nameof(isDocsVisible));
            }
        }

        string userId, threadId, name;
        bool bookMarks;
        public bool sendEnable;
        public string SaveMessage, base64String;
        int id;
        string message;
        JObject jObject = null;
        private ObservableCollection<messages> _messageList = new ObservableCollection<messages>();
        private ObservableCollection<messageUsers> _threadList = new ObservableCollection<messageUsers>();
        private ObservableCollection<messages> AllItems = new ObservableCollection<messages>();
        public ICommand PersonDetailsCommand { get; set; }
        public ICommand ReplayCommand { get; set; }
        public ICommand LoadMoreCommand { get; private set; }
        public ICommand ImageCommand { get; set; }

        private bool _isLoadingInfinite = false;
        private int _totalRecords = 0;
        private bool _isLoadingInfiniteEnabled = false;
        private int _CurrentPage = 1;
        private int _LastPage = 0;
        private bool _isTeamLoading = false;

        private ObservableCollection<messageUsers> _messageUserList = new ObservableCollection<messageUsers>();
        public ObservableCollection<messageUsers> messageUserList
        {
            get { return _messageUserList; }
            set
            {
                _messageUserList = value;
                OnPropertyChanged(nameof(messageUserList));
            }
        }

        private string _labelParty;
        public string LabelParty
        {
            get { return _labelParty; }
            set { SetProperty(ref _labelParty, value); }
        }

        private string _labelTopic;
        public string LabelTopic
        {
            get { return _labelTopic; }
            set { SetProperty(ref _labelTopic, value); }
        }

        public string _TypedMessage { get; set; }
        public string TypedMessage
        {
            get { return _TypedMessage; }
            set
            {
                _TypedMessage = value;
                OnPropertyChanged(TypedMessage);
            }
        }

        public string _Link { get; set; }
        public string Link
        {
            get { return _Link; }
            set
            {
                _Link = value;
                OnPropertyChanged(Link);
            }
        }

        public string _ParticipantName { get; set; }
        public string ParticipantName
        {
            get { return _ParticipantName; }
            set
            {
                _ParticipantName = value;
                OnPropertyChanged(ParticipantName);
            }
        }

        private bool _IsDataNotAvailable { get; set; } = false;
        public bool IsDataNotAvailable
        {
            get { return _IsDataNotAvailable; }
            set
            {
                _IsDataNotAvailable = value;
                OnPropertyChanged(nameof(IsDataNotAvailable));
            }
        }

        private bool _IsListDataAvailable { get; set; } = false;
        public bool IsListDataAvailable
        {
            get { return _IsListDataAvailable; }
            set
            {
                _IsListDataAvailable = value;
                OnPropertyChanged(nameof(IsListDataAvailable));
            }
        }

        private DateTime _msgCreatedDate;
        public DateTime MsgCreatedDate
        {
            get { return _msgCreatedDate; }
            set
            {
                _msgCreatedDate = value;
                OnPropertyChanged(nameof(MsgCreatedDate));
            }
        }

        public bool IsLoadingInfinite
        {
            get { return _isLoadingInfinite; }
            set { SetProperty(ref _isLoadingInfinite, value); }
        }
        public int TotalRecords
        {
            get { return _totalRecords; }
            set { SetProperty(ref _totalRecords, value); }
        }
        public bool IsLoadingInfiniteEnabled
        {
            get { return _isLoadingInfiniteEnabled; }
            set { SetProperty(ref _isLoadingInfiniteEnabled, value); }
        }

        public Xamarin.Forms.ImageSource _FrameImage { get; set; }
        public Xamarin.Forms.ImageSource FrameImage
        {
            get { return _FrameImage; }
            set
            {
                _FrameImage = value;
                OnPropertyChanged("FrameImage");
            }
        }

        private int _TestMinHeight = 50;
        public int TestMinHeight
        {
            get { return _TestMinHeight; }
            set { SetProperty(ref _TestMinHeight, value); }
        }

        private bool _ShowToolBar { get; set; } = false;
        public bool ShowToolBar
        {
            get { return _ShowToolBar; }
            set
            {
                _ShowToolBar = value;
                OnPropertyChanged(nameof(ShowToolBar));
            }
        }

        private MediaFile _imageUrlMediaFile;
        public MediaFile imageUrlMediaFile
        {
            get { return _imageUrlMediaFile; }
            set
            {
                if (value != null)
                {
                    SetProperty(ref _imageUrlMediaFile, value);
                }
            }
        }

        public string _imageUrlfile;
        public string imageUrlfile
        {
            get { return _imageUrlfile; }
            set { SetProperty(ref _imageUrlfile, value); }
        }

        private ImageSource _imageUrl = "image_placeholder.png";
        public ImageSource ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                SetProperty(ref _imageUrl, value);
            }
        }

        public string _FileName;
        public string FileName
        {
            get { return _FileName; }
            set { SetProperty(ref _FileName, value); }
        }

        public string _LinkValidationText;
        public string LinkValidationText
        {
            get { return _LinkValidationText; }
            set { SetProperty(ref _LinkValidationText, value); }
        }

        private bool _isValidationVisible { get; set; } = false;
        public bool isValidationVisible
        {
            get { return _isValidationVisible; }
            set
            {
                _isValidationVisible = value;
                OnPropertyChanged(nameof(isValidationVisible));
            }
        }

        HttpClient httpClient;

        public ICommand SendMessageCommand { get; set; }

        public MessageThreadViewModel()
        {
            if (Application.Current.Properties.ContainsKey("PARTYNAME"))
            {
                LabelParty = Convert.ToString(Application.Current.Properties["PARTYNAME"]);
            }

            if (Application.Current.Properties.ContainsKey("TOPIC"))
            {
                LabelTopic = Convert.ToString(Application.Current.Properties["TOPIC"]);
            }

            if (Application.Current.Properties.ContainsKey("USERID"))
            {
                userId = Convert.ToString(Application.Current.Properties["USERID"]);
            }
            if (Application.Current.Properties.ContainsKey("THREADID"))
            {
                threadId = Convert.ToString(Application.Current.Properties["THREADID"]);
            }
            if (Application.Current.Properties.ContainsKey("ID"))
            {
                id = Convert.ToInt16(Application.Current.Properties["ID"]);
            }
            if (Application.Current.Properties.ContainsKey("NAME"))
            {
                name = Convert.ToString(Application.Current.Properties["NAME"]);
            }

            HeaderTitle = LabelParty;
            HeaderLeftImage = "back_arrow.png";

            var assembly = Assembly.GetAssembly(Application.Current.GetType());

            PersonDetailsCommand = new Command(() => PersonDetailsCommandExecute());
            ReplayCommand = new Command(() => ReplayCommandExecute());
            SendMessageCommand = new Command(() => SendMessageCommandExecute());
            LoadMoreCommand = new Command(async () => { await LoadMore_click(); });
            ImageCommand = new Command<object>(LoadImage);
        }

        private void LoadImage(object obj)
        {
            ImageInsertedEventArgs imageInsertedEventArgs = (obj as ImageInsertedEventArgs);
            this.GetImage(imageInsertedEventArgs);
        }

        private async void GetImage(ImageInsertedEventArgs imageInsertedEventArgs)
        {
            using (Stream imageStream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync())
            {
                if (imageStream != null)
                {
                    Syncfusion.XForms.RichTextEditor.ImageSource imageSource = new Syncfusion.XForms.RichTextEditor.ImageSource();
                    imageSource.ImageStream = imageStream;
                    imageSource.Height = 100;
                    imageSource.Width = 100;
                    imageInsertedEventArgs.ImageSourceCollection.Add(imageSource);
                }
            }
        }

        private void SendMessageCommandExecute()
        {
            if (sendEnable)
                return;

            sendEnable = true;

            //await SendMessage();

            sendEnable = false;
        }

        public async Task SendMessage(string Message)
        {
            try
            {
                if (Message == null || Message == "" || string.IsNullOrWhiteSpace(Message))
                {
                    return;
                }
                else
                {
                    var message = !string.IsNullOrEmpty(Message.Trim()) ? Message.Trim() : string.Empty;
                    if (message.Contains("https://") || message.Contains("http://"))
                    {
                        TypedMessage = "<p><a class=\"e-rte-anchor\" href=\"" + message + "\" title=\"" + message + "\">" + message + "</a></p>";
                        message = TypedMessage;
                    }
                    var values = new Dictionary<object, object>
                    {
                        {"author",userId },
                        {"threadId",threadId},
                        {"plainContent", message},
                        {"createdDate", DateTime.Now.ToUniversalTime() },
                        {"memoToFile",null},
                        {"sendToOfficer",false},
                    };

                    httpClient = new HttpClient();
                    var content = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(Config.SAVE_MESSAGE_API, content);
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
                        await ClosePopup();
                        var content1 = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine(content1);
                        TypedMessage = null;
                        Settings.TypedMessage = string.Empty;
                        App.FrameImage = null;
                        App.FileName = null;
                        isDocsVisible = false;
                        isImageVisible = false;
                        await FetchThreadUserData();

                        //await SendThreadUsersAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await ClosePopup();
                ShowExceptionAlert(ex);
            }
            finally
            {
                await ClosePopup();
            }
        }


        private async Task SendThreadUsersAsync()
        {
            httpClient = new HttpClient();
            if (Settings.MessageThreadUsersData != null && Settings.MessageThreadUsersData.Count > 0)
            {
                var content = new StringContent(JsonConvert.SerializeObject(Settings.MessageThreadUsersData), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(Config.SYNC_PARTICIPANTS + threadId, content);
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
                    var responceContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(responceContent);
                }
            }
            else
            {
                await FetchThreadUserData();
            }
        }

        private async void ReplayCommandExecute()
        {
            MessageReplayViewModel replayViewModel = new MessageReplayViewModel();
            replayViewModel.ScreenTitle = Constants.MessageScreenTitle;
            await App.Current.MainPage.Navigation.PushAsync(new MessageReplyPage(), false);
        }

        public void PersonDetailsCommandExecute()
        {

        }

        public ObservableCollection<messages> MessageList
        {
            get { return _messageList; }
            set
            {
                _messageList = value;
                OnPropertyChanged(nameof(MessageList));
            }
        }

        public ObservableCollection<messageUsers> ThreadList
        {
            get { return _threadList; }
            set
            {
                _threadList = value;
                OnPropertyChanged(nameof(ThreadList));
            }
        }

        private bool _IsLoadingInfiniteVisible { get; set; } = false;
        public bool IsLoadingInfiniteVisible
        {
            get { return _IsLoadingInfiniteVisible; }
            set
            {
                _IsLoadingInfiniteVisible = value;
                OnPropertyChanged(nameof(IsLoadingInfiniteVisible));
            }
        }

        public async Task FetchThreadUserData()
        {
            _isTeamLoading = true;
            IsLoadingInfiniteEnabled = true;
            App.buttonClick = 0;
            try
            {
                MessageList = new ObservableCollection<messages>();
                IsBusy = true;
                _CurrentPage = 1;
                await LoadData(_CurrentPage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception:>" + ex);
            }
            finally
            {
                IsBusy = false;
                _isTeamLoading = false;
            }
        }

        async Task LoadData(int page)
        {
            var tempOpenData = new ObservableCollection<messages>(MessageList);
            using (HttpClient hc = new HttpClient())
            {
                try
                {
                    IsBusy = true;
                    bool IsBookMarkSelect = false;
                    if (App.FrameImage != null)
                    {
                        ImageUrl = App.FrameImage;
                        isImageVisible = true;
                    }
                    if (App.FileName != null)
                    {
                        FileName = App.FileName;
                        isDocsVisible = true;
                    }

                    var jsonString = await hc.GetStringAsync(Config.MESSAGE_THREAD_API + threadId + "/" + page);

                    if (jsonString != "")
                    {
                        var obj = JsonConvert.DeserializeObject<DealMessageThreadList>(jsonString);
                        if (obj != null)
                        {
                            foreach (var user in obj.messages)
                            {
                                ThreadList.Clear();

                                if (user.plainContent.Contains("<p><br/></p>"))
                                {
                                    string value = user.plainContent.Replace("<p><br/></p>", null);
                                    user.plainContent = value;
                                }
                                if (user.plainContent.Contains(">?<"))
                                {
                                    string value = user.plainContent.Replace(">?<", "><");
                                    user.plainContent = value;
                                }

                                if (user.plainContent.Contains("data:image"))
                                {
                                    var image = user.plainContent.Split(',')[1];
                                    var image1 = image.Split('"')[0];
                                    Debug.Write(image1);

                                    string[] delim = { "alt=\"\">" };
                                    var stringMsg = image.Split(delim, StringSplitOptions.None);

                                    var Base64Stream = Convert.FromBase64String(image1);
                                    user.MsgImageUrl = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                                    user.IsImageVisible = true;

                                    if (!string.IsNullOrEmpty(stringMsg[1]))
                                    {
                                        user.plainContent = stringMsg[1];
                                    }
                                    else
                                    {
                                        user.plainContent = string.Empty;
                                    }
                                }
                                
                                string authorIdStr = user.authorId;
                                if (user.messageUsers != null)
                                {
                                    foreach (var thread in user.messageUsers)
                                    {
                                        if (thread.userid_10 == userId)
                                        {
                                            if (thread.followUp == true)
                                            {
                                                IsBookMarkSelect = true;
                                            }
                                            else
                                            {
                                                IsBookMarkSelect = false;
                                            }
                                        }
                                        ThreadList.Add(thread);
                                    }

                                    if (IsBookMarkSelect == true)
                                    {
                                        user.BookMarkImg = "orange_bookmark.png";
                                    }
                                    else
                                    {
                                        user.BookMarkImg = "bookmark.png";
                                    }
                                }

                                if (authorIdStr == null)
                                {
                                    user.HorizontalOption = LayoutOptions.StartAndExpand;
                                    user.IsSenderBookMarkVisible = false;
                                    user.IsSenderProfileVisible = true;
                                    user.IsProfileVisible = false;
                                    user.IsBookMarkVisible = true;
                                }
                                else if (authorIdStr.Equals(userId))
                                {
                                    user.HorizontalOption = LayoutOptions.EndAndExpand;
                                    user.IsSenderBookMarkVisible = true;
                                    user.IsSenderProfileVisible = false;
                                    user.IsProfileVisible = true;
                                    user.IsBookMarkVisible = false;
                                }
                                else
                                {
                                    user.HorizontalOption = LayoutOptions.StartAndExpand;
                                    user.IsSenderBookMarkVisible = false;
                                    user.IsSenderProfileVisible = true;
                                    user.IsProfileVisible = false;
                                    user.IsBookMarkVisible = true;
                                }

                                if (user.plainContent == null)
                                {
                                    user.IsStopVisible = false;
                                    user.HeightRequest = 0;
                                }
                                else if (user.plainContent.Count() < 150)
                                {
                                    user.IsStopVisible = false;
                                    user.HeightRequest = 0;
                                }

                                else if (user.plainContent.Count() > 150)
                                {
                                    user.IsStopVisible = true;
                                    user.moreBtnText = "more";
                                    user.HeightRequest = 35;
                                    user.MaxLines = 3;
                                }
                                TotalRecords = obj.totalRecords;
                                _LastPage = Convert.ToInt32(obj.totalPages);
                                tempOpenData.Add(user);
                            }

                            ObservableCollection<messages> OrderbyIdDesc = new ObservableCollection<messages>(tempOpenData.OrderByDescending(x => x.createdDate.Date));
                            MessageList = new ObservableCollection<messages>(OrderbyIdDesc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception:>" + ex);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public async Task LoadMore_click()
        {
            using (HttpClient hc = new HttpClient())
            {
                try
                {
                    if (_LastPage == _CurrentPage)
                    {
                        IsLoadingInfinite = false;
                        IsLoadingInfiniteEnabled = false;
                        return;
                    }
                    if (_isTeamLoading) { IsLoadingInfinite = false; return; }
                    _CurrentPage++;

                    await LoadData(_CurrentPage);
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    ShowExceptionAlert(ex);
                }
                finally
                {
                    IsBusy = false;
                }
            }
            IsLoadingInfinite = false;
        }

        public async Task PostReadThreadData()
        {
            using (HttpClient hc = new HttpClient())
            {
                try
                {
                    IsBusy = true;

                    var kvalues = new Dictionary<string, object>
                    {
                        {"threadId",threadId},
                        {"userId",userId },
                    };

                    var jObject = JsonConvert.SerializeObject(kvalues);
                    var content = new StringContent(jObject, Encoding.UTF8, "application/json");
                    HttpResponseMessage hs = await hc.PostAsync(Config.MARK_THREADMESSAGE_READ, content);
                    var result = await hs.Content.ReadAsStringAsync();
                    if (hs.IsSuccessStatusCode)
                    { //return Task; 
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public ICommand personIcon_Clicked { get { return new Command<messages>(personIcon_Click); } }
        private async void personIcon_Click(messages list)
        {
            ParticipantListPopup ParticipantPopupview = new ParticipantListPopup();
            ParticipantPopupview.BindingContext = this;
            this.GetParticipantData(list);
            await ShowPopup(ParticipantPopupview);
        }

        public void GetParticipantData(messages messages)
        {
            messageUserList.Clear();
            if (messages.messageUsers.Count != 0)
            {
                IsListDataAvailable = true;
                IsDataNotAvailable = false;
                foreach (var user in messages.messageUsers)
                {
                    messageUserList.Add(user);
                }
            }
            else
            {
                IsDataNotAvailable = true;
                IsListDataAvailable = false;
            }
        }

        public ICommand BookMarkCommand { get { return new Command<messages>(BookMark_click); } }
        private async void BookMark_click(messages list)
        {
            if (list.messageUsers != null)
            {
                if (list.BookMarkImg == "orange_bookmark.png")
                {
                    list.BookMarkImg = "bookmark.png";
                    bookMarks = false;
                }
                else
                {
                    list.BookMarkImg = "orange_bookmark.png";
                    bookMarks = true;
                }

                var values = new Dictionary<object, object>
                {
                    {"name",name },
                    {"userid_10",userId},
                    {"followUp", bookMarks},
                };

                var client = new HttpClient();
                string url = Config.MARK_THREADMESSAGE_BOOKMARK + list.id + "/" + bookMarks + "/" + userId;
                var content1 = new StringContent(JsonConvert.SerializeObject(jObject), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content1);

                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Content == null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.DisplayAlert("Data Not Sent!!", string.Format("Response contained status code: {0}", response.StatusCode), "OK");
                    });
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(content);
                }

            }
        }

        public ICommand TextFormatCommand { get { return new Command(TextFormat_click); } }
        private void TextFormat_click()
        {
            if (ShowToolBar == false)
            {
                TestMinHeight = 55;
                ShowToolBar = true;
            }
            else
            {
                TestMinHeight = 50;
                ShowToolBar = false;
            }
        }

        public ICommand AddDocsCommand { get { return new Command(AddDocs_click); } }
        private void AddDocs_click()
        {
            try
            {
                DocumentPicker(((string name, string FileBase64String, string FileType, string FileName) obj) =>
                {
                    if (obj.name != null && obj.FileName != null && obj.FileBase64String != null && obj.FileType != null)
                    {
                        //AddDocument(obj.FileName, obj.FileBase64String, obj.FileType);
                        FileName = obj.FileName;
                        App.FileName = FileName;
                        if (FileName != null)
                        {
                            isDocsVisible = true;
                        }
                    }
                    else
                    {
                        return;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: { ex.Message}");
            }
        }

        public ICommand AttachLinkCommand { get { return new Command(AttachLink_click); } }
        private async void AttachLink_click()
        {
            try
            {
                await ClosePopup();
                HyperLinkPopup HyperLinkPopupview = new HyperLinkPopup();
                HyperLinkPopupview.BindingContext = this;
                isValidationVisible = false;
                await ShowPopup(HyperLinkPopupview);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: { ex.Message}");
            }
        }

        public ICommand OpenCameraCommand { get { return new Command(OpenCamera_click); } }
        private void OpenCamera_click()
        {
            try
            {
                ImagePicker((string file, MediaFile mediafile) =>
                {
                    if (string.IsNullOrEmpty(file))
                    {
                        return;
                    }
                    else
                    {
                        imageUrlMediaFile = mediafile;
                        imageUrlfile = file;
                        ImageUrl = ImageSource.FromFile(file);
                        App.FrameImage = ImageUrl;

                        byte[] b = File.ReadAllBytes(file);
                        base64String = Convert.ToBase64String(b);

                        if (!string.IsNullOrEmpty(base64String))
                        {
                            App.base64String = "<img src=\"data:image/png;base64," + base64String + "\" alt=\"\">";
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: { ex.Message}");
            }
        }

        public ICommand InsertLinkCommand { get { return new Command(InsertLink_click); } }
        private async void InsertLink_click()
        {
            try
            {
                string pattern = @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$";

                if (!string.IsNullOrEmpty(Link))
                {
                    var m = Regex.Match(Link, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                    if (m.Success)
                    {
                        TypedMessage = Link;
                        Link = null;
                        await ClosePopup();
                    }
                    else
                    {
                        LinkValidationText = "Invalid Link";
                        isValidationVisible = true;
                    }
                }
                else
                {
                    LinkValidationText = "Link field is required";
                    isValidationVisible = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: { ex.Message}");
            }
        }

        public ICommand CloseFrame_Command { get { return new Command(CloseFrame_click); } }
        private void CloseFrame_click()
        {
            isImageVisible = false;
            App.base64String = null;
            App.FrameImage = null;
        }

        public ICommand CloseDocs_Command { get { return new Command(CloseDocs_click); } }
        private void CloseDocs_click()
        {
            isDocsVisible = false;
            App.FileName = null;
        }

        public ICommand ClosePopup_Command { get { return new Command(ClosePopup_click); } }
        private async void ClosePopup_click()
        {
            await ClosePopup();
        }
    }
}

