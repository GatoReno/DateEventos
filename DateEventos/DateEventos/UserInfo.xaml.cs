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
	public partial class UserInfo : ContentPage
    {
        public Member member;
        public MemberDatabase memberDatabase;
        public UserInfo()
        {
            InitializeComponent();
            memberDatabase = new MemberDatabase();
            var members = memberDatabase.GetMembers();
            listMembers.ItemsSource = members;
        }

        public async void OnSelected(object obj, ItemTappedEventArgs args)
        {
            var member = args.Item as Member;
            try
            {
                // await DisplayActionSheet("You selected", member.Name + " " + member.Roll,"ok");
                await userOptionsHandlerAsync(member.UserName, member.ID);
            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.ToString(), "");
                return;
            }
        }



        private async Task userOptionsHandlerAsync(string nm ,int ID)
        {

            var actionSheet = await DisplayActionSheet("Opciones de usuario " +nm , "Cancel", null, "Cerrar sesión");


            switch (actionSheet)
            {
                case "Cerrar sesión":

                    try
                    {
                        memberDatabase.DeleteMember(ID);
                        memberDatabase.DropCitaT();
                        Application.Current.MainPage = new NavigationPage(new SP());
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("", "" + ex.ToString(), "ok");
                    }




                    break;
            }

        }
        //listMembers Name id firma

    }
}