using System;
using System.Collections.Generic;
using System.Text;

namespace DatePickerService.Models
{
    public class Table1
    {
        public int IdUsuario { get; set; }
        public int Eventoid { get; set; }
        public string Longitud { get; set; }
        public string Latitud { get; set; }
        public string FechaAlta { get; set; }
    }

    public class Table
    {
        public int IdUsuario { get; set; }
        public int Eventoid { get; set; }
        public int eventoid { get; set; }
        public int idevento { get; set; }
        public string nombreusuario { get; set; }
        public string descripcion { get; set; }
        public string fecha { get;set; }
        public string hora { get; set; }
        public string nombre { get; set; }
        public string email { set; get; }
        public int telefono { set; get; }
        public string idusuario { get; set; }
        
        public int pendientes { get; set; }
        public string Nombre { get; set; }
        public string Longitud { get; set; }
        public string Latitud { get; set; }
        public string FechaAlta { get; set; }
        public int admin { get; set; }
        public int conf { get; set; }
        public string razon { set; get; }
     
    }

    public class Tablas
    {
        public List<Table1> Table1 { get; set; }
        public List<Table> Table { get; set; }
    }

    public class Root
    {
        public object DatosEnvio { get; set; }
        public object DatosEnvioJson { get; set; }
        public object DatosEnvioJsonDatos { get; set; }
        public object DatosEnvioJsonTitulos { get; set; }
        public object tabla { get; set; }
        public Tablas tablas { get; set; }
        public string bandera { get; set; }
        public string mensaje { get; set; }
    }
}
