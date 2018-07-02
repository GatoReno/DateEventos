using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DateEventos
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        public void trymex() {
            //
            CrossLocalNotifications.Current.Show("sdsds","sdsdsds");
            
           // CrossPushNotification.Current.NotificationHandler();
        }

    }
}
