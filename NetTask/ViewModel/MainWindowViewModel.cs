using NetTask.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using NetTask.Model;
using NetTask.Services;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

namespace NetTask.ViewModel
{
    /// <summary>
    /// The ViewModel for the application's main window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<User> _userList;
        private UserManager _userManager;
        private RelayCommand _closeAppCommand;
        private RelayCommand _refreshDataCommand;
        #endregion // Fields

        #region Constructor

        public MainWindowViewModel()
        {
            base.DisplayName = "GitHub User list";
            RefreshData();
        }

        #endregion // Constructor

        #region Properties

        public ObservableCollection<User> UserList
        {
            get { return _userList; }
            set
            {
                _userList = value;
                OnPropertyChanged("UserList");
            }

        }

        public UserManager usManager
        {
            get
            {
                if (_userManager == null)
                    _userManager = new UserManager();

                return _userManager;
            }
        }

        public RelayCommand CloseAppCommand
        {
            get
            {
                if (_closeAppCommand == null)
                {
                    _closeAppCommand = new RelayCommand(
                        param => Closing_App()
                        );
                }
                return _closeAppCommand;
            }
        }

        public RelayCommand RefreshDataCommand
        {
            get
            {
                if (_refreshDataCommand == null)
                {
                    _refreshDataCommand = new RelayCommand(
                        param => RefreshData()
                        );
                }
                return _refreshDataCommand;
            }
        }


        #endregion // Properties

        #region "Methods"
        private void RefreshData()
        {
            UserList = new ObservableCollection<User>();
            RetriveUsersFromWebAPI("/users?&per_page=100");
        }


        private async void RetriveUsersFromWebAPI(string url)
        {
            try
            {
                await usManager.GetAllUserByPage(url, _userList);
            }
            catch (CustomException cex)
            {
               MessageBox.Show(cex.CustomMessage,"Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Closing_App()
        {
            var response = MessageBox.Show("Do you really want to exit?", "Exit",
                                           MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (response == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }


        #endregion // Methods
    }
}
