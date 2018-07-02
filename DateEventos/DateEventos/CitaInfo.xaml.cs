using DatePickerService.Classes;
using DatePickerService.Models;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using Plugin.Vibrate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public partial class CitaInfo : ContentPage
    {
        public MemberDatabase memberDatabase;
        public Cita cita;
        private int idCita;
        //private int alarmRing = 0;
        //private MemberDatabase memberdatabase;
        //public SQLiteConnection conn;


        public CitaInfo(int ID)
        {
            InitializeComponent();
            memberDatabase = new MemberDatabase();
            var citaunica = memberDatabase.CitaUnica(ID);
            ListCitaUnica.ItemsSource = new List<Cita> { citaunica };
            //DateTime dtx = DateTime.Parse(dt);

            GETUSERSCITA(ID);
            idCita = ID;
      

            //if (alarmRing == 1)
            //{
            //    btnAlarm.IsEnabled = true;
            //}
            //else {
            //    btnAlarm.IsEnabled = false;
            //}
            
        }

        public  void RingAlarm() {

            var cti = memberDatabase.CitaUnica(idCita);
            var razon = cti.razon;
            var v = CrossVibrate.Current;
            v.Vibration(TimeSpan.FromSeconds(12));
            Debug.WriteLine("-----------------------------------------------Alarma Sonando-------------------------------------");
            CrossLocalNotifications.Current.Show("Tú cita" + razon + " es ahora !!!", "Entiendo");
            playAl();
        }

        public void playAl() {

            DependencyService.Get<IAudio>().PlayAudioFile("softAlarm.mp3");
        }
        public async void BorrarEvento()
        {

            var screen = await DisplayActionSheet("¿Seguro deseas eliminar este evento?", "Cancelar", null, "Eliminar");
            switch (screen)
            {
                case "Eliminar":
                    BorrarEvento_Click();
                    break;
            }
        }

        public async void BorrarEvento_Click()
        {


            HttpClient client = new HttpClient();


            var values = new Dictionary<string, string>
                         {
                            { "idevento",  idCita.ToString() }
                         };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://aige.sytes.net/ApiRestSAM/api/CITASAPP/EliminarCita",
                content);
            //handler / respuesta de status
            string status;

            switch (response.StatusCode)
            {


                // 200
                case (System.Net.HttpStatusCode.OK):


                    var responseString = await response.Content.ReadAsStringAsync();

                    // var xjson = JsonConvert.DeserializeObject(responseString);
                    var xjson = JsonConvert.DeserializeObject<Root>(responseString);
                    var bnx = xjson.bandera;

                    if (bnx == "0")
                    {
                        await Navigation.PopToRootAsync();
                        // Application.Current.MainPage = new NavigationPage(new SP());
                    }
                    else
                    {
                        res_x.Text = "Hubo algún error ...";
                    }

                    break;

                // 400
                case (System.Net.HttpStatusCode.BadRequest):
                    status = "Usuario o contraseña invalidos -error 400";
                    res_x.Text = status;
                    break;

                //500
                case (System.Net.HttpStatusCode.InternalServerError):
                    status = "Nuestros servidores estan en mantenimiento";
                    res_x.Text = status;
                    break;

                // 502
                case (System.Net.HttpStatusCode.BadGateway):
                    status = "Usuario o contraseña invalidos - error 502";
                    res_x.Text = status;
                    break;

                // 403 required

                case (System.Net.HttpStatusCode.Forbidden):
                    status = "Acceso rechazado";
                    res_x.Text = status;
                    await DisplayAlert("Error de acceso", "Es probable que tu sesión haya caducado. Ingresa tus datos de acceso nuevamente", "Continuar");
                    break;

                // 404
                case (System.Net.HttpStatusCode.NotFound):
                    status = "Error - 404 Servidor no encontrado";
                    res_x.Text = status;
                    await DisplayAlert("Error de acceso", "Es probable que tu sesión haya caducado. Ingresa tus datos de acceso nuevamente", "Continuar");
                    break;


            }

        }



        public void InvitarUsers()
        {
            var members = memberDatabase.GetMembers();
            var member = members.FirstOrDefault();
            int myid = member.ID;
            Navigation.PushAsync(new InvitarUsers(idCita, myid));
        }

        public async void GETUSERSCITA(int ID)
        {

            var uri = "http://aige.sytes.net/APIRESTSAM/api/citasapp/GetInvitadosCita?idevento=" + ID;

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


        public async void OnSelected_user(object obj, ItemTappedEventArgs args)
        {

            try
            {
                var member = args.Item as Table;
                await userOptionsHandlerAsync(member.idusuario, member.nombre);
            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.ToString(), "");
                return;
            }
        }

        private async Task userOptionsHandlerAsync(string ID, string name)
        {

            var actionSheet = await DisplayActionSheet("Opciones de usuario " + name, "Cancel", null, "Ver información", "Eliminar invitación");


            switch (actionSheet)
            {
                case "Ver información":

                    try
                    {
                        int id = Int32.Parse(ID);
                        await Navigation.PushAsync(new InvitadoPage(id));

                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("", "" + ex.ToString(), "ok");
                    }

                    break;
                case "Eliminar invitación":

                    try
                    {
                        HttpClient client = new HttpClient();


                        var values = new Dictionary<string, string>
                         {
                            { "idusuario",  ID},
                            { "idevento", idCita.ToString() }
                         };

                        var content = new FormUrlEncodedContent(values);

                        var response = await client.PostAsync("http://aige.sytes.net/ApiRestSAM/api/CITASAPP/EliminarInvitadoEvento",
                            content);
                        //handler / respuesta de status
                        string status;

                        switch (response.StatusCode)
                        {


                            // 200
                            case (System.Net.HttpStatusCode.OK):


                                var responseString = await response.Content.ReadAsStringAsync();

                                // var xjson = JsonConvert.DeserializeObject(responseString);
                                var xjson = JsonConvert.DeserializeObject<Root>(responseString);
                                var bn = xjson.bandera;

                                if (bn == "0")
                                {
                                    await DisplayAlert("", "Invitación a " + name + " deshecha", "ok");


                                    await Navigation.PopToRootAsync();
                                }
                                else
                                {
                                    res_x.Text = "Puede que haya existido un error.";

                                }
                                break;

                            // 400
                            case (System.Net.HttpStatusCode.BadRequest):
                                status = "Usuario o contraseña invalidos -error 400";
                                res_x.Text = status;
                                break;

                            //500
                            case (System.Net.HttpStatusCode.InternalServerError):
                                status = "Nuestros servidores estan en mantenimiento";
                                res_x.Text = status;
                                break;

                            // 502
                            case (System.Net.HttpStatusCode.BadGateway):
                                status = "Usuario o contraseña invalidos - error 502";
                                res_x.Text = status;
                                break;

                            // 403 required

                            case (System.Net.HttpStatusCode.Forbidden):
                                status = "Acceso rechazado";
                                res_x.Text = status;
                                await DisplayAlert("Error de acceso", "Es probable que tu sesión haya caducado. Ingresa tus datos de acceso nuevamente", "Continuar");
                                break;

                            // 404
                            case (System.Net.HttpStatusCode.NotFound):
                                status = "Error - 404 Servidor no encontrado";
                                res_x.Text = status;
                                await DisplayAlert("Error de acceso", "Es probable que tu sesión haya caducado. Ingresa tus datos de acceso nuevamente", "Continuar");

                                break;


                        }

                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("", "" + ex.ToString(), "ok");
                    }

                    //await Navigation.PopAsync();
                    // Application.Current.MainPage = new NavigationPage(new SP());

                    break;
            }

        }


    }
}