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
    public partial class UserForm : TemplateForm
    {
        public UserForm()
        {
            InitializeComponent();
        }

        //Properties to handle update and delete operations
        public string UserName { get; set; } //allows UserName to be accessed by ViewUserForm.cs
        public bool IsUpdate { get; set; } //allows IsUpdate to be accessed as a global variable

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            LoadDataIntoRolesComboBox();

            //For Update Process
            if (this.IsUpdate == true)
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_Users_ReloadDataForUpdate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserName", this.UserName);

                        if (con.State != ConnectionState.Open)
                            con.Open();

                        DataTable dtUser = new DataTable();

                        SqlDataReader sdr = cmd.ExecuteReader();

                        dtUser.Load(sdr);

                        DataRow row = dtUser.Rows[0];

                        UserNameTextBox.Text = row["UserName"].ToString();
                        RolesComboBox.SelectedValue = row["RoleId"];
                        IsActiveCheckBox.Checked = Convert.ToBoolean(row["IsActive"]);
                        DescriptionTextBox.Text = row["Description"].ToString();

                        // Change Controls
                        SaveButton.Text = "Update User Information";
                        DeleteButton.Enabled = true;
                    }
                }
            }
        }

        private void LoadDataIntoRolesComboBox()
        {
            //copy this code from LoadDataIntoDataGridView() in ViewRolesForm.cs
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString())) //fetch data from SQL Server
            {
                using (SqlCommand cmd = new SqlCommand("usp_Roles_LoadDataIntoComboBox", con)) //specify stored procedure
                {
                    cmd.CommandType = CommandType.StoredProcedure; //setting commandtype as a Stored Procedure

                    if (con.State != ConnectionState.Open) //check if connection with database is not established
                        con.Open(); //make connection to database
                    DataTable dtRoles = new DataTable(); //create a new DataTable instance that stores the data

                    SqlDataReader sdr = cmd.ExecuteReader(); //read data from database

                    dtRoles.Load(sdr); //load data into dtRoles datatable

                    RolesComboBox.DataSource = dtRoles; //embed data from dtRoles DataTable to RolesComboBox forms object for display
                    RolesComboBox.DisplayMember = "RoleTitle"; //specify what will be displayed in the combobox
                    RolesComboBox.ValueMember = "RoleId"; //assign a value to each item in the combobox
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsFormValid())
            {
                if(this.IsUpdate == true)
                {
                    //Do Update Process
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString())) //connect to database using AppConnection class and GetConnectionString method
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_Users_UpdateUserByUserName", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@OldUserName", this.UserName);
                            cmd.Parameters.AddWithValue("@UserName", UserNameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Password", SecureData.EncryptData(PasswordTextBox.Text.Trim()));
                            cmd.Parameters.AddWithValue("@RoleId", RolesComboBox.SelectedValue);
                            cmd.Parameters.AddWithValue("@IsActive", IsActiveCheckBox.Checked);
                            cmd.Parameters.AddWithValue("@Description", DescriptionTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreatedBy", LoggedInUser.UserName);

                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("User is successfully updated in the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControl();
                        }
                    }
                }
                else 
                {
                    //Do Insert Operation
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString())) //connect to database using AppConnection class and GetConnectionString method
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_Users_InsertNewUser", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@UserName", UserNameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Password", SecureData.EncryptData(PasswordTextBox.Text.Trim()));
                            cmd.Parameters.AddWithValue("@RoleId", RolesComboBox.SelectedValue);
                            cmd.Parameters.AddWithValue("@IsActive", IsActiveCheckBox.Checked);
                            cmd.Parameters.AddWithValue("@Description", DescriptionTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreatedBy", LoggedInUser.UserName );

                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("User is successfully saved in the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControl();
                        }
                    }
                }
            }  
        }

        private void ResetFormControl()
        {
            UserNameTextBox.Clear();
            PasswordTextBox.Clear();
            RolesComboBox.SelectedIndex = 0;
            IsActiveCheckBox.Checked = true;
            DescriptionTextBox.Clear();

            UserNameTextBox.Focus();

            if(this.IsUpdate)
            {
                this.IsUpdate = false;
                SaveButton.Text = "Save User Information";
                DeleteButton.Enabled = false;
                this.UserName = null;
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

            if (UserNameTextBox.Text.Length >= 50)
            {
                MessageBox.Show("Role Title length should be less than or equal to 50 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UserNameTextBox.Focus();
                return false;
            }

            if (PasswordTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Password is Required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UserNameTextBox.Focus();
                return false;
            }

            if (PasswordTextBox.Text.Length >= 50)
            {
                MessageBox.Show("Password length should be less than or equal to 50 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UserNameTextBox.Focus();
                return false;
            }

            if (RolesComboBox.SelectedIndex == -1) //if user doesn't select an option in the combobox
            {
                MessageBox.Show("Please Select user role from drop down.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RolesComboBox.Focus();
                return false;
            }

            return true;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.IsUpdate == true)
            {
                DialogResult result = MessageBox.Show("Are you sure, you want to delete this User?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    //Delete user from database
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_Users_DeleteUserByUserName", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@UserName", this.UserName);

                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("User is successfully deleted from the system.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControl();
                        }
                    }

                }
                else
                {
                    MessageBox.Show("You cancelled this process.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}
