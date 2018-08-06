namespace Optimal
{
	partial class ProductForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.textBoxTitle = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxVendor = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxShort = new System.Windows.Forms.TextBox();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.label5 = new System.Windows.Forms.Label();
			this.bindingSource2 = new System.Windows.Forms.BindingSource(this.components);
			this.sKUDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.option1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.option2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.option3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.barcodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.priceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.costDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.inventoryQuantityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.BackColor = System.Drawing.Color.Red;
			this.buttonCancel.Location = new System.Drawing.Point(713, 569);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 0;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.buttonOk.Location = new System.Drawing.Point(632, 569);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 1;
			this.buttonOk.Text = "&Ok";
			this.buttonOk.UseVisualStyleBackColor = false;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// textBoxTitle
			// 
			this.textBoxTitle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource2, "Name", true));
			this.textBoxTitle.Location = new System.Drawing.Point(12, 25);
			this.textBoxTitle.Name = "textBoxTitle";
			this.textBoxTitle.Size = new System.Drawing.Size(206, 20);
			this.textBoxTitle.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Title";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Vendor";
			// 
			// textBoxVendor
			// 
			this.textBoxVendor.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource2, "Vendor", true));
			this.textBoxVendor.Location = new System.Drawing.Point(12, 76);
			this.textBoxVendor.Name = "textBoxVendor";
			this.textBoxVendor.Size = new System.Drawing.Size(100, 20);
			this.textBoxVendor.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(15, 158);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Description";
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.AcceptsReturn = true;
			this.textBoxDescription.AcceptsTab = true;
			this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxDescription.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource2, "ShortDesciption", true));
			this.textBoxDescription.Location = new System.Drawing.Point(12, 175);
			this.textBoxDescription.Multiline = true;
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxDescription.Size = new System.Drawing.Size(776, 155);
			this.textBoxDescription.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(15, 109);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Short Description";
			// 
			// textBoxShort
			// 
			this.textBoxShort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxShort.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource2, "ShortDesciption", true));
			this.textBoxShort.Location = new System.Drawing.Point(12, 125);
			this.textBoxShort.Name = "textBoxShort";
			this.textBoxShort.Size = new System.Drawing.Size(776, 20);
			this.textBoxShort.TabIndex = 9;
			// 
			// dataGridView1
			// 
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.AutoGenerateColumns = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sKUDataGridViewTextBoxColumn,
            this.option1DataGridViewTextBoxColumn,
            this.option2DataGridViewTextBoxColumn,
            this.option3DataGridViewTextBoxColumn,
            this.barcodeDataGridViewTextBoxColumn,
            this.priceDataGridViewTextBoxColumn,
            this.costDataGridViewTextBoxColumn,
            this.inventoryQuantityDataGridViewTextBoxColumn});
			this.dataGridView1.DataMember = "Variants";
			this.dataGridView1.DataSource = this.bindingSource2;
			this.dataGridView1.Location = new System.Drawing.Point(18, 362);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(776, 150);
			this.dataGridView1.TabIndex = 10;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 346);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(45, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "Variants";
			// 
			// bindingSource2
			// 
			this.bindingSource2.DataMember = "Product";
			this.bindingSource2.DataSource = typeof(Optimal.ProductForm);
			this.bindingSource2.CurrentChanged += new System.EventHandler(this.bindingSource2_CurrentChanged);
			// 
			// sKUDataGridViewTextBoxColumn
			// 
			this.sKUDataGridViewTextBoxColumn.DataPropertyName = "SKU";
			this.sKUDataGridViewTextBoxColumn.HeaderText = "SKU";
			this.sKUDataGridViewTextBoxColumn.Name = "sKUDataGridViewTextBoxColumn";
			// 
			// option1DataGridViewTextBoxColumn
			// 
			this.option1DataGridViewTextBoxColumn.DataPropertyName = "Option1";
			this.option1DataGridViewTextBoxColumn.HeaderText = "Option1";
			this.option1DataGridViewTextBoxColumn.Name = "option1DataGridViewTextBoxColumn";
			// 
			// option2DataGridViewTextBoxColumn
			// 
			this.option2DataGridViewTextBoxColumn.DataPropertyName = "Option2";
			this.option2DataGridViewTextBoxColumn.HeaderText = "Option2";
			this.option2DataGridViewTextBoxColumn.Name = "option2DataGridViewTextBoxColumn";
			// 
			// option3DataGridViewTextBoxColumn
			// 
			this.option3DataGridViewTextBoxColumn.DataPropertyName = "Option3";
			this.option3DataGridViewTextBoxColumn.HeaderText = "Option3";
			this.option3DataGridViewTextBoxColumn.Name = "option3DataGridViewTextBoxColumn";
			// 
			// barcodeDataGridViewTextBoxColumn
			// 
			this.barcodeDataGridViewTextBoxColumn.DataPropertyName = "Barcode";
			this.barcodeDataGridViewTextBoxColumn.HeaderText = "Barcode";
			this.barcodeDataGridViewTextBoxColumn.Name = "barcodeDataGridViewTextBoxColumn";
			// 
			// priceDataGridViewTextBoxColumn
			// 
			this.priceDataGridViewTextBoxColumn.DataPropertyName = "Price";
			this.priceDataGridViewTextBoxColumn.HeaderText = "Price";
			this.priceDataGridViewTextBoxColumn.Name = "priceDataGridViewTextBoxColumn";
			// 
			// costDataGridViewTextBoxColumn
			// 
			this.costDataGridViewTextBoxColumn.DataPropertyName = "Cost";
			this.costDataGridViewTextBoxColumn.HeaderText = "Cost";
			this.costDataGridViewTextBoxColumn.Name = "costDataGridViewTextBoxColumn";
			// 
			// inventoryQuantityDataGridViewTextBoxColumn
			// 
			this.inventoryQuantityDataGridViewTextBoxColumn.DataPropertyName = "InventoryQuantity";
			this.inventoryQuantityDataGridViewTextBoxColumn.HeaderText = "InventoryQuantity";
			this.inventoryQuantityDataGridViewTextBoxColumn.Name = "inventoryQuantityDataGridViewTextBoxColumn";
			// 
			// ProductForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 607);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.textBoxShort);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBoxDescription);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxVendor);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxTitle);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Name = "ProductForm";
			this.Text = "Product";
			this.Load += new System.EventHandler(this.ProductForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.TextBox textBoxTitle;
		private System.Windows.Forms.BindingSource bindingSource2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxVendor;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxShort;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.DataGridViewTextBoxColumn sKUDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn option1DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn option2DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn option3DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn barcodeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn priceDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn costDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn inventoryQuantityDataGridViewTextBoxColumn;
	}
}