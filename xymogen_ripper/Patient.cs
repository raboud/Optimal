using Newtonsoft.Json;

namespace xymogen_ripper
{
    public partial class Patient
	{
		[JsonProperty("Patient No")]
		public long PatientNo { get; set; }

		[JsonProperty("Last Name")]
		public string LastName { get; set; }

		[JsonProperty("First Name")]
		public string FirstName { get; set; }

		[JsonProperty("Address1", NullValueHandling = NullValueHandling.Ignore)]
		public string Address1 { get; set; }

		[JsonProperty("Address2", NullValueHandling = NullValueHandling.Ignore)]
		public string Address2 { get; set; }

		[JsonProperty("City", NullValueHandling = NullValueHandling.Ignore)]
		public string City { get; set; }

		[JsonProperty("State", NullValueHandling = NullValueHandling.Ignore)]
		public string State { get; set; }

		[JsonProperty("Zip", NullValueHandling = NullValueHandling.Ignore)]
		public string Zip { get; set; }

		[JsonProperty("Home Phone", NullValueHandling = NullValueHandling.Ignore)]
		public string HomePhone { get; set; }

		[JsonProperty("Cell Phone", NullValueHandling = NullValueHandling.Ignore)]
		public string CellPhone { get; set; }

		[JsonProperty("EMail", NullValueHandling = NullValueHandling.Ignore)]
		public string EMail { get; set; }

		[JsonProperty("Other Phone", NullValueHandling = NullValueHandling.Ignore)]
		public string OtherPhone { get; set; }

		[JsonProperty("Work Phone", NullValueHandling = NullValueHandling.Ignore)]
		public string WorkPhone { get; set; }
	}




}
