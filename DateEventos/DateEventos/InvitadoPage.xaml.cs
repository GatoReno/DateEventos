using DatePickerService.Models;
using Newtonsoft.Json;
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
	public partial class InvitadoPage : ContentPage
    {
        public InvitadoPage(int ID)
        {
            InitializeComponent();
            GetUserInfo(ID);

        }

        public async void GetUserInfo(int ID)
        {

            var uri = "http://aige.sytes.net/APIRESTSAM/api/citasapp/GetInvitadoInfo?id=" + ID;

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
                        //List<Table_Loc> loc_list = JsonConvert.DeserializeObject<List<Table_Loc>>(xjson);

                        Root myobject = JsonConvert.DeserializeObject<Root>(xjson);


                        DatosUser.IsVisible = true;
                        int myobjcount = myobject.tablas.Table.Count;

                        if (myobjcount == 0)
                        {
                            res_x.Text = "No Existen Ordenes Pendientes";
                        }
                        else
                        {

                            DatosUser.ItemsSource = myobject.tablas.Table;
                        }
                        // ListLoc
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
            //hasta acá para llamar info segun admin type


        }
    }
}