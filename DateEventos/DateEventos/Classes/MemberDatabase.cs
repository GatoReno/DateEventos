using DateEventos;
using DateEventos.Models;
using DatePickerService.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DatePickerService.Classes
{
    public class MemberDatabase
    {
        private SQLiteConnection conn;

        public MemberDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<Member>();
            conn.CreateTable<Cita>();
            conn.CreateTable<CitaExp>();
        }



        //Member / local user CRUD
        public string AddMember(Member member)
        {
            conn.Insert(member);
            return "success baby bluye ;*";
        }

     
        public void DeleteMember(int ID)
        {
            conn.Delete<Member>(ID);
        }


        public IEnumerable<Member> GetMembers()
        {
            var members = (from mem in conn.Table<Member>() select mem);
            return members.ToList();
        }

        //CitasExp CRUD local

        public string AddCitaExp(CitaExp citaEx) {
            conn.Insert(citaEx);
            return "success :3";
        }

        public void DeleteCitaExp(int ID)
        {
            conn.Delete<CitaExp>(ID);
        }
        public string DropCitaExpT()
        {

            conn.DropTable<CitaExp>();
            return "Citas eliminadas";
        }
        public List<CitaExp> GetCitasExpL()
        {
            var citasxExp = (from cit in conn.Table<CitaExp>() select cit);
            return citasxExp.ToList();
        }
        //Citas CRUD local
        public string AddCita(Cita cita)
        {
            conn.Insert(cita);
            return "success :3 ";
        }

        public void DeleteCita(int ID)
        {
            conn.Delete<Cita>(ID);
        }
        public Cita CitaUnica(int ID) {
            Cita citaunica = conn.Table<Cita>().FirstOrDefault( ci => ci.ID == ID );
            return citaunica;
        }
        public IEnumerable<Cita> GetCitas()
        {
            var citasx = (from cit in conn.Table<Cita>() select cit);
            return citasx.ToList();
        }

        public List<Cita> GetCitasL() {
            var citasx = (from cit in conn.Table<Cita>() select cit);
            return citasx.ToList();
        }
        public string CreateCitaT() {
            conn.CreateTable<Cita>();
            return "Tabla de citas creada";
        }
        public string DropCitaT() {

            conn.DropTable<Cita>();
            return "Citas eliminadas";
        }





    }
}
