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
    public partial class ViewStudentForm : TemplateForm
    {
        public ViewStudentForm()
        {
            InitializeComponent();
            LoadDataIntoDataGridView();
        }


        public void LoadDataIntoDataGridView()
        {
            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usp_Students_LoadDataIntoDataGridView ", con)) //specify stored procedure
                {
                    cmd.CommandType = CommandType.StoredProcedure; //declare a command for Stored Procedure

                    if (con.State != ConnectionState.Open) //check if connection with database is established
                        con.Open();
                    DataTable dtStudents = new DataTable(); //create a new DataTable that stores the data

                    SqlDataReader sdr = cmd.ExecuteReader(); //read data from database

                    dtStudents.Load(sdr); //load data into dtRoles datatable

                    StudentDataGridView.DataSource = dtStudents; //embed data in dtRoles in RolesDataGridView forms object for display
                    //StudentDataGridView.Columns[0].Visible = false; //will be used for future lessons
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StudentForm sf = new StudentForm();
            sf.ShowDialog();
        }

        private void refreshRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
            SearchTextBox.Clear();
            SearchTextBox.Focus();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (SearchTextBox.Text != string.Empty) //is the textbox is not empty
            {
                using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_Students_SearchByStudentName", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        //Passing Parameter
                        cmd.Parameters.AddWithValue("@Filter", SearchTextBox.Text.Trim());

                        if (con.State != ConnectionState.Open)
                            con.Open();

                        DataTable dtStudent = new DataTable();

                        SqlDataReader sdr = cmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            dtStudent.Load(sdr);
                            StudentDataGridView.DataSource = dtStudent;
                        }
                        else
                        {
                            MessageBox.Show("No Matching Student is found.", "No record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void StudentDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(StudentDataGridView.Rows.Count > 0)
            {
                int StudentId = Convert.ToInt32(StudentDataGridView.SelectedRows[0].Cells[0].Value);            
                StudentForm studentForm = new StudentForm();
                studentForm.StudentId = StudentId;
                studentForm.IsUpdate = true;
                studentForm.ShowDialog();
            }
        }
    }
}
