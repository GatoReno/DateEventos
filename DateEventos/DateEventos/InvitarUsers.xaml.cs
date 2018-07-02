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
	public partial class InvitarUsers : ContentPage
    {
        public Member member;
        public MemberDatabase memberDatabase;
        private int IDCITx { set; get; }

        public InvitarUsers(int idcita, int myid)
        {
            InitializeComponent();
            IDCITx = idcita;
            GetUsers(myid);

        }

        public async void OnSelected(object obj, ItemTappedEventArgs args)
        {
            var inv = args.Item as Table;
            try
            {
                var idx = inv.idusuario;
                int idus = Int32.Parse(idx);
                string nmus = inv.nombreusuario;
                // await DisplayActionSheet("You selected", member.Name + " " + member.Roll,"ok");
                await userOptionsHandlerAsync(idus, nmus);
            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.ToString(), "");
                return;
            }


        }

        private async Task userOptionsHandlerAsync(int ID, string nmus)
        {

            var actionSheet = await DisplayActionSheet("¿ Deesas invitar a  " + nmus + " ?", "Cancel", null, "Invitar");


            switch (actionSheet)
            {
                case "Invitar":

                    try
                    {
                        string idcita = IDCITx.ToString();
                        string idusr = ID.ToString();
                        HttpClient client = new HttpClient();
                        // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok_ty, acc_tok);
                        client.DefaultRequestHeaders.Add("api-version", "1.0");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var values = new Dictionary<string, string>
                         {
                            { "idusuario", idusr },
                            { "idevento", idcita}
                         };

                        var content = new FormUrlEncodedContent(values);

                        var link1 = "http://aige.sytes.net/ApiRestSAM/api/CITASAPP/Invitar";

                        var response = await client.PostAsync(link1, content);

                        switch (response.StatusCode)
                        {
                            case (System.Net.HttpStatusCode.OK):

                                var responseString = await response.Content.ReadAsStringAsync();



                                // var xjson = JsonConvert.DeserializeObject(responseString);
                                var xjson = JsonConvert.DeserializeObject<Root>(responseString);


                                await DisplayAlert("Éxito", "Usuario " + nmus + " envitado", "Gracias");

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

        }

        public async void TerminarInvitaciones()
        {



            await Navigation.PopToRootAsync();
            await DisplayAlert("Éxito", "Usuario Invitados, te notificaremos cuando acepten tu invitación", "Gracias");


        }

        public async void GetUsers(int myid)
        {

            var uri = "http://aige.sytes.net/APIRESTSAM/api/citasapp/getusuarios?id=" + myid;

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

                        res_x.Text = "Elige a los participantes";
                        ListUsers.IsVisible = true;
                        int myobjcount = myobject.tablas.Table.Count;

                        if (myobjcount == 0)
                        {
                            res_x.Text = "No Existen Ordenes Pendientes";
                        }
                        else
                        {

                            ListUsers.ItemsSource = myobject.tablas.Table;
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

        //termina clase
    }
}