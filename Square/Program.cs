using Newtonsoft.Json;
using ONWLibrary;
using Square.Connect.Api;
using Square.Connect.Client;
using Square.Connect.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Square
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			// Configure OAuth2 access token for authorization: oauth2
			string HMS = "sq0atp-M5ylQzjwtUqHFV_1aZBkWw";
			string TEST = "sq0atp-NbIspn1KqTCzVxmJJblliQ";
			Configuration.Default.AccessToken = HMS;

//			PortCustomersAsync().Wait();
			//WebRequest.DefaultWebProxy = new WebProxy("127.0.0.1", 8888);
			//FixLocations().Wait();
			List<ShopifySharp.Product> products = Shopify.GetProductsAsync().Result;
			//SetInventory(products).Wait();
			//FixBarCodes(products).Wait();
			ListLocationsResponse locations = GetLocations().Result;
            //			DeleteProducts().Wait();
            //			PortItemsAsync(locations.Locations[0].Id).Wait();
            //			PortItemsAsync("me").Wait();
            ListTransactionsResponse transactions = GetTransactions(locations.Locations[0].Id).Result;
            Customer customer = GetCustomer("Raboud", "Robert").Result;
            Customer customer2 = GetCustomer("Raboud", "Carrie").Result;
            GetProductsAsync().Wait();

			//var customerTransactions = transactions.Transactions.Where(t => t.Tenders.Any(te => te.CustomerId == customer.Id)).ToList();
			//foreach (var transaction in customerTransactions)
			//{
			//	foreach (var tender in transaction.Tenders)
			//	{
			//		if (tender.CustomerId == customer.Id)
			//		{
			//			tender.CustomerId = customer2.Id;
			//		}
			//	}
			//}
		}

		static public async Task GetProductsAsync()
		{
			CatalogApi api = new CatalogApi();
			string cursor = null;
//			string[] types = { "ITEM_VARIATION", "MODIFIER_LIST", "ITEM", "MODIFIER", "CATEGORY", "DISCOUNT", "TAX" };
//			foreach (string type in types)
			{
				do
				{
					ListCatalogResponse resp = await api.ListCatalogAsync(cursor, "ITEM");
					if (resp.Objects != null && resp.Objects.Count > 0)
					{
					}
					cursor = resp.Cursor;
				} while (cursor != null);
			}
		}

		static public async Task DeleteProducts()
		{
			CatalogApi api = new CatalogApi();
			string cursor = null;
			string[] types = { "ITEM_VARIATION", "MODIFIER_LIST", "ITEM", "MODIFIER", "CATEGORY", "DISCOUNT", "TAX" };
			foreach (string type in types)
			{
				do
				{
					ListCatalogResponse resp = await api.ListCatalogAsync(cursor, type);
					if (resp.Objects != null && resp.Objects.Count > 0)
					{
						BatchDeleteCatalogObjectsRequest body = new BatchDeleteCatalogObjectsRequest(resp.Objects.Select(s => s.Id).ToList());
						BatchDeleteCatalogObjectsResponse delResp = await api.BatchDeleteCatalogObjectsAsync(body);
					}
					cursor = resp.Cursor;
				} while (cursor != null);
			}
		}

		static public async Task<List<Customer>> GetCustomersAsync()
		{
			List<Customer> customers = new List<Customer>();
			CustomersApi api = new CustomersApi();
			string cursor = null;
			do
			{
				ListCustomersResponse resp = await api.ListCustomersAsync(cursor);
				customers.AddRange(resp.Customers);
				cursor = resp.Cursor;
			} while (cursor != null);
			return customers;
		}

		static public async Task<Customer> GetCustomer(string lastName, string firstName)
		{
			Customer retval = null;
			ListCustomersResponse resp = null;

			CustomersApi api = new CustomersApi();
			string cursor = null;
			do
			{
				resp = await api.ListCustomersAsync(cursor);
				retval = resp.Customers.FirstOrDefault(c => c.FamilyName == lastName && c.GivenName == firstName);
				cursor = resp.Cursor;
			} while (retval == null && resp.Cursor != null);
			return retval;
		}

		static public async Task<ListTransactionsResponse> GetTransactions(string id)
		{
			TransactionsApi api = new TransactionsApi();
			ListTransactionsResponse resp = await api.ListTransactionsAsync(id);
			return resp;
		}

		static public async Task<ListLocationsResponse> GetLocations()
		{
			LocationsApi api = new LocationsApi();

			ListLocationsResponse resp = await api.ListLocationsAsync();
			return resp;

		}

		static public async Task PortCatagories()
		{
			List<ShopifySharp.CustomCollection> list = await Shopify.GetCollections();

			List<CatalogObjectBatch> batches = new List<CatalogObjectBatch>();
			CatalogObjectBatch batch = new CatalogObjectBatch
			{
				Objects = new List<CatalogObject>()
			};
			batches.Add(batch);

			foreach (ShopifySharp.CustomCollection collection in list)
			{
				CatalogObject category = new CatalogObject(
				  Type: "CATEGORY",
				  Id: $"#{collection.Title}",
				  PresentAtAllLocations: true,
				  CategoryData: new CatalogCategory
				  {
					  Name = collection.Title
				  }
				);
				batch.Objects.Add(category);
			}
			CatalogApi api = new CatalogApi();
			BatchUpsertCatalogObjectsRequest body = new BatchUpsertCatalogObjectsRequest(Guid.NewGuid().ToString(), batches);
			BatchUpsertCatalogObjectsResponse response = await api.BatchUpsertCatalogObjectsAsync(body);
		}

		static public async Task<V1Fee> CreateTaxV1(string locationId)
		{
			V1ItemsApi v1 = new V1ItemsApi();
			V1Fee v1Fee = new V1Fee(
				null,
				"Sales Tax",
				".089",
				"FEESUBTOTALPHASE",
				"TAX",
				true,
				true,
				"ADDITIVE",
				"USSALESTAX");
			V1Fee resp = await v1.CreateFeeAsync(locationId, v1Fee);
			return resp;
		}

		static public async Task<string> CreateDiscount(string locationId)
		{
			V1ItemsApi v1 = new V1ItemsApi();
			V1Discount body = new V1Discount(
				null,
				"Employee",
				"0.40",
				null,
				"FIXED",
				false,
				"B21212");
			V1Discount resp = await v1.CreateDiscountAsync(locationId, body);
			return resp.Id;
		}

		static public async Task FixBarCodes(List<ShopifySharp.Product> products)
		{
			CatalogApi api = new CatalogApi();
			string cursor = null;
			List<CatalogObjectBatch> batches = new List<CatalogObjectBatch>();
			CatalogObjectBatch batch = new CatalogObjectBatch
			{
				Objects = new List<CatalogObject>()
			};
			batches.Add(batch);

			do
			{
				ListCatalogResponse resp = await api.ListCatalogAsync(cursor, "ITEM_VARIATION");
				if (resp.Objects != null && resp.Objects.Count > 0)
				{
					batch.Objects.AddRange(resp.Objects);
				}
				cursor = resp.Cursor;
			} while (cursor != null);

			foreach (CatalogObject obj in batch.Objects)
			{
				long id = long.Parse(obj.ItemVariationData.UserData);
				ShopifySharp.Product prod = products.FirstOrDefault(p => p.Variants.Any(v => v.Id == id));
				ShopifySharp.ProductVariant variant = prod.Variants.FirstOrDefault(v => v.Id == id);

//				ShopifySharp.ProductVariant variant = products.Select(p => p.Variants.First(v => v.Id.ToString() == obj.ItemVariationData.UserData)).First();
				obj.ItemVariationData.Upc = variant.Barcode;
				obj.ItemVariationData.Sku = variant.Barcode;
//				obj.PresentAtAllLocations = true;
			}

			BatchUpsertCatalogObjectsRequest body = new BatchUpsertCatalogObjectsRequest(Guid.NewGuid().ToString(), batches);
			BatchUpsertCatalogObjectsResponse response = await api.BatchUpsertCatalogObjectsAsync(body);
		}

		static public async Task SetInventory(List<ShopifySharp.Product> products)
		{
			CatalogApi api = new CatalogApi();
			V1ItemsApi v1api = new V1ItemsApi();
			string cursor = null;

			do
			{
				ListCatalogResponse resp = await api.ListCatalogAsync(cursor, "ITEM_VARIATION");

				foreach (CatalogObject obj in resp.Objects)
				{
					long id = long.Parse(obj.ItemVariationData.UserData);
					ShopifySharp.Product prod = products.FirstOrDefault(p => p.Variants.Any(v => v.Id == id));
					ShopifySharp.ProductVariant variant = prod.Variants.FirstOrDefault(v => v.Id == id);
					V1AdjustInventoryRequest body = new V1AdjustInventoryRequest(variant.InventoryQuantity, "MANUALADJUST", "From Shopify");
					try
					{
						await v1api.AdjustInventoryAsync(obj.CatalogV1Ids[0].LocationId, obj.CatalogV1Ids[0]._CatalogV1Id, body);
					}
					catch (Exception)
					{

					}
					//				obj.PresentAtAllLocations = true;
				}

				cursor = resp.Cursor;
			} while (cursor != null);

			
		}

		static public async Task FixLocations()
		{
			CatalogApi api = new CatalogApi();
			string cursor = null;

			string[] types = { "MODIFIER_LIST", "ITEM", "MODIFIER", "CATEGORY", "DISCOUNT", "TAX", "ITEM_VARIATION" };
			foreach (string type in types)
			{
                List<CatalogObjectBatch> batches = new List<CatalogObjectBatch>();
                CatalogObjectBatch batch = new CatalogObjectBatch
				{
					Objects = new List<CatalogObject>()
				};
				batches.Add(batch);
				do
				{
					ListCatalogResponse resp = await api.ListCatalogAsync(cursor, type);
					if (resp.Objects != null && resp.Objects.Count > 0)
					{
						batch.Objects.AddRange(resp.Objects.Where(o => o.PresentAtAllLocations == false));
					}
					cursor = resp.Cursor;
				} while (cursor != null);

				foreach (CatalogObject obj in batch.Objects)
				{
					obj.PresentAtAllLocations = true;
				}
                BatchUpsertCatalogObjectsRequest body = new BatchUpsertCatalogObjectsRequest(Guid.NewGuid().ToString(), batches);
                BatchUpsertCatalogObjectsResponse response = await api.BatchUpsertCatalogObjectsAsync(body);
            }


        }

		static public async Task PortItemsAsync(string locationId)
		{
			V1Fee tax = await CreateTaxV1(locationId);
			string discoutId = await CreateDiscount(locationId);

			List<ShopifySharp.Product> products = await Shopify.GetProductsAsync();

			V1ItemsApi v1 = new V1ItemsApi();

			foreach (ShopifySharp.Product prod in products)
			{
				System.Console.WriteLine(prod.Title);
				V1Item item = new V1Item(
					Name: prod.Title,
					Type: "NORMAL",
					Visibility: "PUBLIC",
					AvailableOnline: true,
					Variations: new List<V1Variation>(),
					Taxable: true,
					Fees: new List<V1Fee>() { tax }
				);
				foreach (ShopifySharp.ProductVariant variant in prod.Variants)
				{
					V1Variation vari = new V1Variation
					(
						Name: variant.Title,
						PricingType: "FIXEDPRICING",
						PriceMoney: new V1Money(
								Amount: variant.Price.HasValue ? ((int?)(variant.Price.Value * 100L)) : null,
								CurrencyCode: "USD"
							),
						TrackInventory: true,
						UserData: variant.Id.ToString()
					);
					item.Variations.Add(vari);
				}
				V1Item item2 = await v1.CreateItemAsync(locationId, item);
				ShopifySharp.ProductImage image = prod.Images.FirstOrDefault();
				if (image != null)
				{
					await ImageUploader.PortImage(locationId, item2.Id, prod.Images.First().Src);
				}
			}
//			await SetInventory(products);
			await FixBarCodes(products);
			await FixLocations();
		}

		static public async Task PortItemsAsync2(string locationId)
		{
			List<ShopifySharp.Product> products = await Shopify.GetProductsAsync();

			List<CatalogObjectBatch> batches = new List<CatalogObjectBatch>();
			CatalogObjectBatch batch = new CatalogObjectBatch
			{
				Objects = new List<CatalogObject>()
			};
			batches.Add(batch);

			CatalogObject tax = new CatalogObject(
			  "TAX",
			  Id: $"#salestax",
			  PresentAtAllLocations: true,
			  TaxData: new CatalogTax
			  {
				  Name = "SalesTax",
				  AppliesToCustomAmounts = true,
				  CalculationPhase = "SUBTOTALPHASE",
				  Enabled = true,
				  InclusionType = "ADDITIVE",
				  Percentage = "8.9"
			  }
			);

			CatalogObject employee = new CatalogObject(
			  Type: "DISCOUNT",
			  Id: $"#employee",
			  PresentAtAllLocations: true,
			  DiscountData: new CatalogDiscount
			  {
				  Name = "Employee",
				  DiscountType = "FIXEDPERCENTAGE",
				  LabelColor = "Red",
				  PinRequired = false,
				  Percentage = "40"
			  }
			);

			batch.Objects.Add(tax);
			foreach (ShopifySharp.Product prod in products)
			{
				CatalogObject obj = new CatalogObject(
					Type: "ITEM",
					Id: $"#{prod.Title}",
					PresentAtAllLocations: true,
					ItemData: new CatalogItem
					{
						Name = prod.Title,
						//						Description = prod.,
						//					CategoryId = "#Beverages",
						TaxIds = new List<string>() { "#salestax" },
						Variations = new List<CatalogObject>()
					}
				);
				foreach (ShopifySharp.ProductVariant variant in prod.Variants)
				{
					CatalogObject vari = new CatalogObject
					(
						Type: "ITEMVARIATION",
						Id: $"#{prod.Title}-{variant.Title}",
						PresentAtAllLocations: true,
						ItemVariationData: new CatalogItemVariation()
						{
							UserData = variant.Id.ToString(),
							Upc = variant.Barcode,
							ItemId = $"#{prod.Title}",
							Name = variant.Title,
							TrackInventory = true,
							PricingType = "FIXEDPRICING",
							PriceMoney = new Money(
								Amount: variant.Price.HasValue ? ((long?)(variant.Price.Value * 100L)) : null,
								Currency: "USD"
							)
						}
					);
					obj.ItemData.Variations.Add(vari);
				}
				batch.Objects.Add(obj);
			}

			CatalogApi api = new CatalogApi();

			BatchUpsertCatalogObjectsRequest body = new BatchUpsertCatalogObjectsRequest(Guid.NewGuid().ToString(), batches);
			BatchUpsertCatalogObjectsResponse response = await api.BatchUpsertCatalogObjectsAsync(body);

			foreach (CatalogObject item in response.Objects.Where(o => o.Type == "ITEM"))
			{
				if (!string.IsNullOrEmpty(item.ItemData.Variations?[0].ItemVariationData.UserData))
				{
					long oldId = long.Parse(item.ItemData.Variations?[0].ItemVariationData.UserData);
					ShopifySharp.Product prod = products.FirstOrDefault(p => p.Variants.Any(v => v.Id == oldId));

					await ImageUploader.PortImage(locationId, item.Id, prod.Images.First().Src);

				}
			}
		}


		static public async Task PortCustomersAsync()
		{
			List<ShopifySharp.Customer> customers = Shopify.GetCustomers();

			foreach (ShopifySharp.Customer customer in customers)
			{
				CustomersApi api = new CustomersApi();
				if (customer.DefaultAddress != null)
				{
					await api.CreateCustomerAsync(new CreateCustomerRequest(
					  GivenName: customer.FirstName,
					  FamilyName: customer.LastName,
					  EmailAddress: customer.Email,
					  Address: new Address(
						AddressLine1: customer.DefaultAddress.Address1,
						AddressLine2: customer.DefaultAddress.Address2,
						Locality: customer.DefaultAddress.City,
						AdministrativeDistrictLevel1: customer.DefaultAddress.ProvinceCode,
						PostalCode: customer.DefaultAddress.Zip,
						Country: "US"),
					  PhoneNumber: customer.Phone,
					  ReferenceId: customer.Id.Value.ToString()
					));
				}
				else
				{
					await api.CreateCustomerAsync(new CreateCustomerRequest(
					  GivenName: customer.FirstName,
					  FamilyName: customer.LastName,
					  EmailAddress: customer.Email,
					  PhoneNumber: customer.Phone,
					  ReferenceId: customer.Id.Value.ToString()));
				}
			}
		}
	}


	static class ImageUploader
	{
		const string DOMAIN = "https://connect.squareup.com";

		private struct ItemImage
		{
			public string id { get; set; }
			public string url { get; set; }
		}



		// Uploads the image for a given item.
		// Returns a URL.
		static async public Task<string> PortImage(string locationId,
			string itemID, string imageUrl)
		{
			byte[] data;
			using (WebClient web = new WebClient())
			{
				data = await web.DownloadDataTaskAsync(imageUrl);
			}

			using (HttpClient client = new HttpClient())
			{
				// Configure the "Authorization: Bearer ..." HTTP header
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configuration.Default.AccessToken);

				// Read the image data and add it as an HTTP multipart form data item
				ByteArrayContent imageContent = new ByteArrayContent(data);
				Uri uri = new Uri(imageUrl);

				string ext = Path.GetExtension(uri.AbsolutePath);
				if (ext == ".jpeg" || ext == ".jpg")
				{
					imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
				}
				if (ext == ".png")
				{
					imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
				}

				MultipartFormDataContent requestContent = new MultipartFormDataContent
				{
					{ imageContent, "image_data", Path.GetFileName(imageUrl) }
				};

				// POST the image to the server
				string url = $"{DOMAIN}/v1/{locationId }/items/{itemID}/image";
				HttpResponseMessage response = await client.PostAsync(url, requestContent);

				response.EnsureSuccessStatusCode();

				// (Optional) Parse the URL out of the response using DataContractSerializer.
				// There are nicer ways to parse JSON in C#, but this way uses only built-in
				// libraries.
				string respBody = await response.Content.ReadAsStringAsync();
				ItemImage itemImage = JsonConvert.DeserializeObject<ItemImage>(respBody);

				return itemImage.url;
			}
		}
	}
}
