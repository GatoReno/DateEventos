using DatePickerService.Classes;
using DatePickerService.Models;
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
	public partial class CitasExp : ContentPage
	{
        private int idusr;
        private MemberDatabase memberDatabase;
        public CitasExp()
        {
            InitializeComponent();
            memberDatabase = new MemberDatabase();
            var members = memberDatabase.GetMembers();
            var me = members.FirstOrDefault();
            idusr = me.ID;
            btnDrop.Clicked += delegate { DropTableExp(); };
            var exp = memberDatabase.GetCitasExpL();
            int exCount = exp.Count;
            if (exCount == 0)
            {
                res_x.Text = "No tienes citas en esta sección";
                btnDrop.IsVisible = false;
            }
            else { 
            ListExp.ItemsSource = exp;
            res_x.Text = "Citas anteriores";
            }

        }
        public async void DropTableExp() {
            var table = memberDatabase.DropCitaExpT();
            await Navigation.PopToRootAsync();
        }

        public async void OnSelected(object obj, ItemTappedEventArgs args)
        {
            var lex = args.Item as Cita;
            try
            {
                // await DisplayActionSheet("You selected", member.Name + " " + member.Age+" "+member.ID,);
              //  await userOptionsHandlerAsync(lex.ID);
                await Navigation.PushAsync(new CitaInfo(idusr));
            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.ToString(), "");
                return;
            }
        }


    }
}