using ShopifySharp;
using ShopifySharp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ONWLibrary
{
	public static class Shopify
	{
		private static string _myStore { get; set; } = "https://optimalnw.myshopify.com";
		private static string _access { get; set; } = "3912f9da28ad92b17c79bf734b25246f";

		private static CustomerService CustomerService { get { return new CustomerService(_myStore, _access); } }
		private static CustomCollectionService CustomCollectionService { get { return new CustomCollectionService(_myStore, _access); } }
		private static CollectService CollectService { get { return new CollectService(_myStore, _access); } }
//		private static ProductImageService ProductImageService { get { return new ProductImageService(_myStore, _access); } }
		private static ProductService ProductService { get { return new ShopifySharp.ProductService(_myStore, _access); } }
		private static MetaFieldService MetaFieldService { get { return new MetaFieldService(_myStore, _access); } }
		private static OrderService OrderService { get { return new OrderService(_myStore, _access); } }

		static public void Init()
		{
			System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ShopifyService.SetGlobalExecutionPolicy(new SmartRetryExecutionPolicy());
		}

		static public Task<Order> OpenAsync(this Order o)
		{
			return OrderService.OpenAsync(o.Id.Value);
		}

		static public Task<Order> CloseAsync(this Order o)
		{
			return OrderService.CloseAsync(o.Id.Value);
		}

		static public async Task<List<Order>> GetOrdersAsync()
		{
			List<Order> data = new List<Order>();
			try
			{
				OrderFilter filter = new OrderFilter()
				{
					Status = "closed"
				};

				int count = await OrderService.CountAsync(filter).ConfigureAwait(false);

				int pages = (count / 250) + 1;
				for (int page = 1; page <= pages; page++)
				{
					filter.Limit = 250;
					filter.Page = page;
					data.AddRange(await OrderService.ListAsync(filter).ConfigureAwait(false));
				}
			}
			catch (Exception e)
			{

			}
			return data;
		}

		static public Task<Order> UpdateAsync(this Order p)
		{
			return OrderService.UpdateAsync(p.Id.Value, p);
		}

		static public async Task<List<Product>> GetProductsAsync()
		{
			List<Product> products = new List<Product>();
			try
			{
				int count = await ProductService.CountAsync().ConfigureAwait(false);
				int pages = (count / 250) + 1;
				for (int page = 1; page <= pages; page++)
				{
					ProductFilter filter = new ProductFilter
					{
						Limit = 250,
						Page = page
					};
					products.AddRange(await ProductService.ListAsync(filter).ConfigureAwait(false));
				}
			}
			catch (Exception e)
			{

			}
			return products;
		}

		static public Task<Product> CreateProductAsync(Product p)
		{
			return ProductService.CreateAsync(p);
		}

		static public Task<Product> UpdateProductAsync(Product p)
		{
			return ProductService.UpdateAsync(p.Id.Value, p);
		}

		static public List<Customer> GetCustomers()
		{
			List<Customer> customers = new List<Customer>();
			int count = CustomerService.CountAsync().Result;
			int pages = (count / 250) + 1;
			for (int page = 1; page <= pages; page++)
			{
				ListFilter filter = new ListFilter
				{
					Limit = 250,
					Page = page
				};
				customers.AddRange(CustomerService.ListAsync(filter).Result);
			}
			return customers;
		}

		static public Customer CreateCustomer(Customer c)
		{
			return CustomerService.CreateAsync(c).Result;
		}

		static public Customer UpdateCustomer(Customer p)
		{
			return CustomerService.UpdateAsync(p.Id.Value, p).Result;
		}

		static public bool CreateProductMetadata(long productId, string ns, string key, object value, string description = null)
		{
			return CreateMetadata("products", productId, ns, key, value, description);
		}

		static public bool CreateCustomerMetadata(long customerId, string ns, string key, object value, string description = null)
		{
			return CreateMetadata("customer", customerId, ns, key, value, description);
		}

		static public bool CreateMetadata(string type, long id, string ns, string key, object value, string description = null)
		{
			if (value == null)
			{
				return false;
			}
			string valueType;
			if (value is int)
			{
				valueType = "integer";
			}
			else if (value is string)
			{
				valueType = "string";
				if (string.IsNullOrEmpty(value as string))
				{
					return false;
				}
			}
			else
			{
				return false;
			}
			MetaField meta = new MetaField()
			{
				Namespace = ns,
				Key = key,
				Value = value,
				ValueType = valueType
			};
			MetaField t = MetaFieldService.CreateAsync(meta, id, type).Result;
			IEnumerable<MetaField> metas = MetaFieldService.ListAsync(id, type).Result;
			return true;
		}

		static public Collect AddProductToCollection(Product p, string collection)
		{
			Collect retVal = null;
			if (p.Id.HasValue)
			{
				collection = collection.Trim();
				IEnumerable<CustomCollection> customs = CustomCollectionService.ListAsync().Result;

				CustomCollection cc = customs.FirstOrDefault(c => c.Title == collection);
				if (cc == null)
				{
					cc = CreateCustomCollection(collection);
				}

				Collect collect = new Collect() { ProductId = p.Id.Value, CollectionId = cc.Id.Value };
				retVal = CollectService.CreateAsync(collect).Result;
			}
			return retVal;
		}

		static async public Task<List<CustomCollection>> GetCollections()
		{
			List<CustomCollection> list = new List<CustomCollection>();
			int count = await CustomCollectionService.CountAsync().ConfigureAwait(false);
			int pages = (count / 250) + 1;
			for (int page = 1; page <= pages; page++)
			{
				CustomCollectionFilter filter = new CustomCollectionFilter
				{
					Limit = 250,
					Page = page
				};
				list.AddRange(await CustomCollectionService.ListAsync(filter));
			}
			return list;
		}

		static public CustomCollection CreateCustomCollection(string collection)
		{
			CustomCollectionService service = CustomCollectionService;

			collection = collection.Trim();
			IEnumerable<CustomCollection> customs = service.ListAsync().Result;

			CustomCollection cc = customs.FirstOrDefault(c => c.Title == collection && c.PublishedScope != "global");
			if (cc != null)
			{
				//				service.DeleteAsync(cc.Id.Value).Wait();
			}


			//			customs = service.ListAsync().Result;
			if (customs.All(c => c.Title != collection))
			{
				string handle = collection.Replace(" ", "").Replace('&', '-');
				cc = new CustomCollection()
				{
					Title = collection,
					Handle = handle,
					PublishedScope = "global",
					BodyHtml = "",
					Published = true,
					PublishedAt = DateTime.UtcNow,
					SortOrder = "alpha-asc"

				};
				cc = service.CreateAsync(cc).Result;
			}
			return cc;
		}



	}




}
