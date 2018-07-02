using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatePickerService.Models
{
    public class Cita
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string razon { get; set; }
        public string descripcion { set; get; }
        public string fecha { set; get; }
        public string hora { set; get; }
    }
}
