using System;
using System.Data;
using System.Data.SqlClient;
using UserManagementApp.General;
using System.Windows.Forms;

namespace UserManagementApp.Forms
{
    public partial class ViewRolesForm : TemplateForm
    {
        public ViewRolesForm()
        {
            InitializeComponent();
            LoadDataIntoDataGridView();
        }

        private void LoadDataIntoDataGridView()
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usp_Roles_LoadDataIntoDataGridView", con)) //specify stored procedure
                {
                    cmd.CommandType = CommandType.StoredProcedure; //declare a command for Stored Procedure

                    if (con.State != ConnectionState.Open) //check if connection with database is established
                        con.Open();
                    DataTable dtRoles = new DataTable(); //create a new DataTable that stores the data

                    SqlDataReader sdr = cmd.ExecuteReader(); //read data from database

                    dtRoles.Load(sdr); //load data into dtRoles datatable

                    RolesDataGridView.DataSource = dtRoles; //embed data in dtRoles in RolesDataGridView forms object for display
                    //RolesDataGridView.Columns[0].Visible = false; //will be used for future lessons
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if(SearchTextBox.Text != string.Empty) //is the textbox is not empty
            {
                using(SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_Roles_SearchByTitle", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        //Passing Parameter
                        cmd.Parameters.AddWithValue("@RoleTitle", SearchTextBox.Text.Trim());

                        if (con.State != ConnectionState.Open)
                            con.Open();

                        DataTable dtRole = new DataTable();

                        SqlDataReader sdr = cmd.ExecuteReader();
                        
                        if(sdr.HasRows)
                        {
                            dtRole.Load(sdr);
                            RolesDataGridView.DataSource = dtRole;
                        }
                        else
                        {
                            MessageBox.Show("No Matching Role is found.", "No record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void refreshRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
            SearchTextBox.Clear();
            SearchTextBox.Focus();
        }

        private void newRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RolesForm rf = new RolesForm(); //create a new reference to the RolesForm.cs and opens the form related to that script
            rf.ShowDialog();
            LoadDataIntoDataGridView();
        }

        private void RolesDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(RolesDataGridView.Rows.Count > 0) //if the data grid has more than 0 rows
            {
                int roleId = Convert.ToInt32(RolesDataGridView.SelectedRows[0].Cells[0].Value);
                RolesForm RolesForm = new RolesForm();
                RolesForm.RoleId = roleId;
                RolesForm.IsUpdate = true;
                RolesForm.ShowDialog();

                LoadDataIntoDataGridView();
            }
        }
    }
}
