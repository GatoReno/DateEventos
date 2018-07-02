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
    public partial class AuthPage : ContentPage
    {
        public MemberDatabase memberDatabase;
        public Member member;
        private MemberDatabase memberdatabase;
        public SQLiteConnection conn;

        public AuthPage()
        {
            InitializeComponent();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //    this.Content = null;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (this.Content == null)
            {
                Navigation.PushAsync(new PageNav());
            }
        }
        private async Task Access_Login()
        {

            if (string.IsNullOrEmpty(userTxt.Text))
            {
                await DisplayAlert("Error", "Por favor ingrese su nombre de usuario", "ok");
                userTxt.Focus();
                return;

            }
            if (string.IsNullOrEmpty(passTxt.Text))
            {
                await DisplayAlert("Error", "Por favor ingrese una contraseña", "ok");
                passTxt.Focus();
                return;
            }
            else
            {

                string userName = userTxt.Text;
                string pass = passTxt.Text;

                try
                {
                    waitActIndicator.IsRunning = true;
                    btnAuth.IsEnabled = false;
                    HttpClient client = new HttpClient();


                    var values = new Dictionary<string, string>
                         {
                            { "UserName",  "citas_app" },
                            { "Password", "fhHauSvvnJ6j" },
                            { "grant_type" , "password" }
                         };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("http://aige.sytes.net/ApiRestSAM/TOKEN",
                        content);
                    //handler / respuesta de status
                    string status;

                    switch (response.StatusCode)
                    {


                        // 200
                        case (System.Net.HttpStatusCode.OK):


                            var responseString = await response.Content.ReadAsStringAsync();

                            // var xjson = JsonConvert.DeserializeObject(responseString);
                            var xjson = JsonConvert.DeserializeObject<TokenRequest>(responseString);

                            status = xjson.access_token + " " + xjson.expires_in;
                            var xjson_token = xjson.access_token;
                            var xjson_type = xjson.token_type;
                            var xjson_exp = xjson.expires_in;

                            res_Label.Text = "Acceso a Token Request";

                            string tok_ty = xjson_type;
                            string acc_tok = xjson_token;



                            try
                            {
                                Login_Api(userName, pass, tok_ty, acc_tok);
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Alerta", ex.ToString(), "ok");
                            }

                            break;

                        // 400
                        case (System.Net.HttpStatusCode.BadRequest):
                            status = "Usuario o contraseña invalidos -error 400";
                            res_Label.Text = status;
                            break;

                        //500
                        case (System.Net.HttpStatusCode.InternalServerError):
                            status = "Nuestros servidores estan en mantenimiento";
                            res_Label.Text = status;
                            break;

                        // 502
                        case (System.Net.HttpStatusCode.BadGateway):
                            status = "Usuario o contraseña invalidos - error 502";
                            res_Label.Text = status;
                            break;

                        // 403 required

                        case (System.Net.HttpStatusCode.Forbidden):
                            status = "Acceso rechazado";
                            res_Label.Text = status;
                            await DisplayAlert("Error de acceso", "Es probable que tu sesión haya caducado. Ingresa tus datos de acceso nuevamente", "Continuar");
                            break;

                        // 404
                        case (System.Net.HttpStatusCode.NotFound):
                            status = "Error - 404 Servidor no encontrado";
                            res_Label.Text = status;
                            await DisplayAlert("Error de acceso", "Es probable que tu sesión haya caducado. Ingresa tus datos de acceso nuevamente", "Continuar");
                            break;


                    }

                }
                catch (Exception ex)
                {

                    res_Label.Text = ex.ToString();
                }

            }

        }

        public async void Login_Api(string userName, string pass, string tok_ty, string acc_tok)
        {

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok_ty, acc_tok);
            client.DefaultRequestHeaders.Add("api-version", "1.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var values = new Dictionary<string, string>
                         {
                            { "usuario",  userName },
                            { "contrasena", pass }

                         };


            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://aige.sytes.net/ApiRestSAM/api/citasapp/accesousuarios",
                content);


            switch (response.StatusCode)
            {
                case (System.Net.HttpStatusCode.OK):
                    res_Label_api.Text = "Acceso Exitoso";

                    var responseString = await response.Content.ReadAsStringAsync();



                    // var xjson = JsonConvert.DeserializeObject(responseString);
                    var xjson = JsonConvert.DeserializeObject<Root>(responseString);

                    int xid = Convert.ToInt32(xjson.bandera);
                    string id_x = xjson.bandera;
                    var idx = xjson.tablas.Table1[0].IdUsuario;
                    int idid = Convert.ToInt32(xjson.tablas.Table1[0].IdUsuario);

                    if (id_x == "-2")
                    {
                        userTxt.Focus();
                        passTxt.Focus();
                        btnAuth.Text = "Intentar de nuevo";
                        res_Label.Text = "Hubo un problema como los datos de acceso";
                        res_Label_api.Text = "Usuario o contraseña no validos";
                    }
                    else
                    {

                        try
                        {
                            member = new Member();
                            memberdatabase = new MemberDatabase();
                            member.UserName = userName;
                            member.Pass = pass;
                            member.Token_Type = tok_ty;
                            member.Access_Token = acc_tok;
                            member.ID = idid;

                            memberdatabase.AddMember(member);

                        }
                        catch (Exception ex)
                        {
                            res_Label.Text = "Hubo un problema como los datos de acceso: " + ex.ToString();
                            res_Label_api.Text = "Usuario o contraseña no validos";

                        }



                        Application.Current.MainPage = new NavigationPage(new PageNav());

                    }



                    break;

                case (System.Net.HttpStatusCode.BadRequest):
                    res_Label_api.Text = "Error 400";
                    break;

                case (System.Net.HttpStatusCode.Forbidden):
                    res_Label_api.Text = "Error 403";
                    break;
                //500
                case (System.Net.HttpStatusCode.InternalServerError):
                    string status = "Nuestros servidores estan en mantenimiento";
                    res_Label_api.Text = status;
                    break;
            }


            waitActIndicator.IsRunning = false;
            btnAuth.IsEnabled = true;
            return;
        }
    }
}