using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System;

namespace WTools.warehouse
{
    public class WarehouseTools
    {
        public string Od_No { get; set; }
        public string MB001 { get; set; }
        public string Userid { get; set; }
        public string UserType { get; set; }
        public int Upid { get; set; }
        public int Itemid { get; set; }
        public int Quty { get; set; }//,[CheckQuty],[CheckBack],[Pt_Name],[Pt_type],[Pt_Unit]
        public string Pt_Name { get; set; }
        public string Pt_type { get; set; }
        public string Pt_Unit { get; set; }
        public int CheckQuty { get; set; }
        public int CheckBack { get; set; }
        public WarehouseTools() { }
        public bool SetWarehouse()
        {
            bool isok = true;
            //UserType=0.進貨 1.進貨退回 2.銷貨 3.銷退 4.領料 5.領料退回
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("", conn1);
            cmd1.Connection.Open();
            SqlTransaction sqlTransaction = null;
            sqlTransaction = conn1.BeginTransaction();
            cmd1.Transaction = sqlTransaction;
            int InOut = 1;
            string subline = "+";
            if ("124".Contains(UserType))
            {
                InOut = -1;
                subline = "-";
            }
            //進貨主檔
            string sql= $"if (select COUNT(*) FROM [OrderProductM] WHERE [Od_No]='{Od_No}')=0 INSERT INTO OrderProductM([SupId],[Od_No],[TotalPrice],[Cdate],[UsId],[UserType]) VALUES";
            sql += $"('','{Od_No}',0,GETDATE(),'{Userid}','{UserType}')";
            cmd1.CommandText = sql;
            cmd1.ExecuteNonQuery();
            //進貨明細檔
            sql = "INSERT INTO [OrderProuductT]i([Od_No],[PtNo],[Quty],[CheckQuty],[CheckBack],[Pt_Name],[Pt_type],[Pt_Unt]) VALUES";
            sql += $"('{Od_No}','{MB001}',{Quty},{CheckQuty},{CheckBack},'{Pt_Name}','{Pt_type}','{Pt_Unit}')";
            cmd1.CommandText = sql;
            cmd1.ExecuteNonQuery();
            //新增交易
            sql = "INSERT INTO [PDList]([ScID],[userid],[MB001],[Quty],[InOut],[UserType]) VALUES";
            sql += $"('{Od_No}','{Userid}','{MB001}',{Quty},{InOut},'{UserType}')";
            cmd1.CommandText = sql;
            cmd1.ExecuteNonQuery();
            //變更商品數量
            sql = $"UPDATE [Products] SET [MB064] = [MB064]{subline}{Quty},UDate=getdate() WHERE [MB001]='{MB001}'";
            cmd1.CommandText = sql;
            cmd1.ExecuteNonQuery();

            try
            {
                sqlTransaction.Commit();
       
            }
            catch
            {
                sqlTransaction.Rollback();
                isok = false;
                MessageBox.Show("存檔失敗!!!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return isok;
        }

    }
}
