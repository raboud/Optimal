using Newtonsoft.Json;
using Square.Connect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary2.Models.Square
{
	public class CatalogItemDTO
	{
		private CatalogItem _CatalogItem;

		public CatalogItem GetCatalogItem()
		{
			return this._CatalogItem;
		}

		public void SetCatalogItem(CatalogItem  item)
		{
			this._CatalogItem = item;
		}
	}

	public class CatalogItemVariationDTO : CatalogItemVariation
	{
		public string Option1
		{
			get
			{
				this.init();
				return this._userData.Option1;
			}
			set
			{
				this.init();
				this._userData.Option1 = value;
				this.update();
			}
		}

		public string Option2
		{
			get
			{
				this.init();
				return this._userData.Option2;
			}
			set
			{
				this.init();
				this._userData.Option2 = value;
				this.update();
			}
		}

		public string Option3
		{
			get
			{
				this.init();
				return this._userData.Option3;
			}
			set
			{
				this.init();
				this._userData.Option3 = value;
				this.update();
			}
		}

		public decimal? Cost
		{
			get
			{
				this.init();
				return this._userData.Cost;
			}
			set
			{
				this.init();
				this._userData.Cost = value;
				this.update();
			}
		}

		private userData _userData;

		private void update()
		{
			this.UserData = JsonConvert.SerializeObject(this._userData, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
		}
		private bool _initialized = false;
		private void init()
		{
			if (!_initialized)
			{
				this._userData = JsonConvert.DeserializeObject<userData>(this.UserData);
				this._initialized = true;
			}

		}

		private class userData
		{
			public string Option1 { get; set; }
			public string Option2 { get; set; }
			public string Option3 { get; set; }
			public decimal? Cost { get; set; }
		}

	}
}
