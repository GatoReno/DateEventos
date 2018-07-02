using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatePickerService.Models
{
    public class Member
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string idusuario { get; set; }
        public string UserName { get; set; }
        public string Pass { get; set; }
        public string Access_Token { set; get; }
        public string Token_Type { set; get; }

        public Member()

        {
        }
    }
}
