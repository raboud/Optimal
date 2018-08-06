using ONWLibrary;
using Optimal.Models;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Optimal
{
	public partial class ProductForm : Form
	{
//		public Product Product { get; set; }
		public FlatProduct FlatProduct { get; set; }
		public ProductDTO Product { get; set; }

		public ProductForm(FlatProduct flat)
		{
			this.Product = new ProductDTO();
			this.FlatProduct = flat;
			InitializeComponent();
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private async void ProductForm_Load(object sender, EventArgs e)
		{
			this.Product.SetProduct(await Shopify.GetProductAsync(this.FlatProduct.GetProductVariant().ProductId.Value));
			this.bindingSource2.DataSource = this.Product;
//			this.bindingSource2.ResetBindings(false);
			//this.Product = await Shopify.GetProductAsync(this.FlatProduct.GetProductVariant().ProductId.Value);
			//this.bindingSource2.DataSource = this.Product;
			//this.textBoxTitle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource2, "Title", true));
			//this.textBoxVendor.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource2, "Vendor", true));
			//this.textBoxDescription.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource2, "BodyHtml", true));
			//this.textBoxShort.Text = this.Product.Metafields.FirstOrDefault(m => m.Key == "ShortDescription")?.Value.ToString();
			//this.dataGridView1.DataSource = this.bindingSource2;
			//this.dataGridView1.DataMember = "Variants";

			//this.dataGridView1.Columns.Remove("ImageId");
			//this.dataGridView1.Columns.Remove("InventoryQuantityAdjustment");
			//this.dataGridView1.Columns.Remove("OldInventoryQuantity");
			//this.dataGridView1.Columns.Remove("InventoryQuantity");
			//this.dataGridView1.Columns.Remove("Barcode");
			//this.dataGridView1.Columns.Remove("RequiresShipping");
			//this.dataGridView1.Columns.Remove("Taxable");
			//this.dataGridView1.Columns.Remove("UpdatedAt");
			//this.dataGridView1.Columns.Remove("CreatedAt");
			//this.dataGridView1.Columns.Remove("WeightUnit");
			//this.dataGridView1.Columns.Remove("CompareAtPrice");
			//this.dataGridView1.Columns.Remove("Grams");
			//this.dataGridView1.Columns.Remove("Position");
			//this.dataGridView1.Columns.Remove("ProductId");
			//this.dataGridView1.Columns.Remove("Metafields");
		}

		private void bindingSource2_CurrentChanged(object sender, EventArgs e)
		{

		}
	}
}
