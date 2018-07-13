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
	public partial class ProductsForm : Form
	{
		private List<Product> Products;
		private List<FlatProduct> Flats;
		private SortableBindingList<FlatProduct> BindingListProducts;
		private BindingSource BindingSource;

		private int newSortColumn = -1;
		ListSortDirection newColumnDirection = ListSortDirection.Ascending;

		public ProductsForm()
		{
			Shopify.Init();
			//this.Products = Shopify.GetProducts().ToList();
			//this.BindingListProducts = new BindingList<Product>(this.Products);
			//this.BindingSource = new BindingSource(BindingListProducts, null);

			InitializeComponent();
		}

		public void initGrid()
		{
			this.dataGridView1.DataSource = this.BindingSource;
		}

		private async void ProductsForm_Load(object sender, EventArgs e)
		{
			this.Products = (await Shopify.GetProducts()).ToList();
			this.Flats = FlatProduct.FromProducts(this.Products);
			int count = this.Products.Count(p => p.Variants?.Count() == 0);
			this.BindingListProducts = new SortableBindingList<FlatProduct>(this.Flats);
			this.BindingSource = new BindingSource(BindingListProducts, null);
			this.dataGridView1.DataSource = this.BindingSource;

			this.dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
		}

		private async void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			FlatProduct flat = this.dataGridView1.Rows[e.RowIndex].DataBoundItem as FlatProduct;
			ProductVariant pv = flat.GetProductVariant();
			if (pv.Barcode == null) pv.Barcode = "";
			if (pv.SKU == null) pv.SKU = "";
			ProductVariant pv2 = await Shopify.UpdateVariant(pv);
			this.textSearch.Select();
			this.textSearch.SelectAll();
		}

		private void buttonSearch_Click(object sender, EventArgs e)
		{
			this.search();
		}

		private void search()
		{
			int start = (this.dataGridView1.CurrentCell.RowIndex + 1) % this.dataGridView1.RowCount;
			int i = start;
			do
			{
				FlatProduct flat = this.dataGridView1.Rows[i].DataBoundItem as FlatProduct;
				if (flat.Name.IndexOf(this.textSearch.Text, StringComparison.CurrentCultureIgnoreCase) >= 0)
				{
					this.dataGridView1.CurrentCell = this.dataGridView1.Rows[i].Cells["Barcode"];
					this.dataGridView1.Select();
					break;
				}
				else if (flat.Barcode == this.textSearch.Text)
				{
					this.dataGridView1.CurrentCell = this.dataGridView1.Rows[i].Cells["InventoryQuantity"];
					this.dataGridView1.Select();
					break;
				}
				i = (i + 1) % this.dataGridView1.RowCount;
			} while (i != start);
		}

		private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			e.Control.KeyPress -= new KeyPressEventHandler(DecimalColumn_KeyPress);
			e.Control.KeyPress -= new KeyPressEventHandler(NumericColumn_KeyPress);

			if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns["Price"].Index) //Desired Column
			{
				TextBox tb = e.Control as TextBox;
				if (tb != null)
				{
					tb.KeyPress += new KeyPressEventHandler(DecimalColumn_KeyPress);
				}
			}
			if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns["Barcode"].Index) //Desired Column
			{
				TextBox tb = e.Control as TextBox;
				if (tb != null)
				{
					tb.KeyPress += new KeyPressEventHandler(NumericColumn_KeyPress);
				}
			}
			if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns["InventoryQuantity"].Index) //Desired Column
			{
				TextBox tb = e.Control as TextBox;
				if (tb != null)
				{
					tb.KeyPress += new KeyPressEventHandler(NumericColumn_KeyPress);
				}
			}


		}

		private void DecimalColumn_KeyPress(object sender, KeyPressEventArgs e)
		{
			// allowed numeric and one dot  ex. 10.23
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)
				 && e.KeyChar != '.')
			{
				e.Handled = true;
			}

			// only allow one decimal point
			if (e.KeyChar == '.'
				&& (sender as TextBox).Text.IndexOf('.') > -1)
			{
				e.Handled = true;
			}
		}


		private void NumericColumn_KeyPress(object sender, KeyPressEventArgs e)
		{
			// allowed only numeric value  ex.10
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
			{
			    e.Handled = true;
			}
		}

		private void textSearch_KeyPress(object sender, KeyPressEventArgs e)
		{
			System.Diagnostics.Debug.Write(e.KeyChar);
			if (e.KeyChar == '\r')
			{
				this.search();
			}
			if (e.KeyChar == '\n')
			{
				this.search();
			}
		}
	}
}
