using DatePickerService.Classes;
using DatePickerService.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DateEventos
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VerCitasPage : ContentPage
    {

        public SQLiteConnection conn;
        public MemberDatabase memberDatabase;

        public VerCitasPage()
        {
            InitializeComponent();

            CheckDbCitas();
            memberDatabase = new MemberDatabase();
            var users = memberDatabase.GetMembers();
            var user = users.FirstOrDefault();
            int ID = user.ID;

            var appx = new App();
            appx.GetCitasUser(ID);

        }
        public void CheckDbCitas()
        {
            memberDatabase = new MemberDatabase();
            var citas = memberDatabase.GetCitas();
            int check = citas.Count();

            if (check == 0)
            {
                res_x.Text = "Usted no tiene ninguna cita en agenda";
            }
            else
            {
                res_x.Text = "Citas generadas";
                ListCitas.ItemsSource = citas;
            }
        }
        public async void OnSelected(object obj, ItemTappedEventArgs args)
        {
            var cita = args.Item as Cita;
            try
            {
                //await DisplayAlert("You selected", orden.orden + " " + orden.idOrden,"ok");
                int ID = cita.ID;
               

                await Navigation.PushAsync(new CitaInfo(ID));

            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.ToString(), "");
                return;
            }
        }
    }
}