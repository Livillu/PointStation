namespace WTools
{
    partial class UserStockLocal
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.增加倉庫ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.增加儲位ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.刪除節點ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看庫存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.treeView1.Location = new System.Drawing.Point(84, 65);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(408, 512);
            this.treeView1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.增加倉庫ToolStripMenuItem,
            this.增加儲位ToolStripMenuItem,
            this.刪除節點ToolStripMenuItem,
            this.查看庫存ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 124);
            // 
            // 增加倉庫ToolStripMenuItem
            // 
            this.增加倉庫ToolStripMenuItem.Name = "增加倉庫ToolStripMenuItem";
            this.增加倉庫ToolStripMenuItem.Size = new System.Drawing.Size(152, 30);
            this.增加倉庫ToolStripMenuItem.Text = "增加倉庫";
            this.增加倉庫ToolStripMenuItem.Click += new System.EventHandler(this.增加倉庫ToolStripMenuItem_Click);
            // 
            // 增加儲位ToolStripMenuItem
            // 
            this.增加儲位ToolStripMenuItem.Name = "增加儲位ToolStripMenuItem";
            this.增加儲位ToolStripMenuItem.Size = new System.Drawing.Size(152, 30);
            this.增加儲位ToolStripMenuItem.Text = "增加儲位";
            this.增加儲位ToolStripMenuItem.Click += new System.EventHandler(this.增加儲位ToolStripMenuItem_Click);
            // 
            // 刪除節點ToolStripMenuItem
            // 
            this.刪除節點ToolStripMenuItem.Name = "刪除節點ToolStripMenuItem";
            this.刪除節點ToolStripMenuItem.Size = new System.Drawing.Size(152, 30);
            this.刪除節點ToolStripMenuItem.Text = "刪除節點";
            this.刪除節點ToolStripMenuItem.Click += new System.EventHandler(this.刪除節點ToolStripMenuItem_Click);
            // 
            // 查看庫存ToolStripMenuItem
            // 
            this.查看庫存ToolStripMenuItem.Name = "查看庫存ToolStripMenuItem";
            this.查看庫存ToolStripMenuItem.Size = new System.Drawing.Size(152, 30);
            this.查看庫存ToolStripMenuItem.Text = "查看庫存";
            this.查看庫存ToolStripMenuItem.Click += new System.EventHandler(this.查看庫存ToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("新細明體", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Location = new System.Drawing.Point(519, 65);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 31;
            this.dataGridView1.Size = new System.Drawing.Size(876, 512);
            this.dataGridView1.TabIndex = 1;
            // 
            // UserStockLocal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.treeView1);
            this.Name = "UserStockLocal";
            this.Size = new System.Drawing.Size(1422, 638);
            this.Load += new System.EventHandler(this.UserStockLocal_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 增加倉庫ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 增加儲位ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 刪除節點ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查看庫存ToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
