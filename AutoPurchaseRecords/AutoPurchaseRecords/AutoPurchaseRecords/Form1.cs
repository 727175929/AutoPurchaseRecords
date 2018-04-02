using AutoPurchaseRecords.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AutoPurchaseRecords
{
    public partial class Form1 : Form
    {
        public static SQLHelper sqlHelper = new SQLHelper();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridViewColumn myCol = new DataGridViewCheckBoxColumn();   //添加复选框
            this.dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns.Add(myCol);
            GetGridView();
            dataGridView1.Columns[12].Visible = false;    // 永久隐藏

            dataGridView1.Columns[13].Visible = false;    // 隐藏
            dataGridView1.Columns[14].Visible = false;    // 隐藏
            dataGridView1.Columns[5].Visible = false;    // 隐藏
            dataGridView1.Columns[6].Visible = false;    // 隐藏
            dataGridView1.Columns[7].Visible = false;    // 隐藏
            dataGridView1.Columns[8].Visible = false;    // 隐藏
            dataGridView1.Columns[9].Visible = false;    // 隐藏
            dataGridView1.Columns[10].Visible = false;    // 隐藏
            dataGridView1.Columns[11].Visible = false;    // 隐藏
           /* try
            {
                AES aes = new AES();
                XmlDocument xmlInfo = new XmlDocument();
                xmlInfo.Load(System.Environment.CurrentDirectory + "\regedit.xml");
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }*/
            initCombox();
        }

        private void initCombox()
        {
            try
            {
                string sql = "select Distinct Dept from Employ ";
                SQLHelper db = new SQLHelper();
                DataTable dt = db.ExecuteDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strName = dt.Rows[i][0].ToString();
                    comboBox1.Items.Add(strName);
                    comboBox2.Items.Add(strName);
                    comboBox6.Items.Add(strName);
                    comboBox8.Items.Add(strName);
                    comboBox10.Items.Add(strName);
                    comboBox12.Items.Add(strName);
                    comboBox14.Items.Add(strName);
                    comboBox16.Items.Add(strName);
                }
            }
            catch (Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }


        private void GetGridView()
        {
            string sql = @" select BDetail.ID,PName,BDate,EName,Name,Type,BDetail.Unit,BDetail.Price,BDetail.Num,BDetail.Sub,CurrentNum,BID,Invoice,PJID from BDetail,Buy where BDetail.ID not in
 (select BDetail.ID from BuyIn,BuyInGetDetail,BDetail where BuyIn.ID = BuyInGetDetail.BIID and BuyInGetDetail.BID = BDetail.BID) 
 and BDetail.BID = Buy.ID and BDate >= '" + dateTimePicker1.Value.ToShortDateString() + " 00:00" + "' and BDate <= '" + dateTimePicker2.Value.ToShortDateString() + " 23:59" + "'";
            SQLHelper db = new SQLHelper();
            DataTable dt = db.ExecuteDataTable(sql);
            dataGridView1.DataSource = dt;

            dataGridView1.Columns[0].HeaderCell.Value = "全选";
            dataGridView1.Columns[1].HeaderCell.Value = "编号";  //BDetail.ID
            
            dataGridView1.Columns[2].HeaderCell.Value = "供货商";  //PName
            dataGridView1.Columns[3].HeaderCell.Value = "购买日期"; //BDate
            dataGridView1.Columns[4].HeaderCell.Value = "审批人";   //EName

            dataGridView1.Columns[5].HeaderCell.Value = "货物名称";
            dataGridView1.Columns[6].HeaderCell.Value = "类型";
            dataGridView1.Columns[7].HeaderCell.Value = "单位";
            dataGridView1.Columns[8].HeaderCell.Value = "单价";
            dataGridView1.Columns[9].HeaderCell.Value = "数量";
            dataGridView1.Columns[10].HeaderCell.Value = "总金额";
            dataGridView1.Columns[11].HeaderCell.Value = "现库存";


            dataGridView1.Columns[13].HeaderCell.Value = "订单编号";    
            dataGridView1.Columns[14].HeaderCell.Value = "产品编号";    

     
            //dataGridView1.Columns[1].Visible = false;    //隐藏第一列
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("请先选择需要生成的记录");
            }
            else if (comboBox3.Text.ToString().Length <= 0 || comboBox4.Text.ToString().Length <= 0 || comboBox5.Text.ToString().Length <= 0 || comboBox7.Text.ToString().Length <= 0 || comboBox9.Text.ToString().Length <= 0 || comboBox11.Text.ToString().Length <= 0 || comboBox13.Text.ToString().Length <= 0 || comboBox15.Text.ToString().Length <= 0)
            {
                MessageBox.Show("请先设置各类审批人和申请人");
            }
            else
            {
                try
                {
                    String BID,Name,Invoice;  //invoic 如果不为空  需要自动生成一个
                    String pNmae; //提供者公司 对应BuyId : provider    
                    String Ename; //审批者   如果为空选择框中读取  对应BuyId 的person
                    String Sub;//总金额  对应BuyId SumMoney  
                    String BDate;//购买日期   对应BuyId  OrderDate
                    DateTime Temp_BDate;  //转化的过度
                    String PJID, BAPID, GDID;
                    String sql1_1, sql1_2,sql1_3,sql5_1,sql4_1;
                    String sql1, sql2, sql3, sql4, sql5,sql6;
                    String OrderPerson, Person;
                    String BIID,ID;
                    String type,unit;
                    String Num,CurrentNum;  //买入数量和当前库存
                    String Price;
                    String PlanID;
                    DateTime PlanDate, GetDate, OrderDate, PlanGetDate,CheckDate;    //入库日期是BDate  预计到货 PlanDate = BDate-3   GetDate = BDate-1   OrderDate =  BDate -7
                    DateTime EndDate;                     //截止日期 
                    DataGridViewCheckBoxCell checkCell;                //PlanGetDate   计划到货时间
                    Boolean flag;
                    Random r = new Random();
                    for (int row = 0; row <= this.dataGridView1.Rows.Count-1; row++)
                    {
                        checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[row].Cells[0];
                        flag = Convert.ToBoolean(checkCell.Value);
                        if (flag == false)
                        {
                            continue;
                        }
                        ID = this.dataGridView1["ID", row].Value.ToString();
                        BID = this.dataGridView1["BID", row].Value.ToString();
                        Name = this.dataGridView1["Name", row].Value.ToString();
                        Invoice = this.dataGridView1["Invoice", row].Value.ToString();
                        pNmae = this.dataGridView1["PName", row].Value.ToString();
                        Ename = this.dataGridView1["Ename", row].Value.ToString();
                        Sub = this.dataGridView1["Sub", row].Value.ToString();
                        BDate = this.dataGridView1["BDate", row].Value.ToString();
                        Temp_BDate = Convert.ToDateTime(BDate);   //强制转化
                        PJID = this.dataGridView1["PJID", row].Value.ToString();
                        type = this.dataGridView1["Type", row].Value.ToString();
                        unit = this.dataGridView1["Unit", row].Value.ToString();
                        Num = this.dataGridView1["Num",row].Value.ToString();
                        Price =this.dataGridView1["Price",row].Value.ToString();
                        CurrentNum = this.dataGridView1["CurrentNum", row].Value.ToString();
                        OrderPerson = comboBox3.Text.ToString();   //申请人
                        if (Ename.Length <= 0)
                        {
                            Person = comboBox4.Text.ToString();
                        }
                        else
                        {
                            Person = Ename;
                        }
                        if(Invoice.Length<=0)
                        {
                            TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
                            long t = (long)cha.TotalSeconds;     //用时间戳生成一个不会重复的表示符 
                            Invoice = t.ToString();
                        }
                        if (Sub.Length <= 0)
                        {
                            Sub = "0";
                        }
                        if (CurrentNum.Length <= 0)
                        {
                            CurrentNum = "0";
                        }
                        PlanDate = Temp_BDate.AddDays(-3);
                        OrderDate = Temp_BDate.AddDays(-7);
                        GetDate = Temp_BDate.AddDays(-1);
                        CheckDate = PlanDate.AddDays(r.Next(0, 1));   //审核日期   0-1
                        PlanGetDate = PlanDate.AddDays(r.Next(2, 3));  //PlanGetDate     计划到货时间  PlanDate 0-3
                        //入库日期是BDate  预计到货 PlanDate = BDate-3   GetDate = BDate-1   OrderDate =  BDate -7
                        EndDate = OrderDate.AddDays(5);     //截止日期是申请日期+1
                        sql1_1 = @"INSERT INTO BuyPlan (PlanDate,Maker,Checker,CheckDate,State,IsInShopping,PlanType) 
                                 VALUES('" + PlanDate + "','" + comboBox5.Text.ToString() + "','" + comboBox7.Text.ToString() + "','" + CheckDate + "','1','3','1')";
                        SQLHelper db1_1 = new SQLHelper();
                        DataTable dt1_1 = db1_1.ExecuteDataTable(sql1_1);

                        sql1_2 = "select top(1) ID from BuyPlan  order by id desc";
                        SQLHelper db1_2 = new SQLHelper();
                        DataTable dt1_2 = db1_2.ExecuteDataTable(sql1_2);
                        PlanID = dt1_2.Rows[0][0].ToString();   //获取planID

                        sql1_3 = @"INSERT INTO BuyPlanDetail (BPID,Name,Type,Unit,MonthNeedAmount,PJDate,PJAmount,LowAmount,TopAmount,NeedAmount,PlanBuyAmount,PlanGetDate,BuyType,Remark,BuyAmount,toothcolor,pjid)
                                 VALUES('" + PlanID + "','" + Name + "','" + type + "','" + unit + "','0','" + PlanDate + "','" + CurrentNum + "','1','0','" + Num + "','" + Num + "','" + PlanGetDate + "','0','库存报警','" + Num + "','','" + PJID + "')";
                        SQLHelper db1_3 = new SQLHelper();
                        DataTable dt1_3 = db1_3.ExecuteDataTable(sql1_3);

                        sql1 = @"INSERT INTO BuyIn (Invoice,OrderPerson,Provider,Person,SumMoney,OrderDate,PlanID,Phone,Fax,Address,PayKind) VALUES 
                            ('" + Invoice + "','" + comboBox3.Text.ToString() + "','" + pNmae + "','" + comboBox4.Text.ToString() + "','" + Sub + "','" + OrderDate + "','" + PlanID + "','','','','')";
                        SQLHelper db = new SQLHelper();
                        DataTable dt = db.ExecuteDataTable(sql1);

                        sql2 = "select top(1) ID from BuyIn  order by id desc";
                        SQLHelper db2 = new SQLHelper();
                        DataTable dt2 = db2.ExecuteDataTable(sql2);
                        BIID = dt2.Rows[0][0].ToString();   //获取BII 
                        
                        sql3 = @"INSERT INTO BuyInDetail (BIID,BIName,BItype,BIUnit,BIAmount,Price,PlanDate,GetAmount,pjid,BIArea,Remark,toothcolor) 
                        VALUES ('" + BIID + "','" + Name + "','" + type + "','" + unit + "','" + Num + "','" + Price + "','" + PlanDate + "','" + Num + "','" + PJID + "','','','')";
                        SQLHelper db3 = new SQLHelper();
                        DataTable dt3 = db3.ExecuteDataTable(sql3);

                        sql4 = @"INSERT INTO BuyInGetDetail (BIID,Name,type,unit,Price,GetDate,GetAmount,BID,pjid,Area,Remark,toothcolor,State) VALUES 
                            ('" + BIID + "','" + Name + "','" + type + "','" + unit + "','" + Price + "','" + GetDate + "','" + Num + "','" + BID + "','" + PJID + "','','','','1')";
                        SQLHelper db4 = new SQLHelper();
                        DataTable dt4 = db4.ExecuteDataTable(sql4);

                        sql4_1 = "select top(1) ID from BuyInGetDetail  order by id desc";
                        SQLHelper db4_1 = new SQLHelper();
                        DataTable dt4_1 = db4_1.ExecuteDataTable(sql4_1);
                        GDID = dt4_1.Rows[0][0].ToString();   //获取GDID

                        sql5 = @"INSERT INTO BuyApplyPay(ApplyPerson,ApplyDate,Provider,Person,Bank,BankAccount,SumMoney,endPayDate,GetDate,verify1State,verify1,verify1Date,verify2State,verify2,verify2Date,IsPayed,PayMoney,Remark,PayDate,PayPerson)
                            VALUES('" + comboBox11.Text.ToString() + "','" + OrderDate + "','" + pNmae + "','','','','" + Sub + "','" + EndDate + "','','1','" + comboBox13.Text.ToString() + "','" + CheckDate + "','1','" + comboBox13.Text.ToString() + "','" + CheckDate + "','1','" + Sub + "','','" + CheckDate + "','" + comboBox15.Text.ToString() + "')";
                        SQLHelper db5 = new SQLHelper();
                        DataTable dt5 = db5.ExecuteDataTable(sql5);

                        sql5_1 = "select top(1) ID from BuyApplyPay  order by id desc";
                        SQLHelper db5_1 = new SQLHelper();
                        DataTable dt5_1 = db5_1.ExecuteDataTable(sql5_1);
                        BAPID = dt5_1.Rows[0][0].ToString();   //获取BAPID

                        sql6 = @"INSERT INTO BuyApplyPayDetail(BAPID,GDID,BIID,Name,type,Unit,Price,Area,GetDate,GetAmount,Remark)
                            VALUES('" + BAPID + "','" + GDID + "','" + BIID + "','" + Name + "','" + type + "','" + unit + "','" + Price + "','','" + GetDate + "','" + Num + "','')";
                        SQLHelper db6 = new SQLHelper();
                        DataTable dt6 = db6.ExecuteDataTable(sql6);

                        string newLine = "入库ID：" + ID + "已生成对应的采购记录和采购计划\n";
                        // 将textBox1的内容插入到第一行
                        // 索引0是 richText1 第一行位置
                        richTextBox1.Text = richTextBox1.Text.Insert(0, newLine);
                        richTextBox1.ScrollToCaret();
                    }
                    MessageBox.Show("生成完成");
                    GetGridView();
                }
                catch (Exception e3)
                {
                    WriteLog.Write_Log(e3);
                    MessageBox.Show(e3.Message);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Text = "";
            comboBox3.Items.Clear();
            try
            {
                string sql = @"select OtherName from Employ where Dept = '"+comboBox1.Text.ToString()+"' order by OtherName collate Chinese_PRC_CS_AS_KS_WS";
                SQLHelper db = new SQLHelper();
                DataTable dt = db.ExecuteDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strName = dt.Rows[i][0].ToString();
                    comboBox3.Items.Add(strName);
                }
            }
            catch (Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Text = "";
            comboBox4.Items.Clear();
            try
            {
                string sql = @"select OtherName from Employ where Dept = '" + comboBox2.Text.ToString() + "' order by OtherName collate Chinese_PRC_CS_AS_KS_WS";
                SQLHelper db = new SQLHelper();
                DataTable dt = db.ExecuteDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strName = dt.Rows[i][0].ToString();
                    comboBox4.Items.Add(strName);
                }
            }
            catch (Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            int count = dataGridView1.Rows.Count;
            if (checkBox1.Text.Equals("全选"))
            {
                checkBox1.Text = "全不选";
                for (int i = 0; i < count; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                    Boolean flag = Convert.ToBoolean(checkCell.Value);
                    if (flag == false)
                    {
                        checkCell.Value = true;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                checkBox1.Text = "全选";
                for (int i = 0; i < count; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                    Boolean flag = Convert.ToBoolean(checkCell.Value);
                    if (flag == true)
                    {
                        checkCell.Value = false;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                GetGridView();
            }catch(Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("查看明细"))
            {
                dataGridView1.Columns[5].Visible = true;
                dataGridView1.Columns[6].Visible = true;
                dataGridView1.Columns[7].Visible = true;
                dataGridView1.Columns[8].Visible = true;
                dataGridView1.Columns[9].Visible = true;
                dataGridView1.Columns[10].Visible = true;
                dataGridView1.Columns[11].Visible = true;
                dataGridView1.Columns[13].Visible = true;
                dataGridView1.Columns[14].Visible = true;
                button1.Text = "隐藏明细";
            }
            else
            {
                dataGridView1.Columns[5].Visible = false;
                dataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[7].Visible = false;
                dataGridView1.Columns[8].Visible = false;
                dataGridView1.Columns[9].Visible = false;
                dataGridView1.Columns[10].Visible = false;
                dataGridView1.Columns[11].Visible = false;
                dataGridView1.Columns[13].Visible = false;
                dataGridView1.Columns[14].Visible = false;
                button1.Text = "查看明细";
            }
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox13.Text = "";
            comboBox13.Items.Clear();
            try
            {
                string sql = @"select OtherName from Employ where Dept = '" + comboBox14.Text.ToString() + "' order by OtherName collate Chinese_PRC_CS_AS_KS_WS";
                SQLHelper db = new SQLHelper();
                DataTable dt = db.ExecuteDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strName = dt.Rows[i][0].ToString();
                    comboBox13.Items.Add(strName);
                }
            }
            catch (Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox7.Text = "";
            comboBox7.Items.Clear();
            try
            {
                string sql = @"select OtherName from Employ where Dept = '" + comboBox8.Text.ToString() + "' order by OtherName collate Chinese_PRC_CS_AS_KS_WS";
                SQLHelper db = new SQLHelper();
                DataTable dt = db.ExecuteDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strName = dt.Rows[i][0].ToString();
                    comboBox7.Items.Add(strName);
                }
            }
            catch (Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox5.Text = "";
            comboBox5.Items.Clear();
            try
            {
                string sql = @"select OtherName from Employ where Dept = '" + comboBox6.Text.ToString() + "' order by OtherName collate Chinese_PRC_CS_AS_KS_WS";
                SQLHelper db = new SQLHelper();
                DataTable dt = db.ExecuteDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strName = dt.Rows[i][0].ToString();
                    comboBox5.Items.Add(strName);
                }
            }
            catch (Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox9.Text = "";
            comboBox9.Items.Clear();
            try
            {
                string sql = @"select OtherName from Employ where Dept = '" + comboBox10.Text.ToString() + "' order by OtherName collate Chinese_PRC_CS_AS_KS_WS";
                SQLHelper db = new SQLHelper();
                DataTable dt = db.ExecuteDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strName = dt.Rows[i][0].ToString();
                    comboBox9.Items.Add(strName);
                }
            }
            catch (Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox11.Text = "";
            comboBox11.Items.Clear();
            try
            {
                string sql = @"select OtherName from Employ where Dept = '" + comboBox12.Text.ToString() + "' order by OtherName collate Chinese_PRC_CS_AS_KS_WS";
                SQLHelper db = new SQLHelper();
                DataTable dt = db.ExecuteDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strName = dt.Rows[i][0].ToString();
                    comboBox11.Items.Add(strName);
                }
            }
            catch (Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }

        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox15.Text = "";
            comboBox15.Items.Clear();
            try
            {
                string sql = @"select OtherName from Employ where Dept = '" + comboBox16.Text.ToString() + "' order by OtherName collate Chinese_PRC_CS_AS_KS_WS";
                SQLHelper db = new SQLHelper();
                DataTable dt = db.ExecuteDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strName = dt.Rows[i][0].ToString();
                    comboBox15.Items.Add(strName);
                }
            }
            catch (Exception e2)
            {
                WriteLog.Write_Log(e2);
                MessageBox.Show(e2.Message);
            }
        }

    }
}
