using System;
using System.Windows.Forms;
using UserManagementApp.General;

namespace UserManagementApp.Forms
{
    public partial class DashboardForm : TemplateForm
    {
        public DashboardForm()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void exitApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangePasswordForm cpf = new ChangePasswordForm();
            cpf.ShowDialog();
        }

        private void newUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserForm userForm = new UserForm();
            userForm.ShowDialog();
        }

        private void viewUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewUsersForm vuf = new ViewUsersForm();
            vuf.ShowDialog();
        }

        private void newRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RolesForm rf = new RolesForm();
            rf.ShowDialog();
        }

        private void viewRolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewRolesForm vrf = new ViewRolesForm();
            vrf.ShowDialog();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            AdminLabel.Text = LoggedInUser.UserName;

            SetupUserAccess();

            DateLabel.Text = DateTime.Now.ToString("dddd, MMM dd yyyy");
            TimeLabel.Text = DateTime.Now.ToString("hh:mm:ss");
        }

        private void SetupUserAccess()
        {
            switch(LoggedInUser.RoleId)
            {
                case 1:
                    RoleLabel.Text = "Full Rights";
                    break;
                case 2:
                    RoleLabel.Text = "Normal Rights";
                    AdminMenu.Visible = false;
                    break;
                case 3:
                    RoleLabel.Text = "Limited Rights";
                    AdminMenu.Visible = false;
                    break;

            }
        }

        private void AddUserRoleButton_Click(object sender, EventArgs e)
        {
             if(LoggedInUser.RoleId == 1)
            {
                //Display Form and Do what you want
                RolesForm rf = new RolesForm();
                rf.ShowDialog();
            }
             else
            {
                MessageBox.Show("You are not allowed to perform this operation.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void ViewUserRoleButton_Click(object sender, EventArgs e)
        {
            if (LoggedInUser.RoleId == 1)
            {
                //Display Form and Do what you want
                ViewRolesForm vrf = new ViewRolesForm();
                vrf.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to perform this operation.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            UserForm userForm = new UserForm();
            userForm.ShowDialog();
        }

        private void ViewUserButton_Click(object sender, EventArgs e)
        {
            ViewUsersForm vuf = new ViewUsersForm();
            vuf.ShowDialog();
        }

        private void ChangePasswordButton_Click(object sender, EventArgs e)
        {
            ChangePasswordForm cpf = new ChangePasswordForm();
            cpf.ShowDialog();
        }

        private void AddStudentButton_Click(object sender, EventArgs e)
        {
            StudentForm sf = new StudentForm();
            sf.ShowDialog();
        }

        private void ViewStudentButton_Click(object sender, EventArgs e)
        {
            ViewStudentForm vsf = new ViewStudentForm();
            vsf.ShowDialog();
        }
    }
}
