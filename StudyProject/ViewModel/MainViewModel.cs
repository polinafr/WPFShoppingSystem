using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using StudyProject.View.Basket;
using StudyProject.ViewModel;
namespace StudyProject
{
    public class MainViewModel : BE.BaseViewModel//main viewmodel
    {
        //Dividing logics for work with viewmodel 
        public static EditBaseViewModel EditBaseViewModel;//ViewModel for redacting base
        public static BasketViewModel BasketViewModel;//ViewModel for work with the shopping lists
        public static StatisticsViewModel StatisticsViewModel;//ViewModel to work with statistics module
        private UserControl _main_control;
        public static DAL.DALimp DALimp;
        public static BAL.BALimp BALimp;
        public UserControl MainControl
        {
            get => _main_control;
            set
            {
                _main_control = value;
                OnPropertyChanged();
            }
        }

        private List<UserControl> Controls;
        public MainViewModel()
        {

            var firebase = new DAL.Firebase();
            DALimp = new DAL.DALimp();
            BALimp=new BAL.BALimp();
            EditBaseViewModel = new EditBaseViewModel();
            BasketViewModel = new BasketViewModel();
            StatisticsViewModel=new StatisticsViewModel();
           
            Controls = new List<UserControl> { new View.EditBase.MainEditPage(), new MainBasketPage(), new View.Statistics.StatisticsMain() };

        }
        public ICommand OpenEditBase
        {
            get => new RelayCommand(() =>
            {
                MainControl = Controls[0];

            });
        }
        public ICommand OpenBasket
        {
            get => new RelayCommand(() =>
            {
                MainControl = Controls[1];

            });
        }
        public ICommand OpenStatistic
        {
            get => new RelayCommand(() =>
            {
                MainControl = Controls[2];

            });
        }
    }
}
