using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Simon.Helpers;
using Simon.Models;
using Simon.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Simon.Views
{
    public partial class MessageReplyPage : ContentPage
    {
        List<string> country = new List<string>();
        UserListModel ObjAssignList = new UserListModel();
        DealMessageThreadList ObjThreadUsers = new DealMessageThreadList();

        public IEnumerable<string> threadUsersItemSource { get; set; }
        public List<messageUsers> dealthreadUsers { get; set; }

        ObservableCollection<messageUsers> data = new ObservableCollection<messageUsers>();
        MessageReplayViewModel thisVm = null;

        public ObservableCollection<messageUsers> threadUsers
        {
            get;
            set;
        }

        public MessageReplyPage()
        {
            InitializeComponent();
            //searchBarTxt.Focus();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            App.IsFromAddParticipantPage = true;

            thisVm = new MessageReplayViewModel();
            this.BindingContext = thisVm;

            if (NetworkCheck.IsInternet())
            {
                await thisVm.GetData();
                await thisVm.GetThreadUsersData();
            }
            else
            {
                await DisplayAlert("Simon", "No network is available.", "OK");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            thisVm.Back();
            return true;
        }    
    }
}
