using DatePickerService.Classes;
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
	public partial class InvitPage : ContentPage
    {
        private int idusr;
        private MemberDatabase memberDatabase;


        public InvitPage()
        {
            InitializeComponent();
            memberDatabase = new MemberDatabase();
            var members = memberDatabase.GetMembers();
            var me = members.FirstOrDefault();
            idusr = me.ID;
            GetPendientes(idusr);
        }

        public async void OnSelected(object obj, ItemTappedEventArgs args)
        {
            var inv = args.Item as Table;
            try
            {
                var idx = inv.idevento;
                //int idus = Int32.Parse(idx);
                string nmus = inv.razon;
                await userOptionsHandlerAsync(idx, nmus);
            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.ToString(), "");
                return;
            }
        }

        private async Task userOptionsHandlerAsync(int ID, string ra)
        {
            var actionSheet = await DisplayActionSheet("¿ Acpetar invitación a cita/ razón: ' " + ra + " ' ?", "Cancel", null, "Aceptar", "Rechazar");
            switch (actionSheet)
            {
                case "Aceptar":
                    try
                    {
                        HttpClient client = new HttpClient();
                        // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok_ty, acc_tok);
                        client.DefaultRequestHeaders.Add("api-version", "1.0");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var values = new Dictionary<string, string>
                         {
                            { "idusuario", idusr.ToString() },
                            { "idevento", ID.ToString()}
                         };

                        var content = new FormUrlEncodedContent(values);

                        var link1 = "http://aige.sytes.net/ApiRestSAM/api/CITASAPP/AceptarCita";

                        var response = await client.PostAsync(link1, content);

                        switch (response.StatusCode)
                        {
                            case (System.Net.HttpStatusCode.OK):

                                var responseString = await response.Content.ReadAsStringAsync();
                                var xjson = JsonConvert.DeserializeObject<Root>(responseString);


                                await DisplayAlert("Éxito", "Usuario ya eres parte de la cita" + ra + " ", "Gracias");
                                await Navigation.PopToRootAsync();
                                break;

                            case (System.Net.HttpStatusCode.NotFound):
                                res_x.Text = "Error 404";
                                break;

                            case (System.Net.HttpStatusCode.BadRequest):
                                res_x.Text = "Error 400";
                                break;

                            case (System.Net.HttpStatusCode.Forbidden):
                                res_x.Text = "Error 403";
                                break;
                            //500
                            case (System.Net.HttpStatusCode.InternalServerError):
                                string status = "Nuestros servidores estan en mantenimiento";
                                res_x.Text = status;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("", "" + ex.ToString(), "ok");
                    }
                    break;


                case "Rechazar":
                    var actionSheet_ = await DisplayActionSheet("¿Estás seguro de querer rechazar esta invitación?", "Cancelar", null, "Rechazar");

                    switch (actionSheet_)
                    {
                        case "Rechazar":

                            try
                            {
                                HttpClient client = new HttpClient();
                                // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok_ty, acc_tok);
                                client.DefaultRequestHeaders.Add("api-version", "1.0");
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                var valuesx = new Dictionary<string, string>
                                 {
                                    { "idusuario", idusr.ToString() },
                                    { "idevento", ID.ToString()}
                                 };
                                var contentx = new FormUrlEncodedContent(valuesx);

                                var link2 = "http://aige.sytes.net/ApiRestSAM/api/CITASAPP/RechazarCita";
                                var responsex = await client.PostAsync(link2, contentx);
                                switch (responsex.StatusCode)
                                {
                                    case (System.Net.HttpStatusCode.OK):

                                        var responseStringx = await responsex.Content.ReadAsStringAsync();
                                        var xjsonx = JsonConvert.DeserializeObject<Root>(responseStringx);
                                        if (xjsonx.bandera == "0")
                                        {
                                            await DisplayAlert("Solicitud Rechazada", "Usted rechazo invitación para" + ra + " ", "Entiendo");
                                            await Navigation.PopToRootAsync();
                                        }
                                        else
                                        {
                                            await DisplayAlert("Error", "Hubo algún error en la cita" + ra + " ", "Entiendo");
                                        }

                                        break;
                                    case (System.Net.HttpStatusCode.NotFound):
                                        res_x.Text = "Error 404";
                                        break;

                                    case (System.Net.HttpStatusCode.BadRequest):
                                        res_x.Text = "Error 400";
                                        break;

                                    case (System.Net.HttpStatusCode.Forbidden):
                                        res_x.Text = "Error 403";
                                        break;
                                    //500
                                    case (System.Net.HttpStatusCode.InternalServerError):
                                        string status = "Nuestros servidores estan en mantenimiento";
                                        res_x.Text = status;
                                        break;
                                }

                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("", "" + ex.ToString(), "ok");
                            }

                            break;
                    }
                    break;
            }
        }


        public async void GetPendientes(int myid)
        {

            var uri = "http://aige.sytes.net/APIRESTSAM/api/citasapp/GetInvitaciones?id=" + myid;

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(uri);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
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
                        res_x.Text = "Invitaciones pendientes";
                        ListPendientes.IsVisible = true;
                        int myobjcount = myobject.tablas.Table.Count;

                        if (myobjcount == 0)
                        {
                            res_x.Text = "No tienes citas pendientes por confirmar";
                        }
                        else
                        {
                            ListPendientes.ItemsSource = myobject.tablas.Table;
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