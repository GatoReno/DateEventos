using DateEventos.Models;
using DatePickerService.Classes;
using DatePickerService.Models;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using Plugin.Vibrate;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace DateEventos
{
	public partial class App : Application
    {

        public MemberDatabase memberDatabase;
        public Member member;
        public SQLiteConnection conn;

        public App()
        {
            InitializeComponent();

            MainPage =// new NavigationPage(new AuthPage());
                      // new NavigationPage(new PageNav());
                 new NavigationPage(new SP());
            //new  NavigationPage(new DatePickerPage());
            // new DateEventos.MainPage();

          
           
        }

        

        protected override void OnStart()
        {
            // Handle when your app starts
            memberDatabase = new MemberDatabase();
            var members = memberDatabase.GetMembers();
            // checkar si existe user
            int RowCount = 0;
            int membcount = members.Count();
            RowCount = Convert.ToInt32(membcount);
            if (RowCount > 0)
            {
                var member = members.FirstOrDefault();
                int id;
                id = member.ID;
                //obtener registros en el servidor
                    GetCitasUser(id);
                    GetPendientes(id);
            }
        }
        
        protected override void OnSleep()
        {
            Debug.WriteLine("-------------------------------------Slepping-Madafawkers------------------------------------");

            var startTimeSpan = TimeSpan.Zero;

            var periodTimeSpan_ = TimeSpan.FromSeconds(45);

            var timer_ = new System.Threading.Timer((e) =>
            {
                CheckCitas();
            }, null, startTimeSpan, periodTimeSpan_);

            memberDatabase = new MemberDatabase();
            var members = memberDatabase.GetMembers();
            // checkar si existe user
            int RowCount = 0;
            int membcount = members.Count();
            RowCount = Convert.ToInt32(membcount);
            if (RowCount > 0)
            {
                var member = members.FirstOrDefault();
                int id;
                id = member.ID;
                //obtener registros en el servidor                
                GetPendientes(id);
            }


            

        }
        public void CheckCitas()
        {

            Debug.WriteLine("-------------------------------------Eventos checados------------------------------------");
            // Handle when your app sleeps
            List<Cita> citas = memberDatabase.GetCitasL();
            int citCount = citas.Count();
            foreach (Cita ct in citas)
            {
                var dt = ct.fecha;
                var ft = ct.hora;
                var idx = ct.ID;
                var razon = ct.razon;
                var desc = ct.descripcion;
                
                //  string ft_f = TimeSpan.Parse(ft).ToString();

                
                if (ft == "11:0")
                {
                    ft = ft + "0";
                }
                int ft_len = ft.Length;

                switch (ft_len)
                {
                    case (5):
                        if (DateTime.TryParseExact(dt, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal, out DateTime datetime_))
                        {
                            dt = datetime_.ToString("dd-MM-yyyy"); //hay que probar este y el de 4
                        }
                        var chckdrx = dt + " " + ft;
                        DateTime dtchckx = DateTime.ParseExact(chckdrx.Trim(), "dd-MM-yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture);


                        
                        DateTime nowx = DateTime.Now;
                        var result_tx = DateTime.Compare(dtchckx, nowx);

                        var tm10x = dtchckx - nowx  ;
                        var tmm = tm10x.Minutes;
                        if (tmm < 0)
                        {
                            memberDatabase.DeleteCita(idx);
                            Debug.WriteLine("---------------------------------------Alarma eliminada -------------------------------------");
                        }
                        else
                        {
                            if (tmm <= 5)
                            {
                                var v = CrossVibrate.Current;
                                v.Vibration(TimeSpan.FromSeconds(3));
                                CrossLocalNotifications.Current.Show("Faltan " + tmm + " minutos para tu cita " + ct.razon, "Gracias");
                            }

                            if (tmm == 0)
                            {
                                //var ctI = new CitaInfo(idx);
                                //ctI.RingAlarm();


                                var v = CrossVibrate.Current;
                                v.Vibration(TimeSpan.FromSeconds(12));
                                Debug.WriteLine("-----------------------------------------------Alarma Sonando-------------------------------------");
                                CrossLocalNotifications.Current.Show("Tú cita " + razon + " es ahora !!!", "Entiendo");

                                DependencyService.Get<IAudio>().PlayAudioFile("softAlarm.mp3");


                                try
                                {
                                    Debug.WriteLine("-----------------------------------------------Expirando cita-------------------------------------");
                                    var ctExp = new CitaExp();
                                    ctExp.descripcion = desc;
                                    ctExp.fecha = dt;
                                    ctExp.hora = ft;
                                    ctExp.ID = idx;
                                    ctExp.razon = razon;

                                    memberDatabase.AddCitaExp(ctExp);
                                    memberDatabase.DeleteCita(idx);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("--------------------------Error_" + ex.ToString() + "-------------------------------------");
                                    throw;
                                }
                            }
                        }
                      
                      
                        
                        break;
                    case (3):

                        string outputdt = ft.Substring(ft.Length - 1, 1);
                        if (outputdt == "0")
                        {
                            ft = ft + "0";
                        }

                        if (DateTime.TryParseExact(dt, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal, out DateTime datetime))
                        {
                            dt = datetime.ToString("dd-MM-yyyy");
                        }
                        var chckdr = dt + " " + ft;
                        DateTime dtchck = DateTime.ParseExact(chckdr.Trim(), "dd-MM-yyyy H:mm",
                        System.Globalization.CultureInfo.InvariantCulture);




                        DateTime now = DateTime.Now;
                        var result_t = DateTime.Compare(dtchck, now);

                        var tm10 = dtchck - now;
                        var tmmi = tm10.Minutes;
                        if (tmmi < 0)
                        {
                            memberDatabase.DeleteCita(idx);
                            Debug.WriteLine("---------------------------------------Alarma eliminada -------------------------------------");

                        }
                        else
                        {
                            if (tmmi <= 5)
                            {
                                var v = CrossVibrate.Current;
                                v.Vibration(TimeSpan.FromSeconds(3));
                                CrossLocalNotifications.Current.Show("Faltan " + tmmi + " minutos para tu cita " + ct.razon, "Gracias");
                            }

                            if (tmmi == 0)
                            {
                                var v = CrossVibrate.Current;
                                v.Vibration(TimeSpan.FromSeconds(12));
                                Debug.WriteLine("-----------------------------------------------Alarma Sonando-------------------------------------");
                                CrossLocalNotifications.Current.Show("Tú cita " + razon + " es ahora !!!", "Entiendo");

                                DependencyService.Get<IAudio>().PlayAudioFile("softAlarm.mp3");


                                try
                                {
                                    Debug.WriteLine("-----------------------------------------------Expirando cita-------------------------------------");
                                    var ctExp = new CitaExp();
                                    ctExp.descripcion = desc;
                                    ctExp.fecha = dt;
                                    ctExp.hora = ft;
                                    ctExp.ID = idx;
                                    ctExp.razon = razon;

                                    memberDatabase.AddCitaExp(ctExp);
                                    memberDatabase.DeleteCita(idx);

                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("--------------------------Error_" + ex.ToString() + "-------------------------------------");

                                    throw;
                                }
                            }
                        }

                        break;

                    case (4):

                        //string outputdtx = ft.Substring(ft.Length - 1, 1);
                        //if (outputdtx == "0")
                        //{
                        //    ft = ft + "0";
                        //}

                        if (DateTime.TryParseExact(dt, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal, out DateTime datetimex))
                        {
                            dt = datetimex.ToString("dd-MM-yyyy");
                        }
                        var chckdrxx = dt + " " + ft;
                        DateTime dtchckxx = DateTime.ParseExact(chckdrxx.Trim(), "dd-MM-yyyy H:mm",
                        System.Globalization.CultureInfo.InvariantCulture);


                      

                        DateTime nowxx = DateTime.Now;
                        var result_txx = DateTime.Compare(dtchckxx, nowxx);

                        var tm10xx = dtchckxx - nowxx;
                        var tmmix = tm10xx.Minutes;
                        if (tmmix < 0)
                        {
                            memberDatabase.DeleteCita(idx);
                            Debug.WriteLine("---------------------------------------Alarma eliminada -------------------------------------");

                        }
                        else{
                             if (tmmix <= 5)
                        {
                            var v = CrossVibrate.Current;
                            v.Vibration(TimeSpan.FromSeconds(3));
                            CrossLocalNotifications.Current.Show("Faltan " + tmmix + " minutos para tu cita " + ct.razon, "Gracias");
                        }

                        if (tmmix == 0)
                        {
                            var v = CrossVibrate.Current;
                            v.Vibration(TimeSpan.FromSeconds(12));
                            Debug.WriteLine("-----------------------------------------------Alarma Sonando-------------------------------------");
                            CrossLocalNotifications.Current.Show("Tú cita " + razon + " es ahora !!!", "Entiendo");

                            DependencyService.Get<IAudio>().PlayAudioFile("softAlarm.mp3");
                            

                            try
                            {
                                Debug.WriteLine("-----------------------------------------------Expirando cita-------------------------------------");
                                var ctExp = new CitaExp();
                                ctExp.descripcion = desc;
                                ctExp.fecha = dt;
                                ctExp.hora = ft;
                                ctExp.ID = idx;
                                ctExp.razon = razon;

                                memberDatabase.AddCitaExp(ctExp);
                                memberDatabase.DeleteCita(idx);

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("--------------------------Error_" + ex.ToString() + "-------------------------------------");

                                throw;
                            }
                        }
                        }
                       

                        
                       
                        break;
                }





            }

        }
        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public async void GetCitasUser(int id)
        {

            Debug.WriteLine("-----------------------------------------------WORKED-------------------------------------");



            var uri = "http://aige.sytes.net/APIRESTSAM/api/citasapp/getcitas?id=" + id;
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
                    memberDatabase = new MemberDatabase();
                    memberDatabase.DropCitaT();
                    memberDatabase.CreateCitaT();
                    try
                    {
                        Root myobject = JsonConvert.DeserializeObject<Root>(xjson);

                        int myobjcount = myobject.tablas.Table.Count;
                        for (int i = 0; i < myobjcount; i++)
                        {
                            var citx = new Cita();
                            citx.ID = myobject.tablas.Table[i].idevento;
                            citx.razon = myobject.tablas.Table[i].razon;
                            citx.descripcion = myobject.tablas.Table[i].descripcion; citx.fecha = myobject.tablas.Table[i].fecha;

                            citx.hora = myobject.tablas.Table[i].hora;
                            memberDatabase.AddCita(citx);
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("---------------------------500---------------------error" + ex);
                        return;
                    }

                    break;
                //500
                case (System.Net.HttpStatusCode.InternalServerError):

                    Debug.WriteLine("---------------------------500---------------------error");


                    break;
                //404
                case (System.Net.HttpStatusCode.Forbidden):
                    Debug.WriteLine("---------------------------404---------------------error");
                    break;

            }
        }
        public async void GetPendientes(int myId)
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
                        if (px > 0)
                        {
                            CrossLocalNotifications.Current.Show("Invitaciones pendientes", "Usted tiene " + px.ToString() + " invitaciones por confirmar");
                        }
                        else {
                            Debug.WriteLine("-------------Pendientes buscados sin éxito------------------------------------");
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("---------------------------200Error---------------------" + ex.ToString());
                        return;
                    }

                    break;
                //500
                case (System.Net.HttpStatusCode.InternalServerError):
                    Debug.WriteLine("---------------------------500---------------------error");
                    break;
                //404
                case (System.Net.HttpStatusCode.Forbidden):
                    Debug.WriteLine("---------------------------404---------------------error");
                    break;

            }
        }
    }
}
