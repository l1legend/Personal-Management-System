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
    public partial class ChangePasswordForm : TemplateForm
    {
        public ChangePasswordForm()
        {
            InitializeComponent();
        }

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {

        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChangePasswordButton_Click(object sender, EventArgs e)
        {
            if(IsFormValid())
            {
                //Verify Existing Password
                if(IsPasswordVerified())
                {
                    // Go and Update Password
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString())) //connect to database using AppConnection class and GetConnectionString method
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_Users_ChangePassword", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@UserName", LoggedInUser.UserName);
                            cmd.Parameters.AddWithValue("@NewPassword", SecureData.EncryptData(NewPasswordTextBox.Text.Trim()));
                            cmd.Parameters.AddWithValue("@CreatedBy", LoggedInUser.UserName);

                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Password  is successfully changed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControl();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Your old password is not correct, please enter correct password.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetFormControl()
        {
            NewPasswordTextBox.Clear();
            OldPasswordTextBox.Clear();
            ConfirmPasswordTextBox.Clear();

            OldPasswordTextBox.Focus();

        }

        private bool IsPasswordVerified()
        {
            bool isCorrect = false;

            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usp_Users_VerifyPassword", con)) //specify stored procedure
                {
                    cmd.CommandType = CommandType.StoredProcedure; //declare a command for Stored Procedure

                    cmd.Parameters.AddWithValue("@UserName", LoggedInUser.UserName);
                    cmd.Parameters.AddWithValue("@Password", SecureData.EncryptData(OldPasswordTextBox.Text.Trim()));
                    

                    if (con.State != ConnectionState.Open) //check if connection with database is established
                        con.Open();

                    SqlDataReader sdr = cmd.ExecuteReader(); //read data from database

                    if (sdr.HasRows)
                    {
                        isCorrect = true;
                    }
                }
            }

            return isCorrect;
        }

        private bool IsFormValid()
        {
            if (OldPasswordTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Old password is Required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OldPasswordTextBox.Focus();
                return false;
            }

            if (NewPasswordTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("New password is Required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NewPasswordTextBox.Focus();
                return false;
            }

            if (ConfirmPasswordTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Confirm password is Required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConfirmPasswordTextBox.Focus();
                return false;
            }

            if (NewPasswordTextBox.Text.Trim() != ConfirmPasswordTextBox.Text.Trim())
            {
                MessageBox.Show("New and Confirm password does not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConfirmPasswordTextBox.Focus();
                return false;
            }

            return true;

        }
    }
}
