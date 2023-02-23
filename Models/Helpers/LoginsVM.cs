using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TilausJärjestelmä.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class LoginsVM
    {
        public int LoginId { get; set; }
        [DisplayName("Käyttäjätunnus")]
        [Required(ErrorMessage = "Anna käyttäjätunnus!")]
        public string UserName { get; set; }
        [DisplayName("Salasana")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Anna salasana!")]
        public string PassWord { get; set; }
        public int? Level { get; set; }  //ei ole yhdistetty
        public string LoginErrorMessage { get; set; }
    }
}