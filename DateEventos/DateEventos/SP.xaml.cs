using DatePickerService.Classes;
using DatePickerService.Models;
using Plugin.Connectivity;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DateEventos
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SP : ContentPage
    {
        public MemberDatabase memberDatabase;
        public Member member;
        public SQLiteConnection conn;

        Image splashImage;
        public SP()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            var sub = new AbsoluteLayout();
            splashImage = new Image
            {
                Source = "calen.png",
                WidthRequest = 100,
                HeightRequest = 100
            };
            AbsoluteLayout.SetLayoutFlags(splashImage,
               AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(splashImage,
             new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            sub.Children.Add(splashImage);

            this.BackgroundColor = Color.FromHex("#FFFFFF");
            this.Content = sub;
        }



        protected override async void OnAppearing()
        {
            base.OnAppearing();


            await splashImage.ScaleTo(1, 2000); //Time-consuming processes such as initialization
            await splashImage.ScaleTo(0.6, 500, Easing.BounceOut);
            // await splashImage.ScaleTo(150, 100, Easing.Linear);

            await splashImage.FadeTo(0, 750);

            InitializeComponent();


            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Advertencia ! ", "ACTIVA EL CONSUMO DE DATOS O WIFI . PARA PODER CONTINUAR ", "Ok");
                OnAppearing();
            }
            else
            {
                //await DisplayAlert("Recordatorio", "Esta aplicación requiere  EL CONSUMO DE DATOS O WIFI de lo contrario no funionará. Por favor active sus datos", "Continuar");

                await TryAccess();

            }


        }


        public async Task TryAccess()
        {

            memberDatabase = new MemberDatabase();
            var members = memberDatabase.GetMembers();
            int RowCount = 0;
            int membcount = members.Count();
            RowCount = Convert.ToInt32(membcount);
            if (RowCount > 0)
            {

                var mx = memberDatabase.GetMembers();
                var mx_first = mx.FirstOrDefault();


                var tok_ty = mx_first.Token_Type;
                var acc_tok = mx_first.Access_Token;
                var userName = mx_first.UserName;
                var pass = mx_first.Pass;
                var id = mx_first.ID;

                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok_ty, acc_tok);
                client.DefaultRequestHeaders.Add("api-version", "1.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var values = new Dictionary<string, string>
                         {
                            { "usuario",  userName },
                            { "contraseña", pass }

                         };


                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://aige.sytes.net/ApiRestSAM/api/CITASAPP/Acceso",
                    content);


                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode.OK):
                        Application.Current.MainPage = new NavigationPage(new PageNav());

                        break;

                    case (System.Net.HttpStatusCode.BadRequest):
                        testxy.Text = "no good";
                        break;

                    case (System.Net.HttpStatusCode.Forbidden):
                        await DisplayAlert("Su sesión ha caducado", "Reingrese sus datos", "ok");
                        memberDatabase.DeleteMember(id);

                        Application.Current.MainPage = new NavigationPage(new AuthPage());
                        break;

                    //500
                    case (System.Net.HttpStatusCode.InternalServerError):
                        string status = "Nuestros servidores estan en mantenimiento";
                        testxy.Text = status;
                        break;

                }

                //Application.Current.MainPage = new NavigationPage(new PageNav());
            }
            else
            {
                //await DisplayAlert("No existe registro de usuario", "Por favor acceda con su usuario y contraseña. Si no cuenta con ellos " +
                //    "pongase en contacto con su provedor", "Continuar");
                Application.Current.MainPage = new NavigationPage(new AuthPage());
            }




        }
    }
}