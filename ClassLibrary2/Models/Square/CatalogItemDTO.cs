using Newtonsoft.Json;
using Square.Connect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONWLibrary.Models.Square
{
	public class CatalogItemDTO
	{
		private CatalogItem _CatalogItem;

		public CatalogItem GetCatalogItem()
		{
			return _CatalogItem;
		}

		public void SetCatalogItem(CatalogItem  item)
		{
			_CatalogItem = item;
		}
	}

	public class CatalogItemVariationDTO : CatalogItemVariation
	{
		public string Option1
		{
			get
			{
				Init();
				return _userData.Option1;
			}
			set
			{
				Init();
				_userData.Option1 = value;
				Update();
			}
		}

		public string Option2
		{
			get
			{
				Init();
				return _userData.Option2;
			}
			set
			{
				Init();
				_userData.Option2 = value;
				Update();
			}
		}

		public string Option3
		{
			get
			{
				Init();
				return _userData.Option3;
			}
			set
			{
				Init();
				_userData.Option3 = value;
				Update();
			}
		}

		public decimal? Cost
		{
			get
			{
				Init();
				return _userData.Cost;
			}
			set
			{
				Init();
				_userData.Cost = value;
				Update();
			}
		}

		private UserData2 _userData;

		private void Update()
		{
			UserData = JsonConvert.SerializeObject(_userData, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
		}
		private bool _initialized = false;
		private void Init()
		{
			if (!_initialized)
			{
				_userData = JsonConvert.DeserializeObject<UserData2>(UserData);
				_initialized = true;
			}

		}

		private class UserData2
		{
			public string Option1 { get; set; }
			public string Option2 { get; set; }
			public string Option3 { get; set; }
			public decimal? Cost { get; set; }
		}

	}
}
