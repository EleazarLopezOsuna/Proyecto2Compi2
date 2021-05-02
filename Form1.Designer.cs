
namespace Proyecto2_Compiladores2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.code_textbox = new System.Windows.Forms.TextBox();
            this.error_table = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.table_label = new System.Windows.Forms.Label();
            this.symbol_table = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.console_textbox = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Nombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ambito = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Heap = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sizex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Absoluta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Relativa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fila = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Columna = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.error_table)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbol_table)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(137, 42);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(155, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(137, 42);
            this.button2.TabIndex = 1;
            this.button2.Text = "Run";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(298, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(137, 42);
            this.button3.TabIndex = 2;
            this.button3.Text = "Optimization";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(96, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 18);
            this.label3.TabIndex = 9;
            this.label3.Text = "Current position: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 29);
            this.label1.TabIndex = 8;
            this.label1.Text = "Code";
            // 
            // code_textbox
            // 
            this.code_textbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.code_textbox.Location = new System.Drawing.Point(12, 105);
            this.code_textbox.Multiline = true;
            this.code_textbox.Name = "code_textbox";
            this.code_textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.code_textbox.Size = new System.Drawing.Size(551, 536);
            this.code_textbox.TabIndex = 10;
            // 
            // error_table
            // 
            this.error_table.AllowUserToAddRows = false;
            this.error_table.AllowUserToDeleteRows = false;
            this.error_table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.error_table.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5});
            this.error_table.Location = new System.Drawing.Point(583, 402);
            this.error_table.Name = "error_table";
            this.error_table.ReadOnly = true;
            this.error_table.RowTemplate.Height = 25;
            this.error_table.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.error_table.Size = new System.Drawing.Size(760, 242);
            this.error_table.TabIndex = 17;
            this.error_table.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Tipo";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Descripcion";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn3.Width = 497;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Linea";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn4.Width = 60;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Columna";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn5.Width = 60;
            // 
            // table_label
            // 
            this.table_label.AutoSize = true;
            this.table_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.table_label.Location = new System.Drawing.Point(583, 370);
            this.table_label.Name = "table_label";
            this.table_label.Size = new System.Drawing.Size(163, 29);
            this.table_label.TabIndex = 16;
            this.table_label.Text = "Symbol Table";
            this.table_label.Visible = false;
            // 
            // symbol_table
            // 
            this.symbol_table.AllowUserToAddRows = false;
            this.symbol_table.AllowUserToDeleteRows = false;
            this.symbol_table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.symbol_table.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nombre,
            this.Tipo,
            this.Ambito,
            this.Rol,
            this.Heap,
            this.Sizex,
            this.Absoluta,
            this.Relativa,
            this.Fila,
            this.Columna});
            this.symbol_table.Location = new System.Drawing.Point(583, 402);
            this.symbol_table.Name = "symbol_table";
            this.symbol_table.ReadOnly = true;
            this.symbol_table.RowTemplate.Height = 25;
            this.symbol_table.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.symbol_table.Size = new System.Drawing.Size(760, 242);
            this.symbol_table.TabIndex = 15;
            this.symbol_table.Visible = false;
            this.symbol_table.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.symbol_table_CellContentClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(583, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 29);
            this.label2.TabIndex = 14;
            this.label2.Text = "Console";
            // 
            // console_textbox
            // 
            this.console_textbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.console_textbox.Location = new System.Drawing.Point(583, 105);
            this.console_textbox.Multiline = true;
            this.console_textbox.Name = "console_textbox";
            this.console_textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.console_textbox.Size = new System.Drawing.Size(760, 242);
            this.console_textbox.TabIndex = 13;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Nombre
            // 
            this.Nombre.HeaderText = "Nombre";
            this.Nombre.Name = "Nombre";
            this.Nombre.ReadOnly = true;
            this.Nombre.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Nombre.Width = 143;
            // 
            // Tipo
            // 
            this.Tipo.HeaderText = "Tipo";
            this.Tipo.Name = "Tipo";
            this.Tipo.ReadOnly = true;
            this.Tipo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Ambito
            // 
            this.Ambito.HeaderText = "Ambito";
            this.Ambito.Name = "Ambito";
            this.Ambito.ReadOnly = true;
            this.Ambito.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Ambito.Width = 140;
            // 
            // Rol
            // 
            this.Rol.HeaderText = "Rol";
            this.Rol.Name = "Rol";
            this.Rol.ReadOnly = true;
            this.Rol.Width = 95;
            // 
            // Heap
            // 
            this.Heap.HeaderText = "H";
            this.Heap.Name = "Heap";
            this.Heap.ReadOnly = true;
            this.Heap.Width = 40;
            // 
            // Sizex
            // 
            this.Sizex.HeaderText = "S";
            this.Sizex.Name = "Sizex";
            this.Sizex.ReadOnly = true;
            this.Sizex.Width = 40;
            // 
            // Absoluta
            // 
            this.Absoluta.HeaderText = "A";
            this.Absoluta.Name = "Absoluta";
            this.Absoluta.ReadOnly = true;
            this.Absoluta.Width = 40;
            // 
            // Relativa
            // 
            this.Relativa.HeaderText = "R";
            this.Relativa.Name = "Relativa";
            this.Relativa.ReadOnly = true;
            this.Relativa.Width = 40;
            // 
            // Fila
            // 
            this.Fila.HeaderText = "F";
            this.Fila.Name = "Fila";
            this.Fila.ReadOnly = true;
            this.Fila.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Fila.Width = 40;
            // 
            // Columna
            // 
            this.Columna.HeaderText = "C";
            this.Columna.Name = "Columna";
            this.Columna.ReadOnly = true;
            this.Columna.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Columna.Width = 40;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 666);
            this.Controls.Add(this.error_table);
            this.Controls.Add(this.table_label);
            this.Controls.Add(this.symbol_table);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.console_textbox);
            this.Controls.Add(this.code_textbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Run";
            ((System.ComponentModel.ISupportInitialize)(this.error_table)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbol_table)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox code_textbox;
        private System.Windows.Forms.DataGridView error_table;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.Label table_label;
        private System.Windows.Forms.DataGridView symbol_table;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox console_textbox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ambito;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rol;
        private System.Windows.Forms.DataGridViewTextBoxColumn Heap;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sizex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Absoluta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Relativa;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fila;
        private System.Windows.Forms.DataGridViewTextBoxColumn Columna;
    }
}

