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


		public ProductsForm()
		{
			Shopify.Init();
			InitializeComponent();
		}

		public void InitGrid()
		{
			dataGridView1.DataSource = BindingSource;
		}

		private async void ProductsForm_Load(object sender, EventArgs e)
		{
			Products = (await Shopify.GetProducts()).ToList();
			Flats = FlatProduct.FromProducts(Products).Where(fp => fp.Price == null || fp.Price.Value == 0.0m).ToList();
			int count = Products.Count(p => p.Variants?.Count() == 0);
			BindingListProducts = new SortableBindingList<FlatProduct>(Flats);
			BindingSource = new BindingSource(BindingListProducts, null);
			dataGridView1.DataSource = BindingSource;

			dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
			dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
		}

		private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			FlatProduct p = dataGridView1.Rows[e.RowIndex].DataBoundItem as FlatProduct;

			ProductForm productForm = new ProductForm(p);
			productForm.ShowDialog(this);

		}

		private async void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			FlatProduct flat = dataGridView1.Rows[e.RowIndex].DataBoundItem as FlatProduct;
			ProductVariant pv = flat.GetProductVariant();
			if (pv.Barcode == null)
			{
				pv.Barcode = "";
			}

			if (pv.SKU == null)
			{
				pv.SKU = "";
			}

			ProductVariant pv2 = await Shopify.UpdateVariant(pv);
			textSearch.Select();
			textSearch.SelectAll();
		}

		private void ButtonSearch_Click(object sender, EventArgs e)
		{
			Search();
		}

		private void Search()
		{
			int start = (dataGridView1.CurrentCell.RowIndex + 1) % dataGridView1.RowCount;
			int i = start;
			do
			{
				FlatProduct flat = dataGridView1.Rows[i].DataBoundItem as FlatProduct;
				if (flat.Name.IndexOf(textSearch.Text, StringComparison.CurrentCultureIgnoreCase) >= 0)
				{
					dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells["Barcode"];
					dataGridView1.Select();
					break;
				}
				else if (flat.Barcode == textSearch.Text)
				{
					dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells["InventoryQuantity"];
					dataGridView1.Select();
					break;
				}
				i = (i + 1) % dataGridView1.RowCount;
			} while (i != start);
		}

		private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			e.Control.KeyPress -= new KeyPressEventHandler(DecimalColumn_KeyPress);
			e.Control.KeyPress -= new KeyPressEventHandler(NumericColumn_KeyPress);

			if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns["Price"].Index) //Desired Column
			{
				if (e.Control is TextBox tb)
				{
					tb.KeyPress += new KeyPressEventHandler(DecimalColumn_KeyPress);
				}
			}
			if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns["Barcode"].Index) //Desired Column
			{
				if (e.Control is TextBox tb)
				{
					tb.KeyPress += new KeyPressEventHandler(NumericColumn_KeyPress);
				}
			}
			if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns["InventoryQuantity"].Index) //Desired Column
			{
				if (e.Control is TextBox tb)
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

		private void TextSearch_KeyPress(object sender, KeyPressEventArgs e)
		{
			System.Diagnostics.Debug.Write(e.KeyChar);
			if (e.KeyChar == '\r')
			{
				Search();
			}
			if (e.KeyChar == '\n')
			{
				Search();
			}
		}
	}
}
