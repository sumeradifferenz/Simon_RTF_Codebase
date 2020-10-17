﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Simon.Helpers;
using Simon.Models;
using Simon.Views;
using Simon.Views.Popups;
using Xamarin.Forms;

namespace Simon.ViewModel
{
    public class MessageViewModel : BaseViewModel
    {
        string userId;
        bool SetValue = true;
        private ObservableCollection<DealMessageList> _openData = new ObservableCollection<DealMessageList>();
        public ICommand SortCommand { get; set; }
        public ICommand SortingCommand { get; set; }
        public Command FilterCommand { get; set; }
        public Command BookMarkCommand { get; set; }
        public NavigationHelper _helper;
        private ObservableCollection<DealMessageList> AllItems = new ObservableCollection<DealMessageList>();
        bool isFirstTime = false, isSortingPopup = false, isFilterPopup = false;
        public string SortByImage;

        private bool searchFlag = false;
        private bool _isBookMarkFilterOn = false;
        private bool _isLoadingInfinite = false;
        private int _totalRecords = 0;
        private bool _isLoadingInfiniteEnabled = false;
        private int _CurrentPage = 1;
        private int _LastPage = 0;
        private bool _isTeamLoading = false;
        string threadId;
        public ICommand LoadMoreCommand { get; private set; }
        public ICommand SearchCommand { get; set; }

        public MessageViewModel()
        {
            if (App.FollowUp == "true")
            {
                BookMarkImage = "orange_bookmark.png";
            }
            else
            {
                BookMarkImage = "bookmark.png";
            }

            if (Application.Current.Properties.ContainsKey("USERID"))
            {
                userId = Convert.ToString(Application.Current.Properties["USERID"]);
            }

            if (Application.Current.Properties.ContainsKey("THREADID"))
            {
                threadId = Convert.ToString(Application.Current.Properties["THREADID"]);
            }

            SearchCommand = new Command(() => SearchCommandExecuteAsync());
            SortingCommand = new Command(() => SortingCommandExecute());
            FilterCommand = new Command(() => FilterCommandExecute());
            BookMarkCommand = new Command(() => BookMarkCommandExecute());
            SortCommand = new Command(() => SortCommandCommandExecute());
            LoadMoreCommand = new Command(async () => { await LoadMore_click(); });

            MessagingCenter.Subscribe<MessageViewModel, int>(this, "OnNotificationReceived", (sender, args) =>
            {
                if (Settings.MessageCount == args)
                {
                    return;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    var msgFooter = this.FooterItems.FirstOrDefault(x => x.Id == 2);
                    if (args == 0)
                    {
                        msgFooter.isMsgBadgeVisible = false;
                    }
                    else
                    {
                        msgFooter.isMsgBadgeVisible = true;
                    }
                    msgFooter.MsgCount = args;
                    Settings.MessageCount = args;
                });

            });

