using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.Text.Json;
using BAL;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using DAL;

namespace StudyProject.ViewModel
{
  public class BasketViewModel:BE.BaseViewModel
    {
        private ObservableCollection<Model.Good> _good_basket_list;
        public ObservableCollection<Model.Good> GoodBasketList//shopping cart
        {
            get => _good_basket_list;
            set
            {
                _good_basket_list = value;
                OnPropertyChanged();
            }
        }
        private BE.Order _order;
        public BE.Order Order
        {
            get => _order;
            set
            {
                _order = value;
                OnPropertyChanged();
            }
        }
        private List<AssociationRule> rules;
        private ObservableCollection<Model.Good> _often_order;
        public ObservableCollection<Model.Good> OftenOrder//recommended items
        {
            get => _often_order;
            set
            {
                _often_order = value;
                OnPropertyChanged();
            }
        }
        public BasketViewModel()
        {
            InitData();
        }     
        private void InitData()//initialising data
        {
            GoodBasketList=new ObservableCollection<Model.Good>();
            rules = MainViewModel.BALimp.GetAssociationRules(MainViewModel.DALimp.GetAllGoodsFromBasket());
            Order=new BE.Order();
            Order.Basket = new ObservableCollection<BE.Basket>();//link between good and order
            OftenOrder = new ObservableCollection<Model.Good>();
        }
        public ICommand ClearBasket => new DelegateCommand(InitData);
        #region  Command
        public ICommand AddGoodFromQr => new RelayCommand(() =>//add good by qr
        {
            OpenFileDialog load = new OpenFileDialog(); //  load asks user for picture source
            if (load.ShowDialog() == System.Windows.Forms.DialogResult.OK) // if the user hits "open"
            {
              var file=  File.ReadAllBytes(load.FileName);
              Bitmap bmp;
              using (var ms = new MemoryStream(file))
              {
                  bmp = new Bitmap(ms);
              }
              QRCodeDecoder decoder = new QRCodeDecoder(); // decode image
                try
                {
                    var a = decoder.Decode(new QRCodeBitmapImage(bmp));
                    AddToBasket(a, file);//add item to cart
                }
              catch
                {
                    MessageBox.Show("This picture is not QR or it can't be recognized", "Error");
                }
              
            }
            
        });
        private void AddToBasket(string qr_json, byte[] file)
        {
            var good_json = JsonSerializer.Deserialize<BE.GoodSerialized>(qr_json);
            var good=new Model.Good(good_json);
            AddToBasket(good,false);
            Order.Basket.Add(new BE.Basket(good.Id, good.Count, file));//linking the order and the good and addin QR
        }
        public void AddToBasket(Model.Good good,bool file)
        {
            Order.ItogCount += good.Count;
            Order.ItogPrice += good.Price;
            GoodBasketList.Add(good);
            if(file)
                Order.Basket.Add(new BE.Basket(good.Id, good.Count));
            OftenOrder.Remove(OftenOrder.Where(p => p.Id == good.Id).FirstOrDefault());//if the recommended good was added it will be deleted from recommendations
            var rul = MainViewModel.BALimp.GetFindAssotiat(GoodBasketList.Select(p => (BE.Good)p).ToList(), rules);
            List<int> new_coincidence = new List<int>();//List of ids of recommended goods
            foreach (var n_c in rul)
            {
                new_coincidence.AddRange(n_c.Label1.Select(p => Convert.ToInt32(p)).ToList());
            }
            if (new_coincidence.Count > 0)
            {

                foreach (var new_con in new_coincidence)
                {
                    OftenOrder.Add(new Model.Good(new_con));
                }
            }
        }
        public ICommand CreateOrder => new RelayCommand(() => { 
                MainViewModel.DALimp.AddOrder(Order);
                GoodBasketList = new ObservableCollection<Model.Good>();
                Order = new BE.Order();
                Order.Basket = new ObservableCollection<BE.Basket>();
            rules = MainViewModel.BALimp.GetAssociationRules(MainViewModel.DALimp.GetAllGoodsFromBasket());
        });
        public ICommand SelectOffenGoodCommand
        {
            get => new DelegateCommand<Model.Good>(SelectOffenGood);
        }
        private void SelectOffenGood(Model.Good good)
        {
            var AddOffenGood = new View.Basket.AddOffenGood(good);
            AddOffenGood.Show();
        }   
        #endregion
    }
}
