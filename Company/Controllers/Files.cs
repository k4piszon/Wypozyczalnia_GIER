using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Company.Controllers
{
    public class Files
    {
        String Path { get; set; }

        public Files(string path)
        {
            Path = path;
        }
       public void RemoveFile()
       {

            if (File.Exists($@"{Path}"))
            {
                File.Delete($@"{Path}");
                //ViewBag.deleteSuccess = "true";
            }
       }


    }
}