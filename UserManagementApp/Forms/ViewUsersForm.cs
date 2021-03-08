using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserManagementApp.General;

namespace UserManagementApp.Forms
{
    public partial class ViewUsersForm : TemplateForm
    {
        public ViewUsersForm()
        {
            InitializeComponent();
            LoadDataIntoDataGridView();
        }

        private void LoadDataIntoDataGridView()
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usp_Users_LoadDataIntoDataGridView", con)) //specify stored procedure
                {
                    cmd.CommandType = CommandType.StoredProcedure; //declare a command for Stored Procedure

                    if (con.State != ConnectionState.Open) //check if connection with database is established
                        con.Open();
                    DataTable dtRoles = new DataTable(); //create a new DataTable that stores the data

                    SqlDataReader sdr = cmd.ExecuteReader(); //read data from database

                    dtRoles.Load(sdr); //load data into dtRoles datatable

                    UsersDataGridView.DataSource = dtRoles; //embed data in dtRoles in RolesDataGridView forms object for display
                    //RolesDataGridView.Columns[0].Visible = false; //will be used for future lessons
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserForm userForm = new UserForm();
            userForm.ShowDialog();
            LoadDataIntoDataGridView();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
             if (SearchTextBox.Text != string.Empty) //is the textbox is not empty
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_Users_SearchByUserNameorRole", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        //Passing Parameter
                        cmd.Parameters.AddWithValue("@Filter", SearchTextBox.Text.Trim());

                        if (con.State != ConnectionState.Open)
                            con.Open();

                        DataTable dtRole = new DataTable();

                        SqlDataReader sdr = cmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            dtRole.Load(sdr);
                            UsersDataGridView.DataSource = dtRole;
                        }
                        else
                        {
                            MessageBox.Show("No Matching User is found.", "No record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void refreshRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
        }

        private void UsersDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(UsersDataGridView.Rows.Count > 0)
            {
                string userName = UsersDataGridView.SelectedRows[0].Cells[0].Value.ToString();

                UserForm userForm = new UserForm(); //Create a UserForm Instance
                userForm.UserName = userName; //access UserName from userForm.cs
                userForm.IsUpdate = true; //access IsUpdate from userForm.cs
                userForm.ShowDialog();
                LoadDataIntoDataGridView();
            }
        }
    }
}