            MessagingCenter.Subscribe<MessageViewModel, int>(this, "OnNotificationOpen", (sender, args) =>
            {
                if (Settings.MessageCount == args)
                {
                    return;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    var msgFooter = this.FooterItems.FirstOrDefault(x => x.Id == 2);
                    if (args == 0)
                    {
                        msgFooter.isMsgBadgeVisible = false;
                    }
                    else
                    {
                        msgFooter.isMsgBadgeVisible = true;
                    }
                    msgFooter.MsgCount = args;
                    Settings.MessageCount = args;
                });

            });
        }

        /// <summary>
        /// OpenData list item tap command.
        /// </summary>
        public ICommand MessageListItemTapCommand { get { return new Command<DealMessageList>(MessageListItemTapCommandExecute); } }
        private async void MessageListItemTapCommandExecute(DealMessageList dealMessage)
        {
            try
            {
                if (App.buttonClick == 0)
                {
                    App.buttonClick++;

                    int threadIdNo = dealMessage.threadId;
                    string strPartyName = dealMessage.partyName;
                    string strTopic = dealMessage.topic;

                    Application.Current.Properties["THREADID"] = threadIdNo;
                    Application.Current.Properties["PARTYNAME"] = strPartyName;
                    Application.Current.Properties["TOPIC"] = strTopic;

                    int index = OpenData.ToList().FindIndex(s => s.dealId == dealMessage.dealId);
                    if (index != -1)
                    {
                        OpenData[index].hasBeenRead = true;
                    }

                    await Application.Current.SavePropertiesAsync();
                    //await Application.Current.MainPage.Navigation.PushAsync(new MessageThreadViewPage(), false);

                    _helper.NavigateToItemDetail(new MessageThreadViewPage());
                    //Application.Current.MainPage = new NavigationPage(new MessageThreadViewPage());
                }
            }
            catch (Exception ex)
            {
                ShowExceptionAlert(ex);
            }
        }

        private bool isEmpty;
        public bool IsEmpty
        {
            get { return isEmpty; }
            set { isEmpty = value; OnPropertyChanged(nameof(IsEmpty)); }
        }

        private string _SortBylblText;
        public string SortBylblText
        {
            get
            {
                return _SortBylblText;
            }
            set
            {
                SetProperty(ref _SortBylblText, value);
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

        private bool _IsNoDataFound { get; set; } = false;
        public bool IsNoDataFound
        {
            get { return _IsNoDataFound; }
            set
            {
                _IsNoDataFound = value;
                OnPropertyChanged(nameof(IsNoDataFound));
            }
        }

        private bool _isMessagsListVisible;
        public bool IsMessagsListVisible
        {
            get { return _isMessagsListVisible; }
            set { SetProperty(ref _isMessagsListVisible, value); }
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

        public string _BookMarkImage { get; set; } = "bookmark.png";
        public string BookMarkImage
        {
            get { return _BookMarkImage; }
            set
            {
                _BookMarkImage = value;
                OnPropertyChanged("BookMarkImage");
            }
        }

        public string _sortingIcon { get; set; } = "sort.png";
        public string SortingIcon
        {
            get { return _sortingIcon; }
            set
            {
                _sortingIcon = value;
                OnPropertyChanged("SortingIcon");
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

        public ObservableCollection<DealMessageList> OpenData
        {
            get { return _openData; }
            set
            {
                _openData = value;
                OnPropertyChanged(nameof(OpenData));
            }
        }

        public string _searchText = string.Empty;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                // _searchText = value;
                SetProperty(ref _searchText, value);
                SearchDeal(value);
            }
        }

        public void SearchDeal(string searchText)
        {
            try
            {
                if (AllItems.Count > 0)
                {
                    var tempDealList = new ObservableCollection<DealMessageList>();
                    if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrWhiteSpace(searchText))
                    {
                        var Keyword = searchText.ToLower().Trim();
                        if (Keyword.Length >= 1)
                        {
                            var searchedMessage = AllItems.Where(s => s.partyName.ToLower().Contains(Keyword)
                                        || s.topic.ToLower().Contains(Keyword)
                                        || s.lastPostBy.ToLower().Contains(Keyword)
                                        || !string.IsNullOrEmpty(s.lastMessage) && s.lastMessage.ToLower().Contains(Keyword)
                                        || s.dealId.ToString().Contains(Keyword.ToString()));

                            var tempMessageList = searchedMessage
                                        .OrderByDescending(item => item.lastPostDate)
                                        .ToList();

                            if (tempMessageList.Count > 0)
                            {
                                foreach (var item in tempMessageList)
                                {
                                    tempDealList.Add(item);
                                }
                                searchFlag = true;
                                IsMessagsListVisible = true;
                                IsNoDataFound = false;
                            }
                            else
                            {
                                IsMessagsListVisible = false;
                                IsNoDataFound = true;
                            }
                        }
                        else
                        {
                            IsMessagsListVisible = true;
                            IsNoDataFound = false;
                            foreach (var item in OpenData)
                            {
                                tempDealList.Add(item);
                            }
                        }

                        OpenData = new ObservableCollection<DealMessageList>(tempDealList);
                    }
                    else
                    {
                        IsNoDataFound = false;
                        IsMessagsListVisible = true;
                        OpenData = new ObservableCollection<DealMessageList>(AllItems);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public ICommand CleanSearchCommand { get { return new Command(CleanSearch_click); } }
        private void CleanSearch_click()
        {
            IsNoDataFound = false;
            IsMessagsListVisible = true;
            searchFlag = false;
            SearchText = null;
            OpenData = new ObservableCollection<DealMessageList>(AllItems);
        }

        public async Task FetchData()
        {
            _isTeamLoading = true;
            IsLoadingInfiniteEnabled = true;
            try
            {
                OpenData = new ObservableCollection<DealMessageList>();
                IsBusy = true;
                _CurrentPage = 1;
                await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, App.AsceDsce, App.FollowUp);
                //await LoadData(_CurrentPage, App.ReadUnread);
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

        //--------------- Method for New Message API call ------------------
        async Task LoadDataV1(int page, string ReadUnread, string OrderByText, bool AsceDsce, string FollowUp)
        {
            try
            {
                if (isFirstTime)
                {
                    isFirstTime = false;
                    OpenData.Clear();
                }

                var tempOpenData = new ObservableCollection<DealMessageList>(OpenData);
                HttpClient hc = new HttpClient();
                string api = Config.MESSAGES_API_V1 + userId + "/" + page + "/" + ReadUnread + "/" + OrderByText + "/" + AsceDsce + "/" + FollowUp;

                if (!string.IsNullOrEmpty(_searchText))
                {
                    api = api + "/" + _searchText;
                }
                var jsonString = await hc.GetStringAsync(api);
                if (jsonString != "")
                {
                    var obj = JsonConvert.DeserializeObject<List<DealMessageList>>(jsonString);
                    if (obj.Count > 0)
                    {
                        IsDataNotAvailable = false;
                        IsMessagsListVisible = true;

                        foreach (var user in obj)
                        {
                            if (user.lastMessage == null)
                            {
                                user.ImageVisible = false;
                                user.MessageVisible = true;
                            }
                            else if (user.lastMessage.Contains("data:image"))
                            {
                                user.ImageVisible = true;
                                user.MessageVisible = false;
                            }
                            else
                            {
                                if (user.lastMessage.Contains("<p><br/></p>"))
                                {
                                    string value = user.lastMessage.Replace("<p><br/></p>", null);
                                    user.lastMessage = value;
                                }
                                if (user.lastMessage.Contains("<li>"))
                                {
                                    string value = user.lastMessage.Replace("?", "");
                                    user.lastMessage = value;
                                }
                                if (user.lastMessage.Contains("e-rte-anchor"))
                                {
                                    user.lastMessage = user.lastMessage.Split('"')[3];
                                }

                                user.ImageVisible = false;
                                user.MessageVisible = true;
                            }

                            bool IsRead = user.hasBeenRead;
                            bool IsBookMark = user.followupExist;
                            if (IsRead == true)
                            {
                                user.IsRedDotVisible = false;
                                user.LastMsgStyle = (Style)App.Current.Resources["LatoBoldDarkGrayLableStyle"];
                            }
                            else if (IsRead == false)
                            {
                                user.IsRedDotVisible = true;
                                user.LastMsgStyle = (Style)App.Current.Resources["LatoBoldDarkBlueLableStyle"];
                            }

                            if (IsBookMark == true)
                            {
                                user.IsBookMarkVisible = true;
                                user.Switchimg = "orange_bookmark.png";
                                user.LastMsgStyle = (Style)App.Current.Resources["LatoBoldDarkGrayLableStyle"];
                            }
                            else if (IsBookMark == false)
                            {
                                user.IsBookMarkVisible = false;
                                user.LastMsgStyle = (Style)App.Current.Resources["LatoBoldDarkBlueLableStyle"];
                            }

                            TotalRecords = user.totalRecords;
                            _LastPage = Convert.ToInt32(user.totalPages);
                            tempOpenData.Add(user);
                        }

                        OpenData = new ObservableCollection<DealMessageList>(tempOpenData);
                        AllItems = new ObservableCollection<DealMessageList>(tempOpenData);

                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            SearchDeal(SearchText);
                        }
                    }
                    else
                    {
                        if (searchFlag == true)
                        {
                            IsDataNotAvailable = false;
                        }
                        else
                        {
                            IsDataNotAvailable = true;
                        }
                        IsMessagsListVisible = false;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Error", "CANCLE");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception:>" + ex);
            }
        }

        //--------------- Method for Old Message API call ------------------
        //async Task LoadData(int page, string ReadUnread)
        //{
        //    try
        //    {
        //        if (isFirstTime)
        //        {
        //            isFirstTime = false;
        //            OpenData.Clear();
        //        }

        //        var tempOpenData = new ObservableCollection<DealMessageList>(OpenData);
        //        HttpClient hc = new HttpClient();
        //        string api = Config.MESSAGES_API + userId + "/" + page + "/" + ReadUnread;

        //        if (!string.IsNullOrEmpty(_searchText))
        //        {
        //            api = api + "/" + _searchText;
        //        }
        //        var jsonString = await hc.GetStringAsync(api);
        //        if (jsonString != "")
        //        {
        //            var obj = JsonConvert.DeserializeObject<List<DealMessageList>>(jsonString);
        //            if (obj.Count > 0)
        //            {
        //                IsDataNotAvailable = false;
        //                IsMessagsListVisible = true;

        //                foreach (var user in obj)
        //                {
        //                    if (user.lastMessage == null)
        //                    {
        //                        user.ImageVisible = false;
        //                        user.MessageVisible = true;
        //                    }
        //                    else if (user.lastMessage.Contains("data:image"))
        //                    {
        //                        user.ImageVisible = true;
        //                        user.MessageVisible = false;
        //                    }
        //                    else
        //                    {
        //                        if (user.lastMessage.Contains("<p><br/></p>"))
        //                        {
        //                            string value = user.lastMessage.Replace("<p><br/></p>", null);
        //                            user.lastMessage = value;
        //                        }
        //                        user.ImageVisible = false;
        //                        user.MessageVisible = true;
        //                    }

        //                    bool IsRead = user.hasBeenRead;
        //                    bool IsBookMark = user.followupExist;
        //                    if (IsRead == true)
        //                    {
        //                        user.IsRedDotVisible = false;
        //                        user.LastMsgStyle = (Style)App.Current.Resources["LatoRegularGrayLableStyle"];
        //                    }
        //                    else if (IsRead == false)
        //                    {
        //                        user.IsRedDotVisible = true;
        //                        user.LastMsgStyle = (Style)App.Current.Resources["LatoRegularDarkBlueLableStyle"];
        //                    }

        //                    if (IsBookMark == true)
        //                    {
        //                        user.IsBookMarkVisible = true;
        //                        user.Switchimg = "orange_bookmark.png";
        //                        user.LastMsgStyle = (Style)App.Current.Resources["LatoRegularGrayLableStyle"];
        //                    }
        //                    else if (IsBookMark == false)
        //                    {
        //                        user.IsBookMarkVisible = false;
        //                        user.LastMsgStyle = (Style)App.Current.Resources["LatoRegularDarkBlueLableStyle"];
        //                    }

        //                    TotalRecords = user.totalRecords;
        //                    _LastPage = Convert.ToInt32(user.totalPages);
        //                    tempOpenData.Add(user);
        //                }

        //                OpenData = new ObservableCollection<DealMessageList>(tempOpenData);
        //                AllItems = new ObservableCollection<DealMessageList>(tempOpenData);

        //                if (!string.IsNullOrEmpty(SearchText))
        //                {
        //                    SearchDeal(SearchText);
        //                }
        //            }
        //            else
        //            {
        //                if (searchFlag == true)
        //                {
        //                    IsDataNotAvailable = false;
        //                }
        //                else
        //                {
        //                    IsDataNotAvailable = true;
        //                }
        //                IsMessagsListVisible = false;
        //            }
        //        }
        //        else
        //        {
        //            await App.Current.MainPage.DisplayAlert("Alert", "Error", "CANCLE");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Exception:>" + ex);
        //    }
        //}

        public async Task LoadMore_click()
        {
            using (HttpClient hc = new HttpClient())
            {
                try
                {
                    if (App.SelectedName == Constants.BorrowerlblText)
                    {
                        SetValue = App.AsceDsceName;
                    }
                    if (App.SelectedName == Constants.LastPostDatelblText)
                    {
                        SetValue = App.AsceDsce;
                    }

                    if (_LastPage == _CurrentPage)
                    {
                        IsLoadingInfinite = false;
                        IsLoadingInfiniteEnabled = false;
                        return;
                    }
                    if (_isTeamLoading) { IsLoadingInfinite = false; return; }

                    _CurrentPage++;

                    await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, SetValue, App.FollowUp);
                    //await LoadData(_CurrentPage, App.ReadUnread);
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

        public ICommand SortByAscDesClicked { get { return new Command<DealsSortByModel>(SortByAscDes_Click); } }
        private async void SortByAscDes_Click(DealsSortByModel data)
        {
            try
            {
                if (data.name == Constants.BorrowerlblText)
                {
                    if (data.SortByAscDescbtnimg == Constants.NameDescendingImg)
                    {
                        data.SortByAscDescbtnimg = Constants.NameAcendingImg;
                        App.AsceDsceName = false;
                    }
                    else
                    {
                        data.SortByAscDescbtnimg = Constants.NameDescendingImg;
                        App.AsceDsceName = true;
                    }
                }
                else if (data.name == Constants.LastPostDatelblText)
                {
                    if (data.SortByAscDescbtnimg == Constants.NumberDescendingImg)
                    {
                        data.SortByAscDescbtnimg = Constants.NumberAcendingImg;
                        App.AsceDsce = false;
                    }
                    else
                    {
                        data.SortByAscDescbtnimg = Constants.NumberDescendingImg;
                        App.AsceDsce = true;
                    }
                }
            }
            catch (Exception ex)
            {
                await ClosePopup();
                Debug.WriteLine(ex.Message);
            }
        }

        public async void SortCommandCommandExecute()
        {
            try
            {
                isFilterPopup = false;
                isSortingPopup = false;
                IsLoadingInfiniteEnabled = true;
                if (DealsSortByItems != null && DealsSortByItems.Count > 0)
                {
                    foreach (var item in DealsSortByItems)
                    {
                        if (item.name == Constants.ReadlblText)
                        {
                            if (item.Radiobtnimg == Constants.RadiobtnSelectImg)
                            {
                                await ClosePopup();
                                isFirstTime = true;
                                App.ReadUnread = "true";
                                _CurrentPage = 0;
                                await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, App.AsceDsce, App.FollowUp);
                                //await LoadData(_CurrentPage, App.ReadUnread);
                            }
                        }
                        else if (item.name == Constants.UnReadlblText)
                        {
                            if (item.Radiobtnimg == Constants.RadiobtnSelectImg)
                            {
                                await ClosePopup();
                                isFirstTime = true;
                                App.ReadUnread = "false";
                                _CurrentPage = 0;
                                await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, App.AsceDsce, App.FollowUp);
                                //await LoadData(_CurrentPage, App.ReadUnread);
                            }
                        }
                        else if (item.name == Constants.RealAllText)
                        {
                            if (item.Radiobtnimg == Constants.RadiobtnSelectImg)
                            {
                                await ClosePopup();
                                isFirstTime = true;
                                App.ReadUnread = "null";
                                _CurrentPage = 0;
                                await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, App.AsceDsce, App.FollowUp);
                                //await LoadData(_CurrentPage, App.ReadUnread);
                            }
                        }
                        else if (item.name == Constants.BorrowerlblText)
                        {
                            if (item.Radiobtnimg == Constants.RadiobtnSelectImg)
                            {
                                await ClosePopup();
                                isFirstTime = true;
                                App.OrderByText = Constants.PartyNamelblText;
                                _CurrentPage = 0;
                                await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, App.AsceDsceName, App.FollowUp);
                            }
                        }
                        else if (item.name == Constants.LastPostDatelblText)
                        {
                            if (item.Radiobtnimg == Constants.RadiobtnSelectImg)
                            {
                                await ClosePopup();
                                isFirstTime = true;
                                App.OrderByText = Constants.LastPostDateText;
                                _CurrentPage = 0;
                                await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, App.AsceDsce, App.FollowUp);
                            }
                        }
                        else if (item.name == Constants.ClearlblText)
                        {
                            if (item.Radiobtnimg == Constants.RadiobtnSelectImg)
                            {
                                await ClosePopup();
                                isFirstTime = true;
                                App.OrderByText = Constants.LastPostDateText;
                                _CurrentPage = 0;
                                await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, true, App.FollowUp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ClosePopup();
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                await ClosePopup();
            }
        }

        public ICommand DealsSortByclicked { get { return new Command<DealsSortByModel>(DealsSortBy_click); } }
        private async void DealsSortBy_click(DealsSortByModel data)
        {
            try
            {
                var IsAnyUserNameSelected = DealsSortByItems.Where(x => x.Radiobtnimg == Constants.RadiobtnSelectImg).ToList();
                if (IsAnyUserNameSelected.Count > 0 && IsAnyUserNameSelected != null)
                {
                    foreach (var item in IsAnyUserNameSelected)
                    {
                        item.Radiobtnimg = Constants.RadiobtnUnselectImg;
                        item.NamelblStyle = (Style)App.Current.Resources["LatoRegularGrayLableStyle"];
                    }
                }

                if (data != null)
                {
                    if (data.Radiobtnimg == Constants.RadiobtnUnselectImg)
                    {
                        data.Radiobtnimg = Constants.RadiobtnSelectImg;
                        data.NamelblStyle = (Style)App.Current.Resources["LatoBoldDarkBlueLableStyle"];
                    }
                    else
                    {
                        data.Radiobtnimg = Constants.RadiobtnUnselectImg;
                        data.NamelblStyle = (Style)App.Current.Resources["LatoRegularGrayLableStyle"];
                    }
                    if (isFilterPopup)
                    {
                        App.SelectedTitle = data.name;
                    }
                    if (isSortingPopup)
                    {
                        App.SelectedName = data.name;
                    }
                }
            }
            catch (Exception ex)
            {
                await ClosePopup();
                Debug.WriteLine(ex.Message);
            }
        }

        public ICommand ClosePopup_Command { get { return new Command(ClosePopup_click); } }
        private async void ClosePopup_click()
        {
            isFilterPopup = false;
            isSortingPopup = false;
            await ClosePopup();
        }

        private async void BookMarkCommandExecute()
        {
            if (App.SelectedName == Constants.BorrowerlblText)
            {
                SetValue = App.AsceDsceName;
            }
            if (App.SelectedName == Constants.LastPostDatelblText)
            {
                SetValue = App.AsceDsce;
            }

            if (BookMarkImage == "bookmark.png")
            {
                BookMarkImage = "orange_bookmark.png";
                isFirstTime = true;
                App.FollowUp = "true";
                _CurrentPage = 0;
                await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, SetValue, App.FollowUp);
            }
            else
            {
                BookMarkImage = "bookmark.png";
                isFirstTime = true;
                App.FollowUp = "null";
                _CurrentPage = 0;
                await LoadDataV1(_CurrentPage, App.ReadUnread, App.OrderByText, SetValue, App.FollowUp);
            }
        }

        private void SearchCommandExecuteAsync()
        {
            string value = SearchText;
            SearchDeal(value);
        }

        public async void SortingCommandExecute()
        {
            await ClosePopup();
            DealSortByPopup DealSortByPopupview = new DealSortByPopup();
            DealSortByPopupview.BindingContext = this;
            SortBylblText = Constants.SortBylblText;
            isSortingPopup = true;
            this.MessagesSortByData();
            await ShowPopup(DealSortByPopupview);

            //await ShowLoader();
            //IsLoadingInfiniteEnabled = true;
            //List<DealMessageList> tempRecords;

            //if (IsSortApplied = !IsSortApplied)
            //{
            //    SortingIcon = "Sort_Up.png";
            //    tempRecords = AllItems.OrderBy(c => c.lastPostDate).ToList();
            //}
            //else
            //{
            //    SortingIcon = "sort.png";
            //    tempRecords = AllItems.OrderByDescending(c => c.lastPostDate).ToList();
            //}

            //await ClosePopup();

            //OpenData = new ObservableCollection<DealMessageList>(tempRecords);
            //var firstMessage = OpenData.FirstOrDefault();
            //MessagingCenter.Send<object, DealMessageList>(this, "MessageSortApplied", firstMessage);
        }

        public async void FilterCommandExecute()
        {
            try
            {
                await ClosePopup();
                DealSortByPopup DealSortByPopupview = new DealSortByPopup();
                DealSortByPopupview.BindingContext = this;
                SortBylblText = Constants.FilterBylblText;
                isFilterPopup = true;
                this.GetMessageFilter();
                await ShowPopup(DealSortByPopupview);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                ShowExceptionAlert(ex);
            }
        }

        private ObservableCollection<DealsSortByModel> _DealsSortByItems = new ObservableCollection<DealsSortByModel>();
        public ObservableCollection<DealsSortByModel> DealsSortByItems
        {
            get { return _DealsSortByItems; }
            set
            {
                _DealsSortByItems = value;
                OnPropertyChanged(nameof(DealsSortByItems));
            }
        }

        private void MessagesSortByData()
        {
            DealsSortByItems.Clear();
            DealsSortByItems.Add(new DealsSortByModel { name = Constants.BorrowerlblText, Radiobtnimg = (App.SelectedName == Constants.BorrowerlblText) ? Constants.RadiobtnSelectImg : Constants.RadiobtnUnselectImg, SortByAscDescbtnimg = (App.AsceDsceName == true) ? Constants.NameDescendingImg : Constants.NameAcendingImg });
            DealsSortByItems.Add(new DealsSortByModel { name = Constants.LastPostDatelblText, Radiobtnimg = (App.SelectedName == Constants.LastPostDatelblText) ? Constants.RadiobtnSelectImg : Constants.RadiobtnUnselectImg, SortByAscDescbtnimg = (App.AsceDsce == true) ? Constants.NumberDescendingImg : Constants.NumberAcendingImg });
            DealsSortByItems.Add(new DealsSortByModel { name = Constants.ClearlblText});
        }

        public void GetMessageFilter()
        {
            DealsSortByItems.Clear();
            DealsSortByItems.Add(new DealsSortByModel { name = Constants.ReadlblText, Radiobtnimg = (App.SelectedTitle == Constants.ReadlblText) ? Constants.RadiobtnSelectImg : Constants.RadiobtnUnselectImg });
            DealsSortByItems.Add(new DealsSortByModel { name = Constants.UnReadlblText, Radiobtnimg = (App.SelectedTitle == Constants.UnReadlblText) ? Constants.RadiobtnSelectImg : Constants.RadiobtnUnselectImg });
            DealsSortByItems.Add(new DealsSortByModel { name = Constants.RealAllText, Radiobtnimg = (App.SelectedTitle == Constants.RealAllText) ? Constants.RadiobtnSelectImg : Constants.RadiobtnUnselectImg });
        }

        private CancellationTokenSource throttleCts = new CancellationTokenSource();

        /// <summary>
        /// Runs in a background thread, checks for new Query and runs current one
        /// </summary>
        public async Task DelayedQueryForKeyboardTypingSearches(string searchtxt)
        {
            try
            {
                Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(500), this.throttleCts.Token)
                .ContinueWith(async task => await SearchOpenData(searchtxt), CancellationToken.None,
                            TaskContinuationOptions.OnlyOnRanToCompletion,
                            TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
                //Ignore any Threading errors
            }
        }

        public async Task SearchOpenData(string searchText)
        {
            try
            {
                //_searchText = searchText;
                //_CurrentPage = 1;
                //OpenData = new ObservableCollection<DealMessageList>();
                //IsBusy = true;
                //await ShowLoader();
                //await LoadData(_CurrentPage);
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
            }
            finally
            {
                IsBusy = false;
                await ClosePopup();
            }
        }
    }
}

