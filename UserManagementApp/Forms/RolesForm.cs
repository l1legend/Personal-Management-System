using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using UserManagementApp.General;

namespace UserManagementApp.Forms
{
    public partial class RolesForm : TemplateForm
    {
        public RolesForm()
        {
            InitializeComponent();
        }

        //Properties to Handle Update Process
        public int RoleId { get; set; }
        public bool IsUpdate { get; set; }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(IsFormValid())
            {
                if(this.IsUpdate)
                {
                    DialogResult result = MessageBox.Show("Are you sure want to updated this Role?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        //Do Update Operation
                        using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString())) //connect to database using AppConnection class and GetConnectionString method
                        {
                            using (SqlCommand cmd = new SqlCommand("usp_Roles_UpdatedRoleByRoleId", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@RoleId", this.RoleId);
                                cmd.Parameters.AddWithValue("@RoleTitle", TitleTextBox.Text.Trim());
                                cmd.Parameters.AddWithValue("@Description", DescriptionTextBox.Text.Trim());
                                cmd.Parameters.AddWithValue("@CreatedBy", LoggedInUser.UserName);

                                if (con.State != ConnectionState.Open)
                                    con.Open();

                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Role is successfully updated in the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ResetFormControl();
                            }
                        }
                    }
                }
                else
                {
                    //Do Insert Operation
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString())) //connect to database using AppConnection class and GetConnectionString method
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_Roles_InsertNewRole", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@RoleTitle", TitleTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Description", DescriptionTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreatedBy", LoggedInUser.UserName);

                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Role is successfully saved in the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControl();
                        }
                    }
                }
            }
        }

        private void ResetFormControl()
        {
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            TitleTextBox.Focus(); //moves cursor to TitleTextBox

            // Check if form is loaded for updated process
            if(this.IsUpdate)
            {
                this.RoleId = 0;
                this.IsUpdate = false;
                SaveButton.Text = "Save Information";
                DeleteButton.Enabled = false;
            }
        }

        private bool IsFormValid()
        {
            if(TitleTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Role Title is Required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TitleTextBox.Focus();
                return false;
            }

            if (TitleTextBox.Text.Length >= 50)
            {
                MessageBox.Show("Role Title length should be less than or equal to 50 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TitleTextBox.Focus();
                return false;
            }

            return true; 

        }

        private void RolesForm_Load(object sender, EventArgs e)
        {
            if(this.IsUpdate == true)
            {
                using(SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_Roles_ReloadDataForUpdate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@RoleId", RoleId);

                        if (con.State != ConnectionState.Open)
                            con.Open();

                        DataTable dtRole = new DataTable();

                        SqlDataReader sdr = cmd.ExecuteReader();

                        dtRole.Load(sdr);

                        DataRow row = dtRole.Rows[0];

                        TitleTextBox.Text = row["RoleTitle"].ToString();
                        DescriptionTextBox.Text = row["Description"].ToString();

                        // Change Controls
                        SaveButton.Text = "Update Role Information";
                        DeleteButton.Enabled = true;
                    }
                }
            }

        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if(this.IsUpdate)
            {
                DialogResult result = MessageBox.Show("Are you sure want to delete this Role?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if(result == DialogResult.Yes)
                {
                    //Delete record from database
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_Roles_DeleteByRoleId", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@RoleId", this.RoleId);

                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Role is successfully deleted from the system.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControl();
                        }
                    }
                }
            }
        }
    }
}
