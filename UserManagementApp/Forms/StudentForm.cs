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
using System.IO;

namespace UserManagementApp.Forms
{
    public partial class StudentForm : TemplateForm
    {
        public StudentForm()
        {
            InitializeComponent();

        }

        //Properties to handle update and delete operations
        public int StudentId { get; set; }
        public bool IsUpdate { get; set; } //allows IsUpdate to be accessed as a global variable

        private void StudentForm_Load(object sender, EventArgs e)
        {
            //For Update Process
            if (this.IsUpdate == true)
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_Students_ReloadDataForUpdate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@StudentId", this.StudentId);


                        if (con.State != ConnectionState.Open)
                            con.Open();

                        DataTable dtStudent = new DataTable();

                        SqlDataReader sdr = cmd.ExecuteReader();

                        dtStudent.Load(sdr);

                        DataRow row = dtStudent.Rows[0];

                        StudentNameTextBox.Text = row["StudentName"].ToString();
                        AgeTextBox.Text = row["Age"].ToString();
                        GenderTextBox.Text = row["Gender"].ToString();
                        DescriptionTextBox.Text = row["Description"].ToString();

                        var pic = row.Field<byte[]>("Image");
                        if (pic != null)
                            {
                                MemoryStream ms = new MemoryStream(pic);
                                IdPictureBox.Image = Image.FromStream(ms);
                            }


                        // Change Controls
                        SaveButton.Text = "Update Student Information";
                        DeleteButton.Enabled = true;
                    }
                }
            }
        }
    

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if(IsFormValid())
            {
                if(this.IsUpdate == true)
                {
                    //Do Update Process
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString())) //connect to database using AppConnection class and GetConnectionString method
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_Students_UpdateStudentByStudentId", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@StudentId", this.StudentId);
                            cmd.Parameters.AddWithValue("@StudentName", StudentNameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Age", AgeTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Gender", GenderTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Description", DescriptionTextBox.Text.Trim());
                            var image = IdPictureBox.Image;
                            using (var ms = new MemoryStream())
                            {
                                if (image != null)
                                {
                                    image.Save(ms, image.RawFormat);
                                    cmd.Parameters.AddWithValue("@Image", SqlDbType.VarBinary).Value = ms.ToArray();
                                }
                                else
                                {
                                    cmd.Parameters.Add("@Image", SqlDbType.VarBinary).Value = DBNull.Value; //save null to database
                                }
                            }
                            cmd.Parameters.AddWithValue("@CreatedBy", LoggedInUser.UserName);


                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Student is successfully updated in the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControl();
                        }
                    }
                }
                else
                {
                    //Do Insert Process
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString())) //connect to database using AppConnection class and GetConnectionString method
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_Students_InsertNewStudent", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@StudentName", StudentNameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Age", AgeTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Gender", GenderTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Description", DescriptionTextBox.Text.Trim());

                          
                                var image = IdPictureBox.Image;
                                using (var ms = new MemoryStream())
                                {
                                    if (image != null)
                                    {
                                        image.Save(ms, image.RawFormat);
                                        cmd.Parameters.AddWithValue("@Image", SqlDbType.VarBinary).Value = ms.ToArray();
                                    }
                                    else
                                    {
                                        cmd.Parameters.Add("@Image", SqlDbType.VarBinary).Value = DBNull.Value; //save null to database
                                    }
                                }
                            
                            cmd.Parameters.AddWithValue("@CreatedBy", LoggedInUser.UserName);


                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Student is successfully added in the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControl();
                        }
                    }
                }
            }
        }

        private void ResetFormControl()
        {
            StudentNameTextBox.Clear();
            AgeTextBox.Clear();
            GenderTextBox.Clear();
            DescriptionTextBox.Clear();
            IdPictureBox.Dispose();

            StudentNameTextBox.Focus();

            if(this.IsUpdate)
            {
                this.StudentId = 0;
                this.IsUpdate = false;
                SaveButton.Text = "Save User Information";
                DeleteButton.Enabled = false;
                ViewStudentForm vsf = new ViewStudentForm();
                //vsf.LoadDataIntoDataGridView();
                vsf.StudentDataGridView.Refresh();

            }
        }

        private bool IsFormValid()
        {
            if (StudentNameTextBox.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Student name is Required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StudentNameTextBox.Focus();
                return false;
            }

            if (StudentNameTextBox.Text.Length >= 200)
            {
                MessageBox.Show("Student Name length should be less than or equal to 200 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StudentNameTextBox.Focus();
                return false;
            }
            return true;
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            String ImageLocation = "";
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                   ImageLocation = dialog.FileName;
                   IdPictureBox.ImageLocation = ImageLocation;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An Error Occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IdPictureBox_DoubleClick(object sender, EventArgs e)
        {
            var image = IdPictureBox.Image;

            if (image != null)
            {
                PopUpImage pui = new PopUpImage();
                pui.BackgroundImage = IdPictureBox.Image;
                pui.BackgroundImageLayout = ImageLayout.Stretch;
                pui.Show();

            }
        }

        private void ClearPictureButton_Click(object sender, EventArgs e)
        {
            IdPictureBox.Image = null;
            IdPictureBox.Update();
            GC.Collect();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if(this.IsUpdate == true)
            {
                DialogResult result = MessageBox.Show("Are you sure, you want to delete this User?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if(result == DialogResult.Yes)
                {
                    //Delete user from database
                    using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_Student_DeleteStudentById", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@StudentId", this.StudentId);

                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Student is successfully deleted from the system.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControl();
                            
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You cancelled this process", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
