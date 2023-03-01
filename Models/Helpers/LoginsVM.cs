using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TilausJärjestelmä.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web;
    using System.Web.UI.WebControls;

    public partial class LoginsVM
    {
        public LoginsVM() { } //Default konstruktori kirjautumista varten
        private LoginsVM(int LoginId_in, string UserName_in, int Level_in)   //Custom private konstruktori käyttäjien esittämistä varten
        {
            LoginId = LoginId_in;
            UserName = UserName_in;
            Level = Level_in;
        }


        public int LoginId { get; set; }
        [DisplayName("Käyttäjätunnus")]
        [Required(ErrorMessage = "Anna käyttäjätunnus!")]
        public string UserName { get; set; }
        [DisplayName("Salasana")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Anna salasana!")]
        public string PassWord { get; set; }
        public int Level { get; set; }
        public string LoginErrorMessage { get; set; }



        static public List<LoginsVM> GetUserList(string ActiveUser) //Factory funktio, hakee kaikki paitsi kirjautuneen käyttäjän
        {
            TilausDBEntities db = new TilausDBEntities();
            List<LoginsVM> p = new List<LoginsVM>();

            var users = from i in db.Logins
                            where i.UserName != ActiveUser
                            select i;

            foreach (var item in users)
            {
                p.Add(new LoginsVM(item.LoginId, item.UserName, item.Level));
            }

            db.Dispose();
            return p;
        }

        static public int GetLoginId(TilausDBEntities entity) //Hakee uusimmasta id:stä seuraavan id:n
        {
            if (entity.Logins.Count() == 0)
            {
                return 1000;
            }
            else
            {
                var log = from i in entity.Logins
                             select i.LoginId;

                return log.Max() + 1;
            }
        }

    }
}