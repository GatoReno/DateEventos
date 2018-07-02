using DatePickerService.Classes;
using DatePickerService.Models;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DateEventos
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageNav : ContentPage
    {
        private MemberDatabase memberDatabase;
        private int myId;
        public PageNav()
        {
            InitializeComponent();
            memberDatabase = new MemberDatabase();
            var members = memberDatabase.GetMembers();
            var member = members.FirstOrDefault();
            myId = member.ID;

            GetPendientes();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //your code here;
            try
            {
                GetPendientes();
            }
            catch (Exception err)
            {
                DisplayAlert("Error", "" + err.ToString(), "Entiendo");
                throw;
            }

        }
      

        private void ToolbarItem_InfoUser(object sender, EventArgs e)
        {
            //navigation a una page llamada QrPage()
            Navigation.PushAsync(new UserInfo());
        }
        private void ToolbarItem_Appoi(object sender, EventArgs e)
        {
            //navigation a una page llamada QrPage()
            Navigation.PushAsync(new Appoiments());
        }
        public async void GoPendientes()
        {
            await Navigation.PushAsync
                (new InvitPage());
        }
        public async void GetPendientes()
        {

            var uri = "http://aige.sytes.net/APIRESTSAM/api/citasapp/CountCitas?id=" + myId.ToString();

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(uri);

            request.Method = HttpMethod.Get;

            var client = new HttpClient();
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok_ty, acc_tok);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.SendAsync(request);


            switch (response.StatusCode)
            {
                //200
                case (System.Net.HttpStatusCode.OK):

                    HttpContent content = response.Content;
                    string xjson = await content.ReadAsStringAsync();

                    try
                    {
                        Root myobject = JsonConvert.DeserializeObject<Root>(xjson);
                        int px = myobject.tablas.Table[0].pendientes;
                        if (px == 0)
                        {
                            Pend.Text = "Estás al día";
                        }
                        else
                        {
                            string p = px.ToString();
                            Pend.Text = p;
                        }

                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("", "" + ex.ToString(), "ok");
                        return;
                    }

                    break;
                //500
                case (System.Net.HttpStatusCode.InternalServerError):
                    await DisplayAlert("No existe registro de usuario", "Nuestros servidores estan en mantenimiento", "Continuar");

                    await Navigation.PushModalAsync(new AuthPage());
                    break;
                //404
                case (System.Net.HttpStatusCode.Forbidden):
                    try
                    {
                        await DisplayAlert("Su sesión ha caducado", "Reingrese sus datos", "ok");
                        // memberDatabase.DeleteMember(id);
                        await Navigation.PushModalAsync(new AuthPage());
                    }
                    catch (Exception ex)
                    {

                        res_x.Text = ex.ToString();

                    }
                    break;

            }
        }

      
    }
}