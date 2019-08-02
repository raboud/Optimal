using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandR.Utilities.Extensions;

namespace ONWLibrary.Models
{
    public partial class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }

        public string GetFormattedSiteAddress()
        {
            StringBuilder formattedSiteAddress = new StringBuilder();

            formattedSiteAddress.AppendLine(this.Line1?.Trim());
            if (this.Line2.IsValid())
            {
                formattedSiteAddress.AppendLine(this.Line2.SafeTrim());
            }
            formattedSiteAddress.AppendFormat("{0}, {1}  {2}\r\n", this.City.SafeTrim(),
                                                this.State.SafeTrim(), this.ZipCode.SafeTrim());

            return formattedSiteAddress.ToString();
        }


    }
}
