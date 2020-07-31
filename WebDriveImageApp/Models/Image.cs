using Google.Apis.Oauth2.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebDriveImageApp.Models
{
    public class Image
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public long? Size { get; set; }
        public string ImageLink { get; set; }


    }

}
