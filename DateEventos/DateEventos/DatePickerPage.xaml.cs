using DatePickerService.Classes;
using DatePickerService.Models;
using Newtonsoft.Json;
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
	public partial class DatePickerPage : ContentPage
    {
        public MemberDatabase memberDatabase;
        public Member member;
        public Cita cita;
        private MemberDatabase memberdatabase;
        public SQLiteConnection conn;


        public DatePickerPage()
        {
            InitializeComponent();
        }
        public async void InvitarUsuarios()
        {
            string dt = DTP.Date.ToString("yyyy-MM-dd"); ;
            var ttx = TP.Time.Hours;
            int tms = TP.Time.Minutes;
            string tt = ttx.ToString();

            var tmsS = tms.ToString();
            int tmsL = tmsS.Length;

            if (tmsL == 1)
            {
                tmsS = "0" + tms;
            }
            else
            {
                tmsS = tms.ToString();
            }
            lbl.Text = "Cita agendada en la fecha: " + dt + "/" + tt + ":" + tmsS;

            try
            {
                memberdatabase = new MemberDatabase();
                var members = memberdatabase.GetMembers();
                var member = members.FirstOrDefault();
                var ID = member.ID;
                string rz = Razon.Text;
                string desc = Descripcion.Text;
                string IDst = ID.ToString();

                HttpClient client = new HttpClient();

                // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok_ty, acc_tok);
                client.DefaultRequestHeaders.Add("api-version", "1.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var values = new Dictionary<string, string>
                         {
                            { "ID",   IDst },
                            { "fecha", dt },
                            { "hora", tt+" :" +tmsS},
                            { "descripcion", desc},
                            { "razon", rz }
                         };


                var content = new FormUrlEncodedContent(values);

                var link1 = "http://aige.sytes.net/ApiRestSAM/api/CITASAPP/CitaNueva";
                //var link2 = "http://192.168.50.18/ApiRestSAM/api/CITASAPP/CitaNueva";
                var response = await client.PostAsync(link1, content);

                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode.OK):

                        var responseString = await response.Content.ReadAsStringAsync();



                        // var xjson = JsonConvert.DeserializeObject(responseString);
                        var xjson = JsonConvert.DeserializeObject<Root>(responseString);

                        int eventoid = Convert.ToInt32(xjson.tablas.Table1[0].Eventoid);
                        cita = new Cita();
                        cita.descripcion = Descripcion.Text;
                        cita.fecha = dt;
                        cita.razon = rz;
                        cita.hora = tt;
                        cita.ID = eventoid;
                        cita.descripcion = desc;



                        memberdatabase.AddCita(cita);

                        int idcita = eventoid;
                        int myid = ID;
                        await DisplayAlert("Cita generada", "Elija a los usuarios con los que desea compartir esta cita", "Entiendo");
                        await Navigation.PushAsync(new InvitarUsers(idcita, myid));
                        break;

                    case (System.Net.HttpStatusCode.NotFound):
                        fuck.Text = "Error 404";
                        break;

                    case (System.Net.HttpStatusCode.BadRequest):
                        fuck.Text = "Error 400";
                        break;

                    case (System.Net.HttpStatusCode.Forbidden):
                        fuck.Text = "Error 403";
                        break;
                    //500
                    case (System.Net.HttpStatusCode.InternalServerError):
                        string status = "Nuestros servidores estan en mantenimiento";
                        fuck.Text = status;
                        break;
                }
            }
            catch (Exception ex)
            {

                fuck.Text = ex.ToString();
            }
        }


        public async void CrearSolo()
        {
            string dt = DTP.Date.ToString("yyyy-MM-dd"); ;
            int ttx = TP.Time.Hours;
            int tms = TP.Time.Minutes;
            string tt = ttx.ToString();

            var tmsS = tms.ToString();
            int tmsL = tmsS.Length;

            if (tmsL == 1)
            {
                tmsS = "0" + tms;
            }
            else {
                tmsS = tms.ToString();
            }
            lbl.Text ="Cita agendada en la fecha: "+ dt +"/" + tt +":" + tmsS;
            try
            {

                memberdatabase = new MemberDatabase();
                var members = memberdatabase.GetMembers();
                var member = members.FirstOrDefault();
                var ID = member.ID;
                string rz = Razon.Text;
                string desc = Descripcion.Text;
                string IDst = ID.ToString();




                HttpClient client = new HttpClient();

                // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok_ty, acc_tok);
                client.DefaultRequestHeaders.Add("api-version", "1.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var values = new Dictionary<string, string>
                         {
                            { "ID",   IDst },
                            { "fecha", dt },
                            { "hora", tt+":" +tmsS},
                            { "descripcion", desc},
                            { "razon", rz }
                         };


                var content = new FormUrlEncodedContent(values);

                var link1 = "http://aige.sytes.net/ApiRestSAM/api/CITASAPP/CitaNueva";
                //var link2 = "http://192.168.50.18/ApiRestSAM/api/CITASAPP/CitaNueva";
                var response = await client.PostAsync(link1, content);

                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode.OK):

                        var responseString = await response.Content.ReadAsStringAsync();



                        // var xjson = JsonConvert.DeserializeObject(responseString);
                        var xjson = JsonConvert.DeserializeObject<Root>(responseString);

                        int eventoid = Convert.ToInt32(xjson.tablas.Table1[0].Eventoid);
                        cita = new Cita();
                        cita.descripcion = Descripcion.Text;
                        cita.fecha = dt;
                        cita.razon = rz;
                        cita.hora = tt + ":" + tms;
                        cita.ID = eventoid;
                        cita.descripcion = desc;
                        memberdatabase.AddCita(cita);

                        await DisplayAlert("Cita generada", "Lista generada y agregada al diario con éxito", "Entiendo");
                        await Navigation.PopToRootAsync();
                        break;

                    case (System.Net.HttpStatusCode.NotFound):
                        fuck.Text = "Error 404";
                        break;

                    case (System.Net.HttpStatusCode.BadRequest):
                        fuck.Text = "Error 400";
                        break;

                    case (System.Net.HttpStatusCode.Forbidden):
                        fuck.Text = "Error 403";
                        break;
                    //500
                    case (System.Net.HttpStatusCode.InternalServerError):
                        string status = "Nuestros servidores estan en mantenimiento";
                        fuck.Text = status;
                        break;
                }



            }
            catch (Exception ex)
            {

                fuck.Text = ex.ToString();
            }
        }



        public void Tryme()
        {


            if (string.IsNullOrEmpty(Razon.Text))
            {
                Razon.Focus();
            }
            else
            {
                Tryx.IsVisible = false;
                Invitar.IsVisible = true;
                Solo.IsVisible = true;

            }




        }
    }
}