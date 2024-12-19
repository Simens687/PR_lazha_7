using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Bibl
{
    public static class GlobalVariables
    {
        public static int idUser { get; set; }
        public static int idChangedUser {  get; set; }

        public static DataRowView changedUser { get; set;}

        public static int idChangedAuthor { get; set; }

        public static DataRowView changedAuthor { get; set; }

        public static int idChangedGenre { get; set; }

        public static DataRowView changedGenre { get; set; }
    }
}
