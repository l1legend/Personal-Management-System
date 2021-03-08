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
    public partial class LoginForm : TemplateForm
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if(IsFormValid())
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_Users_VerifyLoginDetails", con)) //specify stored procedure
                    {
                        cmd.CommandType = CommandType.StoredProcedure; //declare a command for Stored Procedure

                        cmd.Parameters.AddWithValue("@UserName", UserNameTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@Password", SecureData.EncryptData(PasswordTextBox.Text.Trim()));

                        if (con.State != ConnectionState.Open) //check if connection with database is established
                            con.Open();

                        DataTable dtUser = new DataTable(); //create a new DataTable that stores the data

                        SqlDataReader sdr = cmd.ExecuteReader(); //read data from database

                        if(sdr.HasRows)
                        {
                            dtUser.Load(sdr);
                            DataRow userRow = dtUser.Rows[0]; //get data from user role database

                            LoggedInUser.UserName = userRow["UserName"].ToString(); //store UserName to LoggeddInUser class
                            LoggedInUser.RoleId = Convert.ToInt32(userRow["RoleId"]); //store RoleId to LoggedInUser class

                            this.Hide();
                            DashboardForm dashboardForm = new DashboardForm(); //create a new instance of DashboardForm object
                            dashboardForm.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("User Name or Password is incorrect.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private bool IsFormValid()
        {
            if (UserNameTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("User name is Required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UserNameTextBox.Focus();
                return false;
            }

            if (PasswordTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Password is Required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                PasswordTextBox.Focus();
                return false;
            }

            return true;
        }
    }
}
